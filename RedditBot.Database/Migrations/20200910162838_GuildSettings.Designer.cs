﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UltimateRedditBot.Database;

namespace UltimateRedditBot.Database.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20200910162838_GuildSettings")]
    partial class GuildSettings
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("UltimateRedditBot.Domain.Models.Channel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("GuildId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("UltimateRedditBot.Domain.Models.Guild", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("GuildId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("UltimateRedditBot.Domain.Models.GuildSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("GuildId")
                        .HasColumnType("int");

                    b.Property<int>("MaxBulkAdds")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.ToTable("GuildSettings");
                });

            modelBuilder.Entity("UltimateRedditBot.Domain.Models.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Author")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Downs")
                        .HasColumnType("int");

                    b.Property<bool>("IsOver18")
                        .HasColumnType("bit");

                    b.Property<string>("PostId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PostType")
                        .HasColumnType("int");

                    b.Property<string>("Selftext")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SubRedditId")
                        .HasColumnType("int");

                    b.Property<string>("Thumbnail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Ups")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SubRedditId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("UltimateRedditBot.Domain.Models.SubReddit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsNsfw")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SubReddits");
                });

            modelBuilder.Entity("UltimateRedditBot.Domain.Models.SubRedditHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("GuildId")
                        .HasColumnType("int");

                    b.Property<int>("LastPostId")
                        .HasColumnType("int");

                    b.Property<int>("SubRedditId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.HasIndex("LastPostId");

                    b.HasIndex("SubRedditId");

                    b.ToTable("SubRedditHistories");
                });

            modelBuilder.Entity("UltimateRedditBot.Domain.Models.Channel", b =>
                {
                    b.HasOne("UltimateRedditBot.Domain.Models.Guild", "Guild")
                        .WithMany("Channels")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UltimateRedditBot.Domain.Models.GuildSettings", b =>
                {
                    b.HasOne("UltimateRedditBot.Domain.Models.Guild", "Guild")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UltimateRedditBot.Domain.Models.Post", b =>
                {
                    b.HasOne("UltimateRedditBot.Domain.Models.SubReddit", "SubReddit")
                        .WithMany("Posts")
                        .HasForeignKey("SubRedditId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UltimateRedditBot.Domain.Models.SubRedditHistory", b =>
                {
                    b.HasOne("UltimateRedditBot.Domain.Models.Guild", "Guild")
                        .WithMany("SubRedditHistories")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UltimateRedditBot.Domain.Models.Post", "LastPost")
                        .WithMany()
                        .HasForeignKey("LastPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UltimateRedditBot.Domain.Models.SubReddit", "SubReddit")
                        .WithMany("SubRedditHistories")
                        .HasForeignKey("SubRedditId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
