﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GeekCoding.Data.Models
{
    public partial class OnlineJudgeContext : DbContext
    {
        public virtual DbSet<UserInformation> UserInformation { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Server=DESKTOP-G6RG4NM;Database=OnlineJudge;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInformation>(entity =>
            {
                entity.HasKey(e => e.IdUser);

                entity.Property(e => e.IdUser).ValueGeneratedNever();

                entity.Property(e => e.Clasa).IsRequired();

                entity.Property(e => e.Nume).IsRequired();

                entity.Property(e => e.Prenume).IsRequired();

                entity.Property(e => e.Profesor).IsRequired();

                entity.Property(e => e.Scoala).IsRequired();

                entity.Property(e => e.Username).IsRequired();
            });
        }
    }
}
