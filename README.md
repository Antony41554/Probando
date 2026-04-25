# TeejoshSystem

## Resumen

**TeejoshSystem** es una aplicación de escritorio offline-first para la gestión de inventarios de coleccionables (Hot Wheels, Funkos, TCG, Toys y otros).  
  
El sistema está diseñado bajo principios de **Clean Architecture** y **Hexagonal Architecture**, incorporando **DDD, CQRS y MVVM** cuando aportan valor real al dominio.  

Funciona offline-first: sin conexión a red, sin servicios externos en runtime, con base de datos local. La UI es Avalonia, verificada en Windows y Linux (Fedora 43).
  
> No es un proyecto orientado a frameworks — es un proyecto orientado a estabilidad, evolución y desacoplamiento.  
  
---
  
## Objetivo del Sistema  
  
El objetivo no es solo gestionar inventario, sino construir una base sólida que permita:  
  
- Evolucionar la UI sin afectar el dominio  
- Cambiar la base de datos sin reescribir lógica de negocio  
- Incorporar nuevos canales (API, CLI, mobile) sin modificar el core  
- Mantener reglas de negocio consistentes y centralizadas

---

## Estado del Proyecto

|Campo|Valor|
|---|---|
|**Versión**|0.4 — beta|
|**Estado**|Activo|
|**UI**|Avalonia (migración desde WPF completada parcialmente)|
|**Plataformas verificadas**|Windows, Linux (Fedora 43)|
|**Persistencia**|SQLite — Code-First con migraciones EF Core|
|**Arquitectura**|Estable|
|**Última actualización**|Abril 2026|

---

## Capacidades

- Gestión de 5 tipos de producto con campos distintos entre sí
- Formularios dinámicos que adaptan sus campos según el tipo seleccionado
- Catálogos relacionales con carga en cascada (TCG: Franquicia → Expansiones/Packs)
- Validaciones en tiempo real sobre cada campo
- Búsqueda y filtrado por nombre y tipo
- CRUD completo con confirmaciones antes de acciones destructivas
- Operación completamente offline

---

## Arquitectura

### Modelo C4

La arquitectura está modelada mediante **C4 Model** usando Structurizr DSL.

> Los diagramas se generan desde el archivo `.dsl` — no están incrustados en este documento.

Niveles documentados:

- **System Context** → El usuario interactúa con TeejoshSystem
- **Container** → UI, Application, Domain, Infrastructure, Database
- **Component** → ViewModels, casos de uso, puertos, repositorios, DbContext

---

### Capas y reglas de dependencia

El sistema sigue una estructura en capas con dependencias dirigidas hacia el dominio.

```
UI (Avalonia)
      ↓
Infrastructure ──→ Domain
      ↓                ↑
  Application ─────────┘
```

|Capa|Rol|Depende de|NO Depende de|
|---|---|---|---|
|**Domain**|Núcleo: entidades, value objects, contratos|Nada|Todas|
|**Application**|Casos de uso, comandos, queries, DTOs|Domain|Infrastructure, UI|
|**Infrastructure**|Persistencia, EF Core, repositorios|Domain, Application| UI|
|**UI (Avalonia)**|Presentación: ViewModels, Views, Services|Application|Domain|

> Infrastructure puede referenciar Application únicamente para implementar sus interfaces (outbound ports). Nunca para invocar casos de uso.

La dirección de las dependencias no es una convención — es una garantía. Significa que Domain compila sin conocer que existe EF Core, Avalonia, o SQL Server. Estas reglas no son rígidas por dogma, sino guías para mantener bajo acoplamiento y facilitar cambios futuros.

### Violaciones no aceptables

- Referenciar EF Core, SQL o cualquier framework externo desde `Domain`
- Acceder a repositorios directamente desde un ViewModel
- Instanciar `DbContext` fuera de Infrastructure
- Introducir lógica de negocio en ViewModels o code-behind

### Trade-offs de la Arquitectura

Esta arquitectura prioriza mantenibilidad y desacoplamiento, pero introduce ciertos costos:  
  
#### Complejidad Inicial 

- Más capas, más clases, más abstracciones  
- Curva de aprendizaje mayor  
  
#### Overhead Estructural 

- Para features simples, puede sentirse “pesado”  
- CQRS introduce más archivos (commands, queries, handlers)  
  
#### Performance (Potencial)

- TPT implica joins adicionales  
- Uso de MediatR agrega indirección  
  
#### Justificación

Estos trade-offs son aceptables porque:  
- El dominio tiene múltiples variantes estructurales  
- Se espera evolución del sistema (UI, DB, features)  
- La mantenibilidad es prioritaria sobre la simplicidad inicial

---

### Flujo de ejecución (C4 - Component)

```
Views (Avalonia XAML)
  ↓ data binding
ViewModels
  ↓ MediatR.Send(command/query)
Application Handler
  ↓ orquesta entidades y value objects
Domain (reglas de negocio, invariantes)
  ↓ IProductoRepository / ICatalogoRepository
Infrastructure (EF Core)
  ↓
Base de datos (SQLite)
  ↑
Result<T> / DTOs regresa encapsulado por toda la cadena
```

---

## Patrones y Decisiones

Esta sección explica qué resuelve cada patrón, por qué se eligió, y qué problema concreto habría sin él.

---

### Clean Architecture

**Qué es:** Organización del sistema en capas concéntricas donde las dependencias siempre apuntan hacia adentro (hacia el dominio). Ninguna capa interna conoce las externas.

**Qué problema resuelve:** Sin esta separación, la lógica de negocio termina acoplada a la base de datos, al framework de UI, o a ambos. Cualquier cambio en EF Core, en WPF/Avalonia, o en el esquema SQL arrastraría cambios en las reglas del negocio — que son la parte más estable y valiosa del sistema.

**Por qué importa aquí:** La migración de WPF a Avalonia fue posible sin tocar Domain ni Application. Si la arquitectura estuviera acoplada, esa migración habría requerido reescribir lógica de negocio junto con la UI.

---

### Hexagonal Architecture (Ports & Adapters)

**Qué es:** El dominio define contratos (Ports) para todo lo que necesita del exterior. El exterior (UI, base de datos, APIs) los implementa mediante Adapters. El dominio nunca llama directamente a ninguna implementación concreta.

**Qué problema resuelve:** Sin esto, el dominio está atado a tecnologías específicas. Si mañana se reemplaza SQL Server por SQLite, o se agrega una API REST, o se migra de Avalonia a MAUI, el dominio no debería necesitar cambios.

**Inbound Adapters actuales:** UI Avalonia  
**Outbound Adapters actuales:** EF Core + SQLite  
**Puertos definidos:** `IProductoRepository`, `ICatalogoRepository`

**Por qué importa aquí:** La sección [[#Evolución Planeada]] lista cambios de infraestructura que no requieren tocar el dominio, exactamente porque los ports están bien definidos.

---

### Domain-Driven Design (DDD)

**Qué es:** Modelar el software a partir del dominio del problema — en este caso, el negocio de coleccionables. Las entidades representan conceptos reales del negocio, los value objects encapsulan reglas, y los invariantes se protegen desde adentro.

**Qué problema resuelve:** Sin DDD, las reglas de negocio se dispersan: validaciones en ViewModels, lógica de cálculo en handlers, restricciones en la base de datos. El resultado es que no hay un único lugar donde confiar para saber "qué es válido".

**Aplicación concreta:**

- `Producto` es la entidad raíz del agregado
- `NombreProducto`, `Precio`, `Unidades` son value objects inmutables que validan sus propias reglas — un `Precio` inválido no puede existir en ningún punto del sistema
- `ProductoDetalle` (abstracta) y sus subtipos modelan la variabilidad estructural de cada tipo de coleccionable
- `Producto.AsignarDescripcion` valida que el detalle sea consistente con el tipo del producto — un HotWheelsDetalle no puede asignarse a un Producto de tipo Funko
- Los catálogos (`TcgFranquicia`, `TcgExpansion`, etc.) son entidades de referencia con identidad propia

---

### CQRS (Command Query Responsibility Segregation)

**Qué es:** Separar explícitamente las operaciones que modifican estado (Commands) de las que solo leen (Queries). Cada intención tiene su propio objeto y su propio handler.

**Qué problema resuelve:** Sin CQRS, un mismo método hace demasiado: valida, persiste, y devuelve datos para la UI al mismo tiempo. Esto dificulta mantener, testear y razonar sobre el código. Con CQRS, `CrearProductoCommand` solo sabe crear, y `ObtenerProductosQuery` solo sabe leer.

**Implementación:** CQRS simplificado — sin Event Sourcing, sin event store. MediatR actúa como dispatcher. Los comandos retornan `Result<T>`, las queries retornan DTOs.

**¿Por qué sin Event Sourcing?** El sistema no requiere reconstruir estado a partir de eventos ni mantener historial de cambios intermedios. Event Sourcing agregaría complejidad operativa sin beneficio concreto en este contexto. Si el módulo de auditoría (roadmap) lo requiere en el futuro, es un cambio de Infrastructure, no de Domain.
  
#### Trade-off  
  
- Más clases y handlers  
- Mayor verbosidad  
  
Se acepta este costo a cambio de mayor claridad y separación de responsabilidades.

---

### MVVM (Model-View-ViewModel)

**Qué es:** Separar la vista (XAML) de su lógica de presentación (ViewModel) mediante data binding. La vista no tiene lógica; el ViewModel no conoce la vista.

**Qué problema resuelve:** Sin MVVM, la lógica de presentación vive en code-behind: está atada al framework de UI, no es testeable, y mezcla estado con comportamiento visual. Con MVVM, los ViewModels son clases C# puras que pueden testearse sin levantar la UI.

**Por qué importa en la migración WPF → Avalonia:** Los ViewModels no cambiaron. Solo cambiaron los archivos `.xaml` y la reimplementación de `INotificationService` e `IConfirmationService`. Esto es exactamente el contrato que MVVM garantiza.

---

### Repository Pattern

**Qué es:** Abstraer el acceso a datos detrás de una interfaz. El dominio define qué necesita (`IProductoRepository`), la infraestructura decide cómo lo obtiene.

**Qué problema resuelve:** Sin este patrón, los handlers de Application usarían `DbContext` directamente — acoplando la capa de casos de uso a EF Core. Cambiar la base de datos requeriría modificar Application.

**Implementación:** Los repositorios viven en Infrastructure. Los ports (interfaces) viven en Domain. Application solo conoce los ports.

---

### Result Pattern

**Qué es:** Todos los casos de uso retornan `Result.Success(valor)` o `Result.Failure(error)` en lugar de lanzar excepciones para el flujo de control.

**Qué problema resuelve:** Las excepciones tienen overhead (captura de stack trace) y hacen implícitos los caminos de error — el caller puede ignorarlos o capturarlos incorrectamente. Con `Result<T>`, el caller está obligado a manejar el caso de error, los mensajes son descriptivos para el usuario, y no hay costo de excepciones en rutas normales.

---

### Mediator (MediatR)

**Qué es:** Un despachador central que recibe un comando o query y lo dirige al handler correcto, sin que el emisor conozca al receptor.

**Qué problema resuelve:** Sin Mediator, los ViewModels tendrían referencias directas a múltiples servicios de Application. Con MediatR, el ViewModel solo conoce `IMediator`. Agregar un nuevo caso de uso no requiere modificar la UI.

---

### Shell Pattern (navegación por estado)

**Qué es:** `MainViewModel.CurrentView` controla qué vista está activa. Los DataTemplates en XAML mapean cada ViewModel a su View automáticamente. Los ViewModels se resuelven desde el contenedor DI.

**Por qué no NavigationService:** Introducir un NavigationService implica historial, parámetros de ruta, y deep linking. Ninguna de esas necesidades existe aquí. El Shell Pattern resuelve la navegación de forma simple, sin overhead, y consistente con MVVM.

---

## Estructura del Proyecto

Estructura real según el repositorio:

```
TeejoshSystem/
├── README.md
├── TeejoshSystem.slnx
│
├── TeejoshSystem.Domain/
│   ├── Entities/
│   │   ├── Producto.cs
│   │   ├── Catalogos/        # FunkoSubtipo, TcgFranquicia, TcgExpansion...
│   │   └── Detalles/         # FunkoDetalle, HotWheelsDetalle, TcgDetalle...
│   ├── ValueObjects/
│   │   ├── NombreProducto.cs
│   │   ├── Precio.cs
│   │   └── Unidades.cs
│   ├── Enums/
│   │   └── TipoProducto.cs
│   └── Ports/
│       └── Outbound/
│           └── Repositories/
│               ├── ProductoBusquedaResult.cs
│               ├── IProductoRepository.cs
│               └── ICatalogoRepository.cs
│
├── TeejoshSystem.Application/
│   ├── Common/
│   │   ├── Result.cs
│   │   └── Dtos/
│   │       ├── CatalogoItemDto.cs
│   │       ├── ProductoDetalladoDto.cs
│   │       └── ProductoDto.cs
│   └── Ports/
│       └── Inbound/
│           ├── Productos/
│           │   ├── Commands/   # Crear, Actualizar, Eliminar
│           │   └── Queries/    # ObtenerProductos, BuscarProductos, ObtenerPorId
│           └── Catalogos/
│               └── Queries/    # ObtenerCatalogos, ObtenerExpansionesYPacks
│
├── TeejoshSystem.Infrastructure/
│   ├── Adapters/
│   │   └── Outbound/
│   │       └── Persistence/
│   │           ├── InventarioDbContext.cs
│   │           ├── InventarioDbContextFactory.cs
│   │           ├── Configurations/   # Fluent API — 12 archivos
│   │           ├── Migrations/       # Migracion de SQLite
│   │           └── Repositories/
│   │               ├── ProductoRepository.cs
│   │               └── CatalogoRepository.cs
│   └── DependencyInjection/
│       ├── InfrastructureServiceRegistration.cs
│       └── PersistenceServiceRegistration.cs
│
└── TeejoshSystem.AvaloniaUI/
    ├── Program.cs
    ├── App.axaml / App.axaml.cs
    ├── MainWindow.axaml / MainWindow.axaml.cs
    ├── appsettings.json
    └── Adapters/
        └── Inbound/
            ├── ViewModels/     # Shell, Menu, Productos, Common
            ├── Views/          # Menu, Productos
            └── Services/       # INotificationService, IConfirmationService...
```

---

## Modelo de Base de Datos

### Tabla principal

```sql
product (
    id    INTEGER PRIMARY KEY AUTOINCREMENT,
    type  TEXT    NOT NULL,
    name  TEXT    NOT NULL,
    price REAL    NOT NULL,
    units INTEGER NOT NULL
)
```

### Tablas de detalles (1:1 con `product`, TPC)

```sql
hot_wheels (product_id PK, model, year, serie, category_id)
funko      (product_id PK, box_number, license, subtype_id, special_feature_id)
tcg        (product_id PK, pack_id, expansion_id)
toy        (product_id PK, min_years, min_players, max_players, board_game)
varios     (product_id PK, brand, height, width, length, material, illustration)
```

Todas con `FOREIGN KEY (product_id) REFERENCES product(id) ON DELETE CASCADE`.

### Tablas de catálogos

```sql
hot_wheels_category    (id, name)
funko_subtype          (id, name)
funko_special_feature  (id, name)
tcg_franchise          (id, name)
tcg_expansion          (id, name, franchise_id)
tcg_pack               (id, name, franchise_id)
```

### Tablas de control

```sql
__EFMigrationsHistory  # Control interno de EF Core — no modificar manualmente
```

### Por qué Table-Per-Container (TPC) y no Table-Per-Type (TPT) ó Table-Per-Hierarchy (TPH)

- `TPH` almacena todos los subtipos en una sola tabla con columnas `nullable`. Con 5 tipos de coleccionable estructuralmente muy distintos, generaría una tabla con mayoría de NULLs e integridad referencial débil.
- `TPT` estándar requiere una tabla base para `ProductoDetalle` con solo `ProductoId` — una tabla de una columna que solo sirve de pivot, y EF Core falla al intentar crearla para una clase abstracta.
- `TPC` — cada subtipo concreto tiene su propia tabla completa, sin tabla base. La herencia existe en C# para compartir comportamiento (`ProductoId`, `AsignarProductoId`), pero no se refleja en el esquema. La relación con product se gestiona via `ProductoId` y el repositorio resuelve el detalle correcto usando `Producto.Tipo`.
Se implementa en EF Core con `builder.HasBaseType((Type)null)` en la configuración de cada detalle.

---

## Tecnologías

### Backend (.NET 8)

|Librería|Versión|Propósito|
|---|---|---|
|.NET|8.0|Framework base|
|Entity Framework Core|8.0.0|ORM|
|Microsoft.EntityFrameworkCore.Sqlite|8.0.0|Proveedor SQLite|
|Microsoft.EntityFrameworkCore.Design|8.0.0|EF Core Tools (design time)|
|MediatR|12.2.0|Dispatcher CQRS|
|FluentValidation|11.9.0|Validaciones de Application (opcional)|

### Frontend (Avalonia)

|Librería|Versión|Propósito|
|---|---|---|
|Avalonia|11.3.12|UI multiplataforma (Windows, Linux, macOS)|
|Avalonia.Controls.DataGrid|11.3.12|DataGrid (paquete separado, requiere StyleInclude)|
|~~MsBox.Avalonia~~|-|Diálogos de notificación y confirmación|
|CommunityToolkit.Mvvm|8.2.2|ObservableObject, RelayCommand|
|Microsoft.Extensions.Hosting|8.0.0|DI container|
|Microsoft.Extensions.Configuration|8.0.0|appsettings.json|
|Microsoft.EntityFrameworkCore.Design|8.0.0|Requerido por EF Tools en startup project|

---

## Persistencia — Code-First con SQLite

### Ubicación de la base de datos

```
Windows: C:\Users\<usuario>\AppData\Local\TeejoshSystem\inventario.db
Linux:   ~/.local/share/TeejoshSystem/inventario.db
```

### Comandos de migraciones

```bash
# Generar nueva migración
dotnet ef migrations add <NombreMigracion> \
    --project TeejoshSystem.Infrastructure \
    --startup-project TeejoshSystem.AvaloniaUI \
    --output-dir Adapters/Outbound/Persistence/Migrations

# Aplicar migraciones pendientes
dotnet ef database update \
    --project TeejoshSystem.Infrastructure \
    --startup-project TeejoshSystem.AvaloniaUI

# Revertir última migración
dotnet ef migrations remove \
    --project TeejoshSystem.Infrastructure \
    --startup-project TeejoshSystem.AvaloniaUI
```

### Aplicación automática al arrancar
`App.axaml.cs` llama `db.Database.Migrate()` en el startup. En una instalación nueva, crea la BD y aplica todas las migraciones. En instalaciones existentes, aplica solo las pendientes.

---

## Evolución Planeada

### ~~Database-First → Code-First~~ — Completado

**Estado actual:** La base de datos preexiste. EF Core mapea las tablas mediante Fluent API. No hay migraciones.

**Por qué migrar:** Con Database-First, los cambios de esquema requieren intervención manual en dos lugares (BD y configuraciones). Es frágil y no versionable. Code-First invierte esto: las clases del dominio y sus configuraciones Fluent definen el esquema, las migraciones versionan la BD junto con el código.

**Impacto por capa al migrar:**

|Capa|Cambio|
|---|---|
|Domain|Ninguno|
|Application|Ninguno|
|Infrastructure|Se agregan `Migrations/`. DbContext pasa a ser la fuente de verdad del esquema. Las configuraciones Fluent se mantienen y evolucionan.|
|Base de datos|Generada y versionada desde código|

---

### Elección de base de datos: SQLite sobre SQL Server Express

Para un sistema de escritorio offline-first, de usuario único, con importación/exportación de Excel en el roadmap, **SQLite es la opción más adecuada**.

|Criterio|SQL Server Express|SQLite|
|---|---|---|
|Proceso de servidor|Requiere instalación y servicio en background|No — archivo único|
|Multiplataforma|Windows only (sin configuración extra)|Windows, Linux, macOS nativo|
|Instalación para el usuario|Compleja|Ninguna — incluida en el binario|
|EF Core + Code-First + Migrations|✓|✓|
|Importación/exportación Excel|Indiferente|Indiferente|
|Backup|Requiere herramientas SQL|Copiar el archivo `.db`|
|Concurrencia multiusuario|✓|No necesaria en este contexto|
|Tamaño para coleccionables|Sobredimensionado|Ajustado|

El único escenario donde SQL Server gana es acceso concurrente de múltiples usuarios sobre red — que no aplica aquí. El resto de características de SQL Server Express son overhead innecesario para una aplicación de escritorio de usuario único.

**Cambio de proveedor con Code-First:** reemplazar el proveedor en `PersistenceServiceRegistration.cs` y regenerar migraciones. Domain y Application no se tocan.

---

### UI: Migración a Avalonia — Parcial

La migración de WPF a Avalonia está **parcialmente completada**. El proyecto `TeejoshSystem.AvaloniaUI` reemplaza al anterior `WPF/`.

**Verificado en:** Windows y Linux (Fedora 43).

**Qué cambió:** Solo la capa de presentación — Views reescritas en XAML de Avalonia, reimplementación de `INotificationService` e `IConfirmationService`, ajuste de bindings.

**Qué no cambió:** Domain, Application, Infrastructure — exactamente como garantiza la arquitectura.

> Si la migración de UI hubiera requerido cambios en capas internas, habría indicado una violación arquitectónica.

---

## Problemas Conocidos

### ~~Filtrado por tipo ejecutado en memoria~~ — Resuelto

**Causa:** La tabla `product` no tiene columna discriminadora `type`. El filtrado se hace mediante joins en memoria a las tablas de detalle.  
**Solución aplicada:** Se agregó la columna `type` en `product`. `SearchAsync` ahora filtra directamente en la query SQL sin cargar todos los productos en memor

---

### ~~"Invalid object name 'ProductoDetalle'"~~ — Resuelto

**Causa:** EF Core intentaba crear una tabla para la clase base abstracta `ProductoDetalle`.  
**Solución aplicada:** `builder.HasBaseType((Type)null)` en la configuración Fluent de cada clase derivada. EF Core trata cada detalle como entidad independiente (TPC).

---

### ~~Detalles no se guardaban al crear producto~~ — Resuelto

**Causa:** Faltaban los métodos `Add...DetalleAsync()` en el repositorio.  
**Solución aplicada:** `Add...DetalleAsync()` implementados en `ProductoRepository` e invocados desde `CrearProductoCommandHandler`.

---

### ObtenerProductoPorId — Query pendiente de implementar

**Estado:** `ProductoDetalladoDto` existe en `Common/Dtos/`. Faltan `ObtenerProductoPorIdQuery.cs` y `ObtenerProductoPorIdQueryHandler.cs`.
**Impacto:** La pantalla de edición no puede cargar el detalle completo de un producto por ID.

---

### ~~DataGrid requiere StyleInclude explícito~~ — Resuelto

**Causa:** `Avalonia.Controls.DataGrid` es un paquete separado que requiere `<StyleInclude Source="avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml"/>` en `App.axaml`.
**Solución:** Aplicada. Documentado para futuros controles externos.

---

## Extensibilidad

La arquitectura hexagonal permite conectar nuevos adaptadores sin modificar el core.

### Nuevos Inbound Adapters (sin tocar Domain ni Application)

- API REST con ASP.NET Core
- CLI
- MAUI para mobile

Solo se necesita un nuevo proyecto que invoque los inbound ports existentes vía MediatR.

### Nuevos Outbound Adapters (sin tocar Domain ni Application)

- APIs externas de precios (ej.: TCGPlayer API)
- Cache en memoria para catálogos

Solo se necesita implementar `IProductoRepository` o `ICatalogoRepository` con la nueva tecnología y registrarla en DI.

---

## Convenciones

### Nomenclatura

|Elemento|Convención|Ejemplo|
|---|---|---|
|Clases|PascalCase|`ProductoRepository`|
|Interfaces|I + PascalCase|`IProductoRepository`|
|Métodos|PascalCase|`GetByIdAsync`|
|Variables privadas|`_` + camelCase|`_context`|
|Variables locales|camelCase|`producto`|
|Métodos async|Sufijo `Async`|`AddAsync`|

### Reglas arquitectónicas

- Ports → siempre interfaces
- Adapters → siempre implementan Ports
- DTOs → solo en Application
- Entidades de dominio → nunca cruzan hacia la UI
- Validaciones de negocio → en Domain (value objects)
- Validaciones de presentación → en la UI (INotifyDataErrorInfo)
- Propiedades string en entidades EF → `/= null!` (constructor privado para EF)
- Propiedades string en DTOs → `required`

### Buenas prácticas de código

- `async/await` en todas las operaciones I/O
- `ConfigureAwait(false)` en código de librería
- Cada método tiene una sola responsabilidad (SRP)
- Dependencias siempre inyectadas por constructor
- Sin lógica de negocio en ViewModels ni code-behind
- Clases que no serán heredadas: `sealed`

---

## Roadmap

### Alta prioridad 🔴

- [X] Migración a Code-First + SQLite
- [X] Columna discriminadora `type` en tabla `product`
- [X] Completar Views de productos (migración WPF → Avalonia)
- [ ] Módulo de ventas
- [ ] Validaciones adicionales (stock no negativo tras venta)
- [ ] Implementar `ObtenerProductoPorId` (Query + Handler + OnLoaded)

### Media prioridad 🟡

- [ ] Inicio de sesión con hasheo de contraseña
- [ ] APIs de catálogos (TCGdex, Scryfall, YGOPRODeck)
- [ ] Importación y exportación desde Excel
- [ ] Historial de cambios (audit log)
- [ ] Imágenes de productos
- [ ] Mejorar UI

### Baja prioridad 🟢

- [ ] Autenticación y roles
- [ ] Backup automático de BD
- [ ] Temas claro/oscuro
- [ ] Internacionalización (i18n)
- [ ] API REST para consumo externo

---

## Nota Final

El dominio es estable. Los detalles técnicos son reemplazables.

> La arquitectura no se diseñó para la versión actual — se diseñó para todas las versiones futuras.