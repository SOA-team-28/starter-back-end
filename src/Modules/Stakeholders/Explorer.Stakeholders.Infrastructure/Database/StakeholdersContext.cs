﻿using Explorer.Stakeholders.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<ClubInvitation> ClubInvitations { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<UserClub> UserClubs { get; set; }
    public DbSet<ClubRequest> Requests { get; set; }
    public DbSet<ApplicationGrade> ApplicationGrades { get; set; }

    public DbSet<Message> Messages { get; set; }

    public DbSet<SocialProfile> SocialProfiles { get; set; }

    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();


        modelBuilder.Entity<UserClub>()
	        .HasKey(uc => new { uc.UserId, uc.ClubId });

		modelBuilder.Entity<UserClub>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(uc => uc.UserId);

        modelBuilder.Entity<UserClub>()
            .HasOne<Club>()
            .WithMany()
            .HasForeignKey(uc => uc.ClubId);




        //One-to-one relationship User-SocialProfile
        modelBuilder.Entity<User>()
            .HasOne<SocialProfile>()
            .WithOne();

        modelBuilder.Entity<SocialProfile>().Ignore(sp => sp.Followers);
        modelBuilder.Entity<SocialProfile>().Ignore(sp => sp.Followable);

        modelBuilder.Entity<SocialProfile>()
            .HasMany(sc => sc.Followed)
            .WithMany()
            .UsingEntity(j =>
            {
                j.ToTable("UserFollowers");
            });

        /*
        modelBuilder.Entity<Message>()
            .HasOne<User>()
            .WithMany(u => u.Inbox)
            .HasForeignKey(m => m.RecipientId);
        */

        /*
        modelBuilder.Entity<Message>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.RecipientId);
        */



        ConfigureStakeholder(modelBuilder);
    }

    private static void ConfigureStakeholder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);
    }
}
