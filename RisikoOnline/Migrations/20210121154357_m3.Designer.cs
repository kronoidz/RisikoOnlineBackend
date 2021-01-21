﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RisikoOnline;

namespace RisikoOnline.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20210121154357_m3")]
    partial class m3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("RisikoOnline.Entities.Invitation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("Accepted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ReceiverName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SenderName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ReceiverName");

                    b.HasIndex("SenderName");

                    b.ToTable("Invitation");
                });

            modelBuilder.Entity("RisikoOnline.Entities.Player", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Name");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("RisikoOnline.Entities.Invitation", b =>
                {
                    b.HasOne("RisikoOnline.Entities.Player", "Receiver")
                        .WithMany("IncomingInvitations")
                        .HasForeignKey("ReceiverName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RisikoOnline.Entities.Player", "Sender")
                        .WithMany("OutgoingInvitations")
                        .HasForeignKey("SenderName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("RisikoOnline.Entities.Player", b =>
                {
                    b.Navigation("IncomingInvitations");

                    b.Navigation("OutgoingInvitations");
                });
#pragma warning restore 612, 618
        }
    }
}