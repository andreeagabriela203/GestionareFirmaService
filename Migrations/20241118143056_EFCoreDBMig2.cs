using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirmaService.Migrations
{
    /// <inheritdoc />
    public partial class EFCoreDBMig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Angajati",
                columns: table => new
                {
                    AngajatiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prenume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataNastere = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataAngajare = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salariu = table.Column<int>(type: "int", nullable: false),
                    CNP = table.Column<int>(type: "int", nullable: false),
                    NrTelefon = table.Column<int>(type: "int", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sex = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Angajati", x => x.AngajatiId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Angajati");
        }
    }
}
