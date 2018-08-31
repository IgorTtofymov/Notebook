using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Notebook.Models
{
    public class PersonDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public PersonDbContext() : base("PersonContext")
        {

        }
    }
}