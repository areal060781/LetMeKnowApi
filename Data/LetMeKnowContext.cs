using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetMeKnowApi.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LetMeKnowApi.Data
{
    public class LetMeKnowContext : DbContext
    {
        public DbSet<Suggestion> Suggestions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        
        public LetMeKnowContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }


            modelBuilder.Entity<Suggestion>()
                .ToTable("Suggestion");

            modelBuilder.Entity<Suggestion>()
                .Property(s => s.CreatorId)
                .IsRequired();

            modelBuilder.Entity<Suggestion>()
                .Property(u => u.Title)
                .HasMaxLength(60)
                .IsRequired(); 

            modelBuilder.Entity<Suggestion>()
                .Property(u => u.Content)                
                .IsRequired(); 

            modelBuilder.Entity<Suggestion>()
                .Property(u => u.Image)                
                .IsRequired(); 

            modelBuilder.Entity<Suggestion>()
                .Property(s => s.DateCreated)
                .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<Suggestion>()
                .Property(s => s.DateUpdated)
                .HasDefaultValue(DateTime.Now);            

            modelBuilder.Entity<Suggestion>()
                .Property(s => s.Status)
                .HasDefaultValue(SuggestionStatus.Nuevo);

            modelBuilder.Entity<Suggestion>()
                .HasOne(s => s.Creator)
                .WithMany(c => c.SuggestionsCreated)
                .HasForeignKey(s => s.CreatorId);
            
            modelBuilder.Entity<Suggestion>()
                .HasOne(s => s.Area)
                .WithMany(c => c.Suggestions)
                .HasForeignKey(s => s.AreaId);

            modelBuilder.Entity<Suggestion>()
                .HasIndex("Status")
                .HasName("Status"); 
                
            /**/

            modelBuilder.Entity<User>()
              .ToTable("User");

            modelBuilder.Entity<User>()
                .Property(u => u.UserName)
                .HasMaxLength(20)
                .IsRequired();                

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)                
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Email)                
                .IsRequired();                

            modelBuilder.Entity<User>()
                .Property(u => u.Salt)                
                .IsRequired();  

            modelBuilder.Entity<User>()
                .HasIndex("UserName")
                .IsUnique()
                .HasName("UserName");    

            modelBuilder.Entity<User>()
                .HasIndex("Email")
                .HasName("Email");  

            /**/

            modelBuilder.Entity<Role>()
                .Property(r => r.Name) 
                .HasMaxLength(40)               
                .IsRequired();            

            /**/

            modelBuilder.Entity<Area>()
                .Property(u => u.Name) 
                .HasMaxLength(60)               
                .IsRequired();
            
            /**/
        
            modelBuilder.Entity<UserRole>()
              .ToTable("UserRole");

            modelBuilder.Entity<UserRole>()
                .HasOne(x => x.User)
                .WithMany(u => u.Roles)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(x => x.Role)
                .WithMany(u => u.Users)
                .HasForeignKey(x => x.RoleId);

        }
    }
}
