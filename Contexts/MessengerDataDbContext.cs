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

    public virtual DbSet<ChatUserModel> ChatUsers { get; set; }

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
            entity.HasKey(e => e.Id).HasName("PK__Chats__3214EC07D7910FD2");
        });

        modelBuilder.Entity<MessagesModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatMess__3214EC076285B573");
        });

        modelBuilder.Entity<ChatUserModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatUser__3214EC07C32704BA");
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

        modelBuilder.Entity<MessagesStatusModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Messages__3214EC07E2DEECFE");

            entity.ToTable("MessagesStatus");
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
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07C2FFB5C0");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105341CD7B234").IsUnique();

            entity.HasIndex(e => e.PrivateKeysStorageId, "UQ__Users__F2956E4B632E199B").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(60);
            entity.Property(e => e.PhoneNumber).HasMaxLength(13);
            entity.Property(e => e.PrivateKeysStorageId).HasColumnName("PrivateKeysStorageID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
