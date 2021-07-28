using Microsoft.EntityFrameworkCore;
using WebTiburonTestTask.Models.DbModels;

#nullable disable

namespace WebTiburonTestTask.Context
{
    public partial class SurveyDBContext : DbContext
    {
        public SurveyDBContext()
        {
        }

        public SurveyDBContext(DbContextOptions<SurveyDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Interview> Interviews { get; set; }
        public virtual DbSet<InterviewHasAnswer> InterviewHasAnswers { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Result> Results { get; set; }
        public virtual DbSet<Survey> Surveys { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Name=DbConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Russian_Russia.1251");

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.ToTable("Answer");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.AnswerText)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_question_asnwers");
            });

            modelBuilder.Entity<Interview>(entity =>
            {
                entity.ToTable("Interview");
            });

            modelBuilder.Entity<InterviewHasAnswer>(entity =>
            {
                entity.ToTable("InterviewHasAnswer");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.InterviewId).IsRequired();

                entity.HasOne(d => d.Answer)
                    .WithMany(p => p.InterviewHasAnswers)
                    .HasForeignKey(d => d.AnswerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_interviewHasAnswer_answers");

                entity.HasOne(d => d.Interview)
                    .WithMany(p => p.InterviewHasAnswers)
                    .HasForeignKey(d => d.InterviewId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_interviewHasAnswer_interviews");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("Question");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.QuestionText)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.HasOne(d => d.Survey)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.SurveyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_survey_questions");
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.ToTable("Result");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.InterviewId).IsRequired();

                entity.HasOne(d => d.Interview)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.InterviewId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_result_interviews");
            });

            modelBuilder.Entity<Survey>(entity =>
            {
                entity.ToTable("Survey");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.SurveyName)
                    .IsRequired()
                    .HasMaxLength(300);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
