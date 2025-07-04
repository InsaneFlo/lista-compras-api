﻿// <auto-generated />
using ListaComprasAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ListaComprasAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250531204303_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.36")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ListaComprasAPI.Models.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Comprado")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("ListaId")
                        .HasColumnType("int");

                    b.Property<string>("Nome")
                        .HasColumnType("longtext");

                    b.Property<int>("Quantidade")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ListaId");

                    b.ToTable("Itens");
                });

            modelBuilder.Entity("ListaComprasAPI.Models.Lista", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Nome")
                        .HasColumnType("longtext");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Listas");
                });

            modelBuilder.Entity("ListaComprasAPI.Models.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<string>("Nome")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("ListaComprasAPI.Models.Item", b =>
                {
                    b.HasOne("ListaComprasAPI.Models.Lista", "Lista")
                        .WithMany("Items")
                        .HasForeignKey("ListaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lista");
                });

            modelBuilder.Entity("ListaComprasAPI.Models.Lista", b =>
                {
                    b.HasOne("ListaComprasAPI.Models.Usuario", "Usuario")
                        .WithMany("Listas")
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("ListaComprasAPI.Models.Lista", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("ListaComprasAPI.Models.Usuario", b =>
                {
                    b.Navigation("Listas");
                });
#pragma warning restore 612, 618
        }
    }
}
