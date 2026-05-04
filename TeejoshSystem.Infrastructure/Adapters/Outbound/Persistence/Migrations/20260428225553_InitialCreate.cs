using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "funko_special_feature",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_funko_special_feature", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "funko_subtype",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_funko_subtype", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hot_wheels_category",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hot_wheels_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    type = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    units = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sale",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    total = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tcg_expansion",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FranquiciaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tcg_expansion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tcg_franchise",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tcg_franchise", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tcg_pack",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FranquiciaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tcg_pack", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "funko",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "INTEGER", nullable: false),
                    box_number = table.Column<int>(type: "INTEGER", nullable: false),
                    license = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    subtype = table.Column<int>(type: "INTEGER", nullable: false),
                    special_caracteristic = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_funko", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_funko_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "hot_wheels",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "INTEGER", nullable: false),
                    model = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    year = table.Column<int>(type: "INTEGER", nullable: false),
                    serie = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    category_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hot_wheels", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_hot_wheels_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tcg",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "INTEGER", nullable: false),
                    pack_id = table.Column<int>(type: "INTEGER", nullable: false),
                    expansion_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tcg", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_tcg_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "toy",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "INTEGER", nullable: false),
                    min_years_old = table.Column<int>(type: "INTEGER", nullable: false),
                    min_players = table.Column<int>(type: "INTEGER", nullable: false),
                    max_players = table.Column<int>(type: "INTEGER", nullable: false),
                    is_board_game = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_toy", x => x.product_id);
                    table.CheckConstraint("check_players", "max_players >= min_players");
                    table.ForeignKey(
                        name: "FK_toy_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "varios",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "INTEGER", nullable: false),
                    brand = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    height = table.Column<decimal>(type: "TEXT", nullable: false),
                    width = table.Column<decimal>(type: "TEXT", nullable: false),
                    length = table.Column<decimal>(type: "TEXT", nullable: false),
                    material = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ilustration = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_varios", x => x.product_id);
                    table.CheckConstraint("check_dimensions", "height > 0 AND width > 0 AND (length IS NULL OR length > 0)");
                    table.ForeignKey(
                        name: "FK_varios_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sale_detail",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    sale_id = table.Column<int>(type: "INTEGER", nullable: false),
                    product_id = table.Column<int>(type: "INTEGER", nullable: false),
                    product_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale_detail", x => x.id);
                    table.ForeignKey(
                        name: "FK_sale_detail_sale_sale_id",
                        column: x => x.sale_id,
                        principalTable: "sale",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "funko_special_feature",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Chase" },
                    { 2, "Glow in the Dark" },
                    { 3, "Flocked" },
                    { 4, "Metallic" },
                    { 5, "Diamond / Glitter" },
                    { 6, "Black Light" },
                    { 7, "Chrome" },
                    { 8, "Translucent" },
                    { 9, "Exclusivo" }
                });

            migrationBuilder.InsertData(
                table: "funko_subtype",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Pop! Vinyl" },
                    { 2, "Pop! Deluxe" },
                    { 3, "Pop! Super" },
                    { 4, "Pop! Mega" },
                    { 5, "Pop! Rides" },
                    { 6, "Pop! Moments" },
                    { 7, "Pop! Albums" },
                    { 8, "Bitty Pop!" },
                    { 9, "Funko Soda" },
                    { 10, "Mystery Minis" }
                });

            migrationBuilder.InsertData(
                table: "hot_wheels_category",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Basic Car" },
                    { 2, "Treasure Hunt" },
                    { 3, "Super Treasure Hunt" },
                    { 4, "Car Culture" },
                    { 5, "Premium" },
                    { 6, "Boulevard" },
                    { 7, "Pop Culture" },
                    { 8, "Team Transport" },
                    { 9, "Mystery Models" },
                    { 10, "HWC / RLC" }
                });

            migrationBuilder.InsertData(
                table: "tcg_expansion",
                columns: new[] { "id", "FranquiciaId", "name" },
                values: new object[,]
                {
                    { 1, 1, "Escarlata y Púrpura Base" },
                    { 2, 1, "151" },
                    { 3, 1, "Obsidiana Llameante" },
                    { 4, 1, "Destinos de Paldea" },
                    { 5, 1, "Fuerza Temporal" },
                    { 6, 2, "Legendary Collection" },
                    { 7, 2, "Age of Overlord" },
                    { 8, 2, "Phantom Nightmare" },
                    { 9, 3, "Wilds of Eldraine" },
                    { 10, 3, "The Lost Caverns of Ixalan" },
                    { 11, 3, "Murders at Karlov Manor" },
                    { 12, 4, "Romance Dawn" },
                    { 13, 4, "Paramount War" },
                    { 14, 4, "Pillars of Strength" },
                    { 15, 4, "Kingdoms of Intrigue" },
                    { 16, 5, "Serie Base" }
                });

            migrationBuilder.InsertData(
                table: "tcg_franchise",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Pokémon" },
                    { 2, "Yu-Gi-Oh!" },
                    { 3, "Magic: The Gathering" },
                    { 4, "One Piece" },
                    { 5, "Bluey" }
                });

            migrationBuilder.InsertData(
                table: "tcg_pack",
                columns: new[] { "id", "FranquiciaId", "name" },
                values: new object[,]
                {
                    { 1, 1, "Sobre Individual" },
                    { 2, 1, "Blister 3 Sobres" },
                    { 3, 1, "Elite Trainer Box" },
                    { 4, 1, "Caja de 36 Sobres" },
                    { 5, 1, "Colección Premium" },
                    { 6, 2, "Sobre Individual" },
                    { 7, 2, "Caja de 24 Sobres" },
                    { 8, 2, "Structure Deck" },
                    { 9, 3, "Draft Booster" },
                    { 10, 3, "Set Booster" },
                    { 11, 3, "Collector Booster" },
                    { 12, 3, "Bundle" },
                    { 13, 4, "Sobre Individual" },
                    { 14, 4, "Caja de 24 Sobres" },
                    { 15, 4, "Starter Deck" },
                    { 16, 5, "Sobre Individual" },
                    { 17, 5, "Starter Pack" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_funko_special_feature_name",
                table: "funko_special_feature",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_funko_subtype_name",
                table: "funko_subtype",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_hot_wheels_category_name",
                table: "hot_wheels_category",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sale_detail_sale_id",
                table: "sale_detail",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "IX_tcg_expansion_name_FranquiciaId",
                table: "tcg_expansion",
                columns: new[] { "name", "FranquiciaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tcg_franchise_name",
                table: "tcg_franchise",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tcg_pack_name_FranquiciaId",
                table: "tcg_pack",
                columns: new[] { "name", "FranquiciaId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "funko");

            migrationBuilder.DropTable(
                name: "funko_special_feature");

            migrationBuilder.DropTable(
                name: "funko_subtype");

            migrationBuilder.DropTable(
                name: "hot_wheels");

            migrationBuilder.DropTable(
                name: "hot_wheels_category");

            migrationBuilder.DropTable(
                name: "sale_detail");

            migrationBuilder.DropTable(
                name: "tcg");

            migrationBuilder.DropTable(
                name: "tcg_expansion");

            migrationBuilder.DropTable(
                name: "tcg_franchise");

            migrationBuilder.DropTable(
                name: "tcg_pack");

            migrationBuilder.DropTable(
                name: "toy");

            migrationBuilder.DropTable(
                name: "varios");

            migrationBuilder.DropTable(
                name: "sale");

            migrationBuilder.DropTable(
                name: "product");
        }
    }
}
