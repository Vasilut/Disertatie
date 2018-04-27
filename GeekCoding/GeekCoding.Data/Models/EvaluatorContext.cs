using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GeekCoding.Data.Models
{
    public partial class EvaluatorContext : IdentityDbContext<User>
    {
        public virtual DbSet<Problem> Problem { get; set; }

        public EvaluatorContext(DbContextOptions options):base(options)
        {

        }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
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

                entity.Property(e => e.ProblemContent)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ProblemName)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}
