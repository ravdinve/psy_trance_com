using System.Data.Entity.Migrations;

namespace psy_trance_com.DataAccessLayer.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }
    }
}