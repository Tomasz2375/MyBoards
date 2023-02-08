using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBoards.Migrations
{
    /// <inheritdoc />
    public partial class ViewModelAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW View_TopAuthors AS
            SELECT top 5 u.FullName, COUNT(*) as [WorkItemsCreated] 
            from Users u
            JOIN WorkItems wi ON wi.AuthorId = u.Id
            GROUP BY u.Id, u.FullName
            order by WorkItemsCreated desc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            DROP VIEW View_TopAuthors
            ");
        }
    }
}
