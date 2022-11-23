using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Posy.Concurrency.Models;
using System;

namespace Posy.Concurrency.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");

            modelBuilder.Entity<Student>().Property(b => b.ID).UseSerialColumn();

            //modelBuilder.Entity<Department>().UseXminAsConcurrencyToken();
            var converter = new ValueConverter<byte[], long>(
                v => BitConverter.ToInt64(v, 0),
                v => BitConverter.GetBytes(v));

            modelBuilder.Entity<Department>()
                    .Property(_ => _.RowVersion)
                    .HasColumnName("xmin")
                    .HasColumnType("xid")
                    .HasConversion(converter);
        }
    }
}
