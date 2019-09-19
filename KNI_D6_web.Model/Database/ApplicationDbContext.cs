using KNI_D6_web.Model.Achievements;
using KNI_D6_web.Model.Parameters;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KNI_D6_web.Model.Database
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Parameter> Parameters { get; set; }

        public DbSet<ParameterValue> ParameterValues { get; set; }

        public DbSet<Achievement> Achievements { get; set; }

        public DbSet<AchievementParameter> AchievementParameters { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<UserEvent> UserEvents { get; set; }

        public DbSet<NewsPost> NewsPosts { get; set; }

        public DbSet<UserAchievement> UserAchievements { get; set; }

        public DbSet<AchievementsGroup> AchievementGroups { get; set; }

        public DbSet<Semester> Semesters { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Achievement>(entity => 
            {
                entity.HasKey(a => a.Id);

                entity.HasOne(n => n.AchievementGroup)
                    .WithMany(u => u.Ahievements)
                    .HasForeignKey(n => n.AchievementsGroupId)
                    .HasPrincipalKey(u => u.Id)
                    .IsRequired();

                entity.HasOne(a => a.Semester)
                    .WithMany(s => s.Achievements)
                    .HasForeignKey(n => n.SemesterId);
            });

            modelBuilder.Entity<AchievementsGroup>()
                .HasKey(ag => ag.Id);

            modelBuilder.Entity<Parameter>()
                .Property(p => p.Name).IsRequired();

            modelBuilder.Entity<ParameterValue>()
            .HasKey(pv => new { pv.ParameterId, pv.UserId });

            modelBuilder.Entity<ParameterValue>()
                .HasOne(pv => pv.Parameter)
                .WithMany(p => p.ParameterValues)
                .HasForeignKey(pv => pv.ParameterId)
                .HasPrincipalKey(p => p.Id)
                .IsRequired();

            modelBuilder.Entity<ParameterValue>()
                .HasOne(pv => pv.User)
                .WithMany(u => u.ParameterValues)
                .HasForeignKey(pv => pv.UserId)
                .HasPrincipalKey(u => u.Id)
                .IsRequired();

            modelBuilder.Entity<Parameter>(entity => 
            {
                entity.Property(p => p.Name).IsRequired();
            });

            modelBuilder.Entity<AchievementParameter>(entity =>
            {
                entity.HasKey(ap => new { ap.ParameterId, ap.AchievementId});

                entity.HasOne(ap => ap.Parameter)
                    .WithMany(p => p.AchievementParameters)
                    .HasForeignKey(ap => ap.ParameterId)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();

                entity.HasOne(ap => ap.Achievement)
                    .WithMany(a => a.AchievementParameters)
                    .HasForeignKey(ap => ap.AchievementId)
                    .HasPrincipalKey(a => a.Id)
                    .IsRequired();
            });

            modelBuilder.Entity<Event>(entity => 
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Date).IsRequired();

                entity.HasOne(e => e.Semester)
                    .WithMany(s => s.Events)
                    .HasForeignKey(n => n.SemesterId);
            });

            //UserEvents
            modelBuilder.Entity<UserEvent>()
                .HasKey(ue => new { ue.UserId, ue.EventId });

            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.User)
                .WithMany(u => u.UserEvents)
                .HasForeignKey(ue => ue.UserId)
                .HasPrincipalKey(u => u.Id)
                .IsRequired();

            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.Event)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(ue => ue.EventId)
                .HasPrincipalKey(e => e.Id)
                .IsRequired();

            //NewsPosts
            modelBuilder.Entity<NewsPost>()
                .Property(n => n.Article).IsRequired();
            modelBuilder.Entity<NewsPost>()
                .Property(n => n.Text).IsRequired();
            modelBuilder.Entity<NewsPost>()
                .Property(n => n.PublicationDate).IsRequired();

            modelBuilder.Entity<NewsPost>()
                .HasOne(n => n.Author)
                .WithMany(u => u.NewsPosts)
                .HasForeignKey(n => n.AuthorId)
                .HasPrincipalKey(u => u.Id)
                .IsRequired();

            //UserAchievements
            modelBuilder.Entity<UserAchievement>()
                .HasKey(ua => new { ua.UserId, ua.AchievementId });

            modelBuilder.Entity<UserAchievement>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAchievements)
                .HasForeignKey(ua => ua.UserId)
                .HasPrincipalKey(u => u.Id)
                .IsRequired();

            modelBuilder.Entity<UserAchievement>()
                .HasOne(ua => ua.Achievement)
                .WithMany(a => a.UserAchievements)
                .HasForeignKey(ua => ua.AchievementId)
                .HasPrincipalKey(a => a.Id)
                .IsRequired();

            modelBuilder.Entity<Semester>(entity =>
            {
                entity.HasKey(s => s.Id);
            });
        }
    }
}
