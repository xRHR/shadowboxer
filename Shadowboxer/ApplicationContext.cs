using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Shadowboxer
{
    internal sealed class ApplicationContext : DbContext
    {
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Shadowbox> Shadowboxes { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string err_msg = "";
            IConfiguration app_cfg = new ConfigurationBuilder()
                .AddJsonFile("appconfig.json", optional: false, reloadOnChange: true)
                .Build();

            string? host = app_cfg["db:host"];
            string? name = app_cfg["db:name"];
            string? user = app_cfg["db:user"];
            string? pswd = app_cfg["db:pswd"];

            if (string.IsNullOrEmpty(host))
            {
                err_msg += "Database host is not set\n";
            }
            if (string.IsNullOrEmpty(name))
            {
                err_msg += "Database name is not set\n";
            }
            if (string.IsNullOrEmpty(user))
            {
                err_msg += "Database user is not set\n";
            }
            if (string.IsNullOrEmpty(pswd))
            {
                err_msg += "Database password is not set\n";
            }

            if (!string.IsNullOrEmpty(err_msg))
            {
                throw new Exception("Configuration error:\n" + err_msg);
            }

            string conn_str = $"Host={host};Database={name};Username={user};Password={pswd}";
            Console.WriteLine(conn_str);
            optionsBuilder.UseNpgsql(conn_str);
        }
    }
}
