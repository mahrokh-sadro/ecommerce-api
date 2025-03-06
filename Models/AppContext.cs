using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models;

public partial class AppContext : DbContext
{
    public AppContext(DbContextOptions<AppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("CartItem")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.ProductId).HasColumnType("int(11)");
            entity.Property(e => e.Quantity).HasColumnType("int(11)");
            entity.Property(e => e.ShoppingCartId).HasMaxLength(255);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("Product")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Brand).HasMaxLength(45);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Image).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.Price).HasPrecision(10);
            entity.Property(e => e.Type).HasMaxLength(45);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
