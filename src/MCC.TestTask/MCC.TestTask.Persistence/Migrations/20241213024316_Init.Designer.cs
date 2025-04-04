﻿// <auto-generated />
using System;
using MCC.TestTask.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MCC.TestTask.Persistance.Migrations
{
    [DbContext(typeof(BlogDbContext))]
    [Migration("20241213024316_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CommunityUser", b =>
                {
                    b.Property<Guid>("SubscribedToId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SubscribersId")
                        .HasColumnType("uuid");

                    b.HasKey("SubscribedToId", "SubscribersId");

                    b.HasIndex("SubscribersId");

                    b.ToTable("CommunityUser");
                });

            modelBuilder.Entity("MCC.TestTask.Domain.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsMarkedAsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("ParentId");

                    b.HasIndex("PostId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("MCC.TestTask.Domain.Community", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("CommunityType")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Communities");
                });

            modelBuilder.Entity("MCC.TestTask.Domain.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CommunityId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<int>("ReadingTime")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CommunityId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("MCC.TestTask.Domain.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("PostId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("MCC.TestTask.Domain.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateOnly?>("BirthDate")
                        .HasColumnType("date");

                    b.Property<Guid?>("CommunityId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Gender")
                        .HasColumnType("integer");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<Guid?>("PostId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.HasIndex("PostId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CommunityUser", b =>
                {
                    b.HasOne("MCC.TestTask.Domain.Community", null)
                        .WithMany()
                        .HasForeignKey("SubscribedToId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MCC.TestTask.Domain.User", null)
                        .WithMany()
                        .HasForeignKey("SubscribersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MCC.TestTask.Domain.Comment", b =>
                {
                    b.HasOne("MCC.TestTask.Domain.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MCC.TestTask.Domain.Comment", "Parent")
                        .WithMany("Replies")
                        .HasForeignKey("ParentId");

                    b.HasOne("MCC.TestTask.Domain.Post", null)
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("MCC.TestTask.Domain.Community", b =>
                {
                    b.HasOne("MCC.TestTask.Domain.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("MCC.TestTask.Domain.Post", b =>
                {
                    b.HasOne("MCC.TestTask.Domain.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MCC.TestTask.Domain.Community", "Community")
                        .WithMany("Posts")
                        .HasForeignKey("CommunityId");

                    b.Navigation("Author");

                    b.Navigation("Community");
                });

            modelBuilder.Entity("MCC.TestTask.Domain.Tag", b =>
                {
                    b.HasOne("MCC.TestTask.Domain.Post", null)
                        .WithMany("Tags")
                        .HasForeignKey("PostId");
                });

            modelBuilder.Entity("MCC.TestTask.Domain.User", b =>
                {
                    b.HasOne("MCC.TestTask.Domain.Community", null)
                        .WithMany("Administrators")
                        .HasForeignKey("CommunityId");

                    b.HasOne("MCC.TestTask.Domain.Post", null)
                        .WithMany("LikedBy")
                        .HasForeignKey("PostId");
                });

            modelBuilder.Entity("MCC.TestTask.Domain.Comment", b =>
                {
                    b.Navigation("Replies");
                });

            modelBuilder.Entity("MCC.TestTask.Domain.Community", b =>
                {
                    b.Navigation("Administrators");

                    b.Navigation("Posts");
                });

            modelBuilder.Entity("MCC.TestTask.Domain.Post", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("LikedBy");

                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}
