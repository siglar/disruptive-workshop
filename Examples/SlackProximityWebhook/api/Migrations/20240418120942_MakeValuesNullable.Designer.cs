﻿// <auto-generated />
using System;
using Exercise5;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Exercise5.Migrations
{
    [DbContext(typeof(SensorContext))]
    [Migration("20240418120942_MakeValuesNullable")]
    partial class MakeValuesNullable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("Exercise5.Sensor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DeviceId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId")
                        .IsUnique();

                    b.ToTable("Sensors");
                });

            modelBuilder.Entity("Exercise5.SensorValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Proximity")
                        .HasColumnType("TEXT");

                    b.Property<int>("SensorId")
                        .HasColumnType("INTEGER");

                    b.Property<float?>("Temperature")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SensorId");

                    b.ToTable("SensorValues");
                });

            modelBuilder.Entity("Exercise5.SensorValue", b =>
                {
                    b.HasOne("Exercise5.Sensor", "Sensor")
                        .WithMany("Values")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sensor");
                });

            modelBuilder.Entity("Exercise5.Sensor", b =>
                {
                    b.Navigation("Values");
                });
#pragma warning restore 612, 618
        }
    }
}
