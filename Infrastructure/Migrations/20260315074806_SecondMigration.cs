using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Top10AdvancedSaveLigandsCsvUrl",
                table: "Proteins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Top10AdvancedSaveLigandsImgUrl",
                table: "Proteins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Ligands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PdbUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SdfUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProteinId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ligands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ligands_Proteins_ProteinId",
                        column: x => x.ProteinId,
                        principalTable: "Proteins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ligands_ProteinId",
                table: "Ligands",
                column: "ProteinId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ligands");

            migrationBuilder.DropColumn(
                name: "Top10AdvancedSaveLigandsCsvUrl",
                table: "Proteins");

            migrationBuilder.DropColumn(
                name: "Top10AdvancedSaveLigandsImgUrl",
                table: "Proteins");
        }
    }
}
