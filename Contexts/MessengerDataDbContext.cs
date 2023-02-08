using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StepChat.Classes.Configuration;
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

    public virtual DbSet<ChatsModel> Chats { get; set; }

    public virtual DbSet<ChatsStorageModel> ChatsStorages { get; set; }

    public virtual DbSet<GroupsModel> Groups { get; set; }

    public virtual DbSet<ImagesModel> Images { get; set; }

    public virtual DbSet<KeysModel> Keys { get; set; }

    public virtual DbSet<MessagesModel> Messages { get; set; }

    public virtual DbSet<PrivateKeysStorageModel> PrivateKeysStorages { get; set; }

    public virtual DbSet<TeachersModel> Teachers { get; set; }

    public virtual DbSet<UsersModel> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json");
        var config = builder.Build();
        string? connectionString = config.GetConnectionString("DefaultConnection");

        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatsModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Chats__3214EC07AF265FD5");

            entity.HasIndex(e => e.UserId, "UQ__Chats__1788CC4D5E161CC9").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Chat)
                .HasPrincipalKey<ChatsStorageModel>(p => p.ChatId)
                .HasForeignKey<ChatsModel>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Chats__Id__245D67DE");
        });

        modelBuilder.Entity<ChatsStorageModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatsSto__3214EC074F7BC46A");

            entity.ToTable("ChatsStorage");

            entity.HasIndex(e => e.ChatId, "UQ__ChatsSto__A9FBE6276AC395BB").IsUnique();

            entity.Property(e => e.ChatId).HasColumnName("ChatID");
        });

        modelBuilder.Entity<GroupsModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Groups__3214EC07214EBC37");

            entity.Property(e => e.Name).IsUnicode(false);
        });

        modelBuilder.Entity<ImagesModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Images__3214EC072DFE3BC7");

            entity.Property(e => e.Image).HasColumnName("Image");
        });

        modelBuilder.Entity<KeysModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Keys__3214EC07E98376CB");

            entity.Property(e => e.Email).IsUnicode(false);
            entity.Property(e => e.Key)
                .IsUnicode(false)
                .HasColumnName("Key");
        });

        modelBuilder.Entity<MessagesModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Messages__3214EC07060349D9");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.User).IsUnicode(false);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Message)
                .HasPrincipalKey<ChatsModel>(p => p.UserId)
                .HasForeignKey<MessagesModel>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__Id__2739D489");
        });

        modelBuilder.Entity<PrivateKeysStorageModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PrivateK__3214EC07FDF9FB1E");

            entity.ToTable("PrivateKeysStorage");

            entity.HasIndex(e => e.KeysId, "UQ__PrivateK__02EEC771CF004CA5").IsUnique();
        });

        modelBuilder.Entity<TeachersModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Teachers__3214EC07BDF4B683");

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
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07F4E2FBA0");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105349372D545").IsUnique();

            entity.HasIndex(e => e.PrivateKeysStorageId, "UQ__Users__F2956E4BE9FC4FF3").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(60);
            entity.Property(e => e.PhoneNumber).HasMaxLength(13);
            entity.Property(e => e.PrivateKeysStorageId).HasColumnName("PrivateKeysStorageID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
