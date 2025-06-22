using Agricon.Core.Model.Entities;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using System.Security.Cryptography.Xml;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Agricon.Core.Model.Enums;

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

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Description)
                .HasConversion<string>(); 

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Status)
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

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.BookingId).HasColumnName("bookingId");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.PaymentMethod).HasColumnName("paymentMethod");
                entity.Property(e => e.Amount).HasColumnName("amount");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
                entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");
                entity.Property(e => e.Reference).HasColumnName("ref");
            });
        }


    }
}
