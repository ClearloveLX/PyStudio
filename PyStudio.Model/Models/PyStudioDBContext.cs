using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PyStudio.Model.Models
{
    public partial class PyStudioDBContext : DbContext
    {
        public virtual DbSet<InfoArea> InfoArea { get; set; }
        public virtual DbSet<InfoLogger> InfoLogger { get; set; }
        public virtual DbSet<InfoUser> InfoUser { get; set; }

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

            modelBuilder.Entity<InfoUser>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("Info_User");

                entity.Property(e => e.UserId).HasColumnName("User_Id");

                entity.Property(e => e.UserAddress)
                    .HasColumnName("User_Address")
                    .HasMaxLength(200);

                entity.Property(e => e.UserBirthday)
                    .HasColumnName("User_Birthday")
                    .HasColumnType("date");

                entity.Property(e => e.UserBlog)
                    .HasColumnName("User_Blog")
                    .HasMaxLength(200);

                entity.Property(e => e.UserCreateTime)
                    .HasColumnName("User_CreateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserEmail)
                    .HasColumnName("User_Email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserHeadPhoto)
                    .HasColumnName("User_HeadPhoto")
                    .HasMaxLength(50);

                entity.Property(e => e.UserIntroduce)
                    .HasColumnName("User_Introduce")
                    .HasMaxLength(200);

                entity.Property(e => e.UserIps)
                    .HasColumnName("User_Ips")
                    .HasMaxLength(50);

                entity.Property(e => e.UserLoginTime)
                    .HasColumnName("User_LoginTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("User_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.UserNickName)
                    .HasColumnName("User_NickName")
                    .HasMaxLength(50);

                entity.Property(e => e.UserPwd)
                    .IsRequired()
                    .HasColumnName("User_Pwd")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserSex)
                    .HasColumnName("User_Sex")
                    .HasDefaultValueSql("((2))");

                entity.Property(e => e.UserStatus)
                    .HasColumnName("User_Status")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.UserTel)
                    .HasColumnName("User_Tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
        }
    }
}
