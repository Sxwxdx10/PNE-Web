using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNE_DataAccess
{
    public class ScaffoldMigrationContext : PneContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            DotNetEnv.Env.Load("../.env");
            var env = System.Environment.GetEnvironmentVariables();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=PNE;Username=pne_owner;Password=mJz7Re5jZVdl", x => x.UseNetTopologySuite());
        }
    }
}
