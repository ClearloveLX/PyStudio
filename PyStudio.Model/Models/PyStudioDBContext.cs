using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PyStudio.Model.Models
{
    public partial class PyStudioDBContext : DbContext
    {
        public virtual DbSet<InfoArea> InfoArea { get; set; }
        public virtual DbSet<InfoLogger> InfoLogger { get; set; }

        public PyStudioDBContext(DbContextOptions<PyStudioDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InfoArea>(entity =>
            {
                entity.HasKey(e => e.AreaId);

                entity.ToTable("Info_Area");

                entity.Property(e => e.AreaId).ValueGeneratedNever();

                entity.Property(e => e.AreaCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AreaCoord)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AreaLevel).HasDefaultValueSql("((0))");

                entity.Property(e => e.AreaName).HasMaxLength(50);

                entity.Property(e => e.AreaNote).HasMaxLength(150);

                entity.Property(e => e.AreaPathId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AreaPid)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AreaZipCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<InfoLogger>(entity =>
            {
                entity.HasKey(e => e.LoggerId);

                entity.ToTable("Info_Logger");

                entity.Property(e => e.LoggerCreateTime).HasColumnType("datetime");

                entity.Property(e => e.LoggerDescription).HasColumnType("ntext");
            });
        }
    }
}
