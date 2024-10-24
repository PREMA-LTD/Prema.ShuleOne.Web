using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Emit;
using Prema.ShuleOne.Web.Backend.Database.LocationData;
using Prema.ShuleOne.Web.Server.Models.Location;
using Prema.ShuleOne.Web.Server.Models;


namespace Prema.ShuleOne.Web.Backend.Database
{
    public partial class ShuleOneDatabaseContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ShuleOneDatabaseContext(DbContextOptions<ShuleOneDatabaseContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            this.OnModelBuilding(builder);

            //builder.Entity<Gender>().HasData(
            //    new Gender { id = 1, name = "Male" },
            //    new Gender { id = 2, name = "Female" }
            //);

            var csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "LocationData", "kenya-location-data.csv");
            var records = LoadLocationData.LoadCsvData(csvFilePath);

            var counties = records.Select(r => r.Item1).DistinctBy(c => c.id).ToList();
            var subcounties = records.Select(r => r.Item2).DistinctBy(c => c.id).ToList();
            var wards = records.Select(r => r.Item3).DistinctBy(w => w.id).ToList();

            builder.Entity<County>().HasData(counties);
            builder.Entity<Subcounty>().HasData(subcounties);
            builder.Entity<Ward>().HasData(wards);

        }

        public DbSet<County> County { get; set; }
        public DbSet<Subcounty> Subcounty { get; set; }
        public DbSet<Ward> Ward { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmployeeBankDetails> EmployeeBankDetails { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<StudentContact> StudentContact { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
    }

}