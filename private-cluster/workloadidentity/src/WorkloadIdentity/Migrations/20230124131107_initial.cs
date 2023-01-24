using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkloadIdentity.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Customer",
                columns: new[] { "Id", "Company", "Created", "Level", "SourceId", "Updated" },
                values: new object[,]
                {
                    { "5c3ac12f-ec83-449e-a37e-de7442cde7da", "Amazon", new DateTime(2023, 1, 15, 14, 11, 6, 986, DateTimeKind.Local).AddTicks(5564), "100", 1004, new DateTime(2023, 1, 24, 14, 11, 6, 986, DateTimeKind.Local).AddTicks(5566) },
                    { "991e0c2f-a768-40b9-9eaa-b7c31eb3fcc4", "Microsoft", new DateTime(2023, 1, 21, 14, 11, 6, 986, DateTimeKind.Local).AddTicks(5520), "100", 1001, new DateTime(2023, 1, 24, 14, 11, 6, 986, DateTimeKind.Local).AddTicks(5556) },
                    { "b84178a0-b0ff-4721-96cc-5d271d93f6b9", "Google", new DateTime(2023, 1, 19, 14, 11, 6, 986, DateTimeKind.Local).AddTicks(5558), "100", 1002, new DateTime(2023, 1, 24, 14, 11, 6, 986, DateTimeKind.Local).AddTicks(5559) },
                    { "fbf6dc01-93f9-4772-891f-46e5a79d6e2a", "Apple", new DateTime(2023, 1, 17, 14, 11, 6, 986, DateTimeKind.Local).AddTicks(5561), "100", 1003, new DateTime(2023, 1, 24, 14, 11, 6, 986, DateTimeKind.Local).AddTicks(5562) }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
