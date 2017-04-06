using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using LetMeKnowApi.Data;
using LetMeKnowApi.Model;

namespace LetMeKnowApi.Migrations
{
    [DbContext(typeof(LetMeKnowContext))]
    partial class LetMeKnowContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("LetMeKnowApi.Model.Area", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(60);

                    b.HasKey("Id");

                    b.ToTable("Areas");
                });

            modelBuilder.Entity("LetMeKnowApi.Model.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("LetMeKnowApi.Model.Suggestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AreaId");

                    b.Property<string>("Content")
                        .IsRequired();

                    b.Property<int>("CreatorId");

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(new DateTime(2017, 4, 6, 16, 17, 9, 111, DateTimeKind.Local));

                    b.Property<DateTime>("DateUpdated")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(new DateTime(2017, 4, 6, 16, 17, 9, 118, DateTimeKind.Local));

                    b.Property<string>("Image")
                        .IsRequired();

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(60);

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.HasIndex("CreatorId");

                    b.HasIndex("Status")
                        .HasName("Status");

                    b.ToTable("Suggestion");
                });

            modelBuilder.Entity("LetMeKnowApi.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("PasswordHash")
                        .IsRequired();

                    b.Property<string>("Salt")
                        .IsRequired();

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .HasName("Email");

                    b.HasIndex("UserName")
                        .IsUnique()
                        .HasName("UserName");

                    b.ToTable("User");
                });

            modelBuilder.Entity("LetMeKnowApi.Model.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("RoleId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("LetMeKnowApi.Model.Suggestion", b =>
                {
                    b.HasOne("LetMeKnowApi.Model.Area", "Area")
                        .WithMany("Suggestions")
                        .HasForeignKey("AreaId");

                    b.HasOne("LetMeKnowApi.Model.User", "Creator")
                        .WithMany("SuggestionsCreated")
                        .HasForeignKey("CreatorId");
                });

            modelBuilder.Entity("LetMeKnowApi.Model.UserRole", b =>
                {
                    b.HasOne("LetMeKnowApi.Model.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");

                    b.HasOne("LetMeKnowApi.Model.User", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId");
                });
        }
    }
}
