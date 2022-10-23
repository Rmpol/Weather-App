using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using itProgerWPF.Models;
using Microsoft.EntityFrameworkCore;

namespace itProgerWPF
{
    internal class AppDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }

        public AppDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseMySql("server=localhost;port=3306;username=root;password=root;database=wpfapp");
        }

    }
}
