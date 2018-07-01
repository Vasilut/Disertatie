using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GeekCoding.Data.Models
{
    public partial class EvaluatorContext : IdentityDbContext<User>
    {
        public virtual DbSet<Problem> Problem { get; set; }
        public virtual DbSet<ProgresStatus> ProgresStatus { get; set; }
        public virtual DbSet<Solution> Solution { get; set; }
        public virtual DbSet<Submision> Submision { get; set; }
        public virtual DbSet<Evaluation> Evaluation { get; set; }
        public virtual DbSet<Tests> Tests { get; set; }

        public EvaluatorContext(DbContextOptions options) : base(options)
        {

        }

        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //Scaffold-DbContext "Server=DESKTOP-8R8BLMM;Database=OnlineJudge;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force -Tables mama
        //            if (!optionsBuilder.IsConfigured)
        //            {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //                optionsBuilder.UseSqlServer(@"Server=DESKTOP-8R8BLMM;Database=OnlineJudge;Trusted_Connection=True;");
        //            }
        //        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Problem>(entity =>
            {
                entity.Property(e => e.ProblemId).ValueGeneratedNever();

                entity.Property(e => e.Dificulty)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MemoryLimit).HasMaxLength(50);

                entity.Property(e => e.ProblemContent).IsRequired();

                entity.Property(e => e.ProblemName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TimeLimit).HasMaxLength(50);
            });

            modelBuilder.Entity<ProgresStatus>(entity =>
            {
                entity.Property(e => e.ProgresStatusId).ValueGeneratedNever();

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Problem)
                    .WithMany(p => p.ProgresStatus)
                    .HasForeignKey(d => d.ProblemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProgresSt__Probl__2B3F6F97");
            });

            modelBuilder.Entity<Solution>(entity =>
            {
                entity.Property(e => e.SolutionId).ValueGeneratedNever();

                entity.Property(e => e.Author)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.DateAdded).HasColumnType("date");

                entity.HasOne(d => d.Problem)
                    .WithMany(p => p.Solution)
                    .HasForeignKey(d => d.ProblemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Solution__Proble__2C3393D0");
            });

            modelBuilder.Entity<Evaluation>(entity =>
            {
                entity.Property(e => e.EvaluationId).ValueGeneratedNever();

                entity.Property(e => e.EvaluationResult).IsRequired();

                entity.HasOne(d => d.Submision)
                    .WithMany(p => p.Evaluation)
                    .HasForeignKey(d => d.SubmisionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Evaluatio__Submi__36B12243");
            });

            modelBuilder.Entity<Tests>(entity =>
            {
                entity.HasKey(e => e.TestId);

                entity.Property(e => e.TestId).ValueGeneratedNever();

                entity.Property(e => e.FisierIn)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FisierOk)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TestInput).IsRequired();

                entity.Property(e => e.TestOutput).IsRequired();

                entity.HasOne(d => d.Problem)
                    .WithMany(p => p.Tests)
                    .HasForeignKey(d => d.ProblemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Tests__ProblemId__412EB0B6");
            });

            modelBuilder.Entity<Submision>(entity =>
            {
                entity.Property(e => e.SubmisionId).ValueGeneratedNever();

                entity.Property(e => e.Compilator)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.DataOfSubmision).HasColumnType("datetime");

                entity.Property(e => e.MessageOfSubmision)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.SourceSize)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.StateOfSubmision)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Problem)
                    .WithMany(p => p.Submision)
                    .HasForeignKey(d => d.ProblemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Submision__Probl__2D27B809");
            });
        }
    }
}
