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
        public virtual DbSet<Contest> Contest { get; set; }
        public virtual DbSet<UserContest> UserContest { get; set; }
        public virtual DbSet<ProblemContest> ProblemContest { get; set; }
        public virtual DbSet<SubmisionContest> SubmisionContest { get; set; }
        public virtual DbSet<Announcement> Announcement { get; set; }
        public virtual DbSet<UserInformation> UserInformation { get; set; }

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

            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.Property(e => e.AnnouncementId).ValueGeneratedNever();

                entity.HasOne(d => d.Contest)
                    .WithMany(p => p.Announcement)
                    .HasForeignKey(d => d.ContestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Announcem__Conte__123EB7A3");
            });

            modelBuilder.Entity<Contest>(entity =>
            {
                entity.Property(e => e.ContestId).ValueGeneratedNever();

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.StatusContest)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);
            });

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

            modelBuilder.Entity<ProblemContest>(entity =>
            {
                entity.Property(e => e.ProblemContestId).ValueGeneratedNever();

                entity.HasOne(d => d.Contest)
                    .WithMany(p => p.ProblemContest)
                    .HasForeignKey(d => d.ContestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProblemCo__Conte__0A9D95DB");

                entity.HasOne(d => d.Problem)
                    .WithMany(p => p.ProblemContest)
                    .HasForeignKey(d => d.ProblemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProblemCo__Probl__0B91BA14");
            });

            modelBuilder.Entity<SubmisionContest>(entity =>
            {
                entity.Property(e => e.SubmisionContestId).ValueGeneratedNever();

                entity.HasOne(d => d.Contest)
                    .WithMany(p => p.SubmisionContest)
                    .HasForeignKey(d => d.ContestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Submision__Conte__0E6E26BF");

                entity.HasOne(d => d.Submision)
                    .WithMany(p => p.SubmisionContest)
                    .HasForeignKey(d => d.SubmisionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Submision__Submi__0F624AF8");
            });

            modelBuilder.Entity<UserContest>(entity =>
            {
                entity.Property(e => e.UserContestId).ValueGeneratedNever();

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Contest)
                    .WithMany(p => p.UserContest)
                    .HasForeignKey(d => d.ContestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserConte__Conte__07C12930");
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

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

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
                    .IsRequired();

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
