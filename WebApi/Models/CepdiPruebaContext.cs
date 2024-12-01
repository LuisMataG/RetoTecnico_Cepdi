using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models;

public partial class CepdiPruebaContext : DbContext
{
    public CepdiPruebaContext()
    {
    }

    public CepdiPruebaContext(DbContextOptions<CepdiPruebaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Formasfarmaceutica> Formasfarmaceuticas { get; set; }

    public virtual DbSet<Medicamento> Medicamentos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Formasfarmaceutica>(entity =>
        {
            entity.HasKey(e => e.Idformafarmaceutica).HasName("PK_FormaFarmaceutica");

            entity.ToTable("formasfarmaceuticas");

            entity.Property(e => e.Idformafarmaceutica).HasColumnName("idformafarmaceutica");
            entity.Property(e => e.Habilitado).HasColumnName("habilitado");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Medicamento>(entity =>
        {
            entity.HasKey(e => e.Idmedicamento);

            entity.ToTable("medicamentos");

            entity.Property(e => e.Idmedicamento).HasColumnName("idmedicamento");
            entity.Property(e => e.Bhabilitado).HasColumnName("bhabilitado");
            entity.Property(e => e.Concentracion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("concentracion");
            entity.Property(e => e.Idformafarmaceutica).HasColumnName("idformafarmaceutica");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.Presentacion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("presentacion");
            entity.Property(e => e.Stock).HasColumnName("stock");

            entity.HasOne(d => d.IdformafarmaceuticaNavigation).WithMany(p => p.Medicamentos)
                .HasForeignKey(d => d.Idformafarmaceutica)
                .HasConstraintName("FK__medicamen__idfor__35BCFE0A");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Idusuario);

            entity.ToTable("usuarios");

            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
            entity.Property(e => e.Fechacreacion).HasColumnName("fechacreacion");
            entity.Property(e => e.Idperfil).HasColumnName("idperfil");
            entity.Property(e => e.Nombre)
                .HasMaxLength(200)
                .HasColumnName("nombre");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Usuario1)
                .HasMaxLength(100)
                .HasColumnName("usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
