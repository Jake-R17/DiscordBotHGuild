﻿// <auto-generated />
using System;
using DiscordBotHGuild.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiscordBotHGuild.Migrations
{
    [DbContext(typeof(SqliteContext))]
    [Migration("20210227235004_UpdateMutedDB")]
    partial class UpdateMutedDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.3");

            modelBuilder.Entity("DiscordBotHGuild.Models.MutedUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("GuildId")
                        .HasColumnType("TEXT");

                    b.Property<string>("MemberId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("MutedExpiration")
                        .HasColumnType("TEXT");

                    b.Property<string>("MutedReason")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("MutedUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
