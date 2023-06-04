﻿// <auto-generated />
using System;
using DualBuffer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DualBuffer.Migrations
{
    [DbContext(typeof(NetworkDbContext))]
    partial class NetworkDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("DualBuffer.Models.Enums.Call", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsBlocked")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("NumResourceBlocks")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeArrived")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("WaitingTime")
                        .HasColumnType("int");

                    b.Property<int>("allocatedChannels")
                        .HasColumnType("int");

                    b.Property<double>("callDuration")
                        .HasColumnType("double");

                    b.Property<double>("requiredBandwidth")
                        .HasColumnType("double");

                    b.Property<double>("signalToNoiseRatio")
                        .HasColumnType("double");

                    b.Property<int>("totalChannels")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("calls");
                });
#pragma warning restore 612, 618
        }
    }
}
