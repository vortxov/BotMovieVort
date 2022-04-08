﻿// <auto-generated />
using System;
using BotMovieVort.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BotMovieVort.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220315081319_v1")]
    partial class v1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BotMovieVort.Domain.Entity.Genre", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ItemFilmId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ItemSerialsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ItemFilmId");

                    b.HasIndex("ItemSerialsId");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("BotMovieVort.Domain.Entity.ItemFilm", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RatingKP")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ItemFilms");
                });

            modelBuilder.Entity("BotMovieVort.Domain.Entity.ItemSerials", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RatingKP")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ItemSerials");
                });

            modelBuilder.Entity("BotMovieVort.Domain.Entity.Season", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ItemSerialsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ItemSerialsId");

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("BotMovieVort.Domain.Entity.Series", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberSeries")
                        .HasColumnType("int");

                    b.Property<Guid>("SeasonId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SeasonId");

                    b.ToTable("Series");
                });

            modelBuilder.Entity("BotMovieVort.Domain.Entity.Genre", b =>
                {
                    b.HasOne("BotMovieVort.Domain.Entity.ItemFilm", null)
                        .WithMany("Genres")
                        .HasForeignKey("ItemFilmId");

                    b.HasOne("BotMovieVort.Domain.Entity.ItemSerials", null)
                        .WithMany("Genres")
                        .HasForeignKey("ItemSerialsId");
                });

            modelBuilder.Entity("BotMovieVort.Domain.Entity.Season", b =>
                {
                    b.HasOne("BotMovieVort.Domain.Entity.ItemSerials", null)
                        .WithMany("Seasons")
                        .HasForeignKey("ItemSerialsId");
                });

            modelBuilder.Entity("BotMovieVort.Domain.Entity.Series", b =>
                {
                    b.HasOne("BotMovieVort.Domain.Entity.Season", "Season")
                        .WithMany("Series")
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Season");
                });

            modelBuilder.Entity("BotMovieVort.Domain.Entity.ItemFilm", b =>
                {
                    b.Navigation("Genres");
                });

            modelBuilder.Entity("BotMovieVort.Domain.Entity.ItemSerials", b =>
                {
                    b.Navigation("Genres");

                    b.Navigation("Seasons");
                });

            modelBuilder.Entity("BotMovieVort.Domain.Entity.Season", b =>
                {
                    b.Navigation("Series");
                });
#pragma warning restore 612, 618
        }
    }
}
