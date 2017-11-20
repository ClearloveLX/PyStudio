using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PyStudio.Model.Models.BaseInfo;
using PyStudio.Model.Models.Sys;
using PyStudio.Model.Models.Account;
using PyStudio.Model.Models.SmallApp;

namespace PyStudio.Model.Models
{
    public partial class PyStudioDBContext : DbContext
    {
        public virtual DbSet<InfoArea> InfoArea { get; set; }
        public virtual DbSet<InfoEi> InfoEi { get; set; }
        public virtual DbSet<SysLogger> SysLogger { get; set; }
        public virtual DbSet<InfoUser> InfoUser { get; set; }
        public virtual DbSet<InfoMessageBoard> InfoMessageBoard { get; set; }

        public PyStudioDBContext(DbContextOptions<PyStudioDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region BaseInfo

            #region InfoArea

            modelBuilder.Entity<InfoArea>(entity =>
            {
                entity.HasKey(e => e.AreaId);

                entity.Property(e => e.AreaId)
                    .ValueGeneratedNever();

                entity.Property(e => e.AreaCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AreaCoord)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AreaLevel)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.AreaName)
                    .HasMaxLength(50);

                entity.Property(e => e.AreaNote)
                    .HasMaxLength(150);

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

            #endregion

            #region InfoEi

            modelBuilder.Entity<InfoEi>(entity =>
            {
                entity.HasKey(e => e.Eicol1);

                entity.Property(e => e.Eicol1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Eicol2)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Eicol3)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Eicol4)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            #endregion

            #endregion

            #region Account

            #region InfoUser

            modelBuilder.Entity<InfoUser>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserAddress)
                    .HasMaxLength(200);

                entity.Property(e => e.UserBirthday)
                    .HasColumnType("date");

                entity.Property(e => e.UserBlog)
                    .HasMaxLength(200);

                entity.Property(e => e.UserCreateTime)
                    .HasColumnType("datetime");

                entity.Property(e => e.UserEmail)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserHeadPhoto)
                    .HasMaxLength(50);

                entity.Property(e => e.UserIntroduce)
                    .HasMaxLength(200);

                entity.Property(e => e.UserIps)
                    .HasMaxLength(50);

                entity.Property(e => e.UserLoginTime)
                    .HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserNickName)
                    .HasMaxLength(50);

                entity.Property(e => e.UserPwd)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserSex)
                    .HasDefaultValueSql("((2))");

                entity.Property(e => e.UserStatus)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.UserTel)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            #endregion

            #endregion

            #region Sys

            #region SysLogger

            modelBuilder.Entity<SysLogger>(entity =>
            {
                entity.HasKey(e => e.LoggerId);

                entity.Property(e => e.LoggerCreateTime)
                    .HasColumnType("datetime");

                entity.Property(e => e.LoggerDescription)
                    .HasColumnType("ntext");

                entity.Property(e => e.LoggerIps)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                      .WithMany(p => p.SysLogger)
                      .HasForeignKey(d => d.LoggerUser)
                      .HasConstraintName("FK_SysLogger_InfoUser");
            });

            #endregion

            #endregion

            #region SmallTools

            #region InfoMessageBoard

            modelBuilder.Entity<InfoMessageBoard>(entity =>
            {
                entity.HasKey(e => e.MessageBoardId);

                entity.Property(e => e.MessageBoardUser).HasMaxLength(100);

                entity.Property(e => e.MessageBoardIp).HasMaxLength(50).IsUnicode(false);

                entity.Property(e => e.MessageBoardCreateTime).HasColumnType("datetime");

                entity.Property(e => e.MessageBoardContent).HasColumnType("ntext");
            });

            #endregion

            #endregion
        }
    }
}
