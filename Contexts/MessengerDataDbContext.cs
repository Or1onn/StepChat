﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StepChat.Models;

namespace StepChat.Contexts;

public partial class MessengerDataDbContext : DbContext
{
    public MessengerDataDbContext()
    {
    }

    public MessengerDataDbContext(DbContextOptions<MessengerDataDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<GroupsModel> Groups { get; set; }

    public virtual DbSet<ImagesModel> Images { get; set; }

    public virtual DbSet<TeachersModel> Teachers { get; set; }

    public virtual DbSet<UsersModel> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("workstation id=UsersDB.mssql.somee.com;packet size=4096;user id=Or1onn_SQLLogin_1;pwd=d8p1ng4ouc;data source=UsersDB.mssql.somee.com;persist security info=False;initial catalog=UsersDB; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroupsModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Groups__3214EC07214EBC37");

            entity.Property(e => e.Name).IsUnicode(false);
        });

        modelBuilder.Entity<ImagesModel>(entity =>
        {
            entity.HasKey(e => e.Image).HasName("PK__Images__7516F70CD8FA8BCA");

            entity.Property(e => e.Id).HasColumnName("Image");
        });

        modelBuilder.Entity<TeachersModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Teachers__3214EC073C7C8295");

            entity.Property(e => e.Email)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.Patronymic).IsUnicode(false);
            entity.Property(e => e.Surname).IsUnicode(false);
            entity.Property(e => e.TeachingGroups).IsUnicode(false);
        });

        modelBuilder.Entity<UsersModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07799A326E");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534B879DE27").IsUnique();

            entity.Property(e => e.BirthDate).HasColumnType("date");
            entity.Property(e => e.Email).HasMaxLength(60);
            entity.Property(e => e.PhoneNumber).HasMaxLength(13);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}