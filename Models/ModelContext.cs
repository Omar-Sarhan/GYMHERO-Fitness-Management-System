using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Gym.Models;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<Plan> Plans { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Testimonial> Testimonials { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("DATA SOURCE=localhost:1521;USER ID=C##omarsarhan;PASSWORD=Test321;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("C##OMARSARHAN")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.Cardid).HasName("SYS_C008436");

            entity.ToTable("CARD");

            entity.Property(e => e.Cardid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("CARDID");
            entity.Property(e => e.Balance)
                .HasColumnType("NUMBER")
                .HasColumnName("BALANCE");
            entity.Property(e => e.Password)
                .HasColumnType("NUMBER")
                .HasColumnName("PASSWORD");
        });

        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.Planid).HasName("SYS_C008452");

            entity.ToTable("PLAN");

            entity.Property(e => e.Planid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("PLANID");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Planname)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PLANNAME");
            entity.Property(e => e.Price)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("PRICE");
            entity.Property(e => e.Trainerid)
                .HasColumnType("NUMBER")
                .HasColumnName("TRAINERID");

            entity.HasOne(d => d.Trainer).WithMany(p => p.Plans)
                .HasForeignKey(d => d.Trainerid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PLAN_TRAINER");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.Profileid).HasName("SYS_C008440");

            entity.ToTable("PROFILE");

            entity.Property(e => e.Profileid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("PROFILEID");
            entity.Property(e => e.Fname)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("FNAME");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("GENDER");
            entity.Property(e => e.Imagepath)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("IMAGEPATH");
            entity.Property(e => e.Lname)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("LNAME");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("SYS_C008432");

            entity.ToTable("ROLE");

            entity.Property(e => e.Roleid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ROLEID");
            entity.Property(e => e.Rolename)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ROLENAME");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Subscriptionid).HasName("SYS_C008457");

            entity.ToTable("SUBSCRIPTION");

            entity.Property(e => e.Subscriptionid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("SUBSCRIPTIONID");
            entity.Property(e => e.Fromdate)
                .HasColumnType("DATE")
                .HasColumnName("FROMDATE");
            entity.Property(e => e.Memberid)
                .HasColumnType("NUMBER")
                .HasColumnName("MEMBERID");
            entity.Property(e => e.Planid)
                .HasColumnType("NUMBER")
                .HasColumnName("PLANID");
            entity.Property(e => e.Todate)
                .HasColumnType("DATE")
                .HasColumnName("TODATE");

            entity.HasOne(d => d.Member).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.Memberid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SUBSCRIPTION_MEMBER");

            entity.HasOne(d => d.Plan).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.Planid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SUBSCRIPTION_PLAN");
        });

        modelBuilder.Entity<Testimonial>(entity =>
        {
            entity.HasKey(e => e.Testimonialid).HasName("SYS_C008462");

            entity.ToTable("TESTIMONIAL");

            entity.Property(e => e.Testimonialid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("TESTIMONIALID");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CONTENT");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.User).WithMany(p => p.Testimonials)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TESTIMONIAL_USER");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("SYS_C008444");

            entity.ToTable("USERS");

            entity.HasIndex(e => e.Email, "SYS_C008445").IsUnique();

            entity.Property(e => e.Userid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");
            entity.Property(e => e.Cardid)
                .HasColumnType("NUMBER")
                .HasColumnName("CARDID");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Profileid)
                .HasColumnType("NUMBER")
                .HasColumnName("PROFILEID");
            entity.Property(e => e.Roleid)
                .HasColumnType("NUMBER")
                .HasColumnName("ROLEID");

            entity.HasOne(d => d.Card).WithMany(p => p.Users)
                .HasForeignKey(d => d.Cardid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_USER_CARD");

            entity.HasOne(d => d.Profile).WithMany(p => p.Users)
                .HasForeignKey(d => d.Profileid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_USER_PROFILE");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_USER_ROLE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
