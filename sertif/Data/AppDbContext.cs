using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using sertif.Models;

namespace sertif.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Student> Students { get; set; }
		public DbSet<Laboran> Laborans { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<Loan> Loans { get; set; }
		public DbSet<Class> Classs { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

		}
	}
}
