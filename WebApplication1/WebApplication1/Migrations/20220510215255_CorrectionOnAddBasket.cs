using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class CorrectionOnAddBasket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "Baskets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "Baskets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_ColorId",
                table: "Baskets",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_SizeId",
                table: "Baskets",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_Colors_ColorId",
                table: "Baskets",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_Sizes_SizeId",
                table: "Baskets",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_Colors_ColorId",
                table: "Baskets");

            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_Sizes_SizeId",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_ColorId",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_SizeId",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "Baskets");
        }
    }
}
