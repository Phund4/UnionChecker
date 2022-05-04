using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionCheckers
{
    public class DB:DbContext
    {
        public DbSet<Users> Users { get; set; }

        public DB()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("Server=mysql60.hostland.ru;Database=host1323541_pd1;Uid=host1323541_itstep;Pwd=269f43dc");
        }
    }
}

