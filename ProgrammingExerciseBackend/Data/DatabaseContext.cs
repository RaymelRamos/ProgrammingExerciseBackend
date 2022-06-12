using Microsoft.EntityFrameworkCore;
using ProgrammingExercise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ProgrammingExerciseBackend.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
    : base(options)
        { }
        public DbSet<Contact> Contact { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>().ToTable("Contact");
            modelBuilder.Entity<Contact>(x =>
           {
               x.HasKey(e => e.Id);
           });
            base.OnModelCreating(modelBuilder);
        }
    }
}
