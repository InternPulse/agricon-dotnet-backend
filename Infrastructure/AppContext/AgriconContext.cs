using Agricon.Core.Model.Entities;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using System.Security.Cryptography.Xml;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Agricon.Infrastructure.AppContext
{
    public class AgriconContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public AgriconContext(DbContextOptions<AgriconContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .Property(t => t.PaymentMethod)
                .HasConversion<string>();

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                    }
                }
            }
        }

    }
}
