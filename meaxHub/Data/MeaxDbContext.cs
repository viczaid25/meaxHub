using MeaxHub.Models;
using Microsoft.EntityFrameworkCore;

namespace MeaxHub.Data
{
    public class MeaxDbContext : DbContext
    {
        public MeaxDbContext(DbContextOptions<MeaxDbContext> options) : base(options) { }

        public DbSet<MeaxAllUser> MeaxAllUsers => Set<MeaxAllUser>();
        public DbSet<MeaxAllSystem> MeaxAllSystems => Set<MeaxAllSystem>();
        public DbSet<MeaxAllUserSystem> MeaxAllUserSystems => Set<MeaxAllUserSystem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // meax_all_user
            modelBuilder.Entity<MeaxAllUser>(e =>
            {
                e.ToTable("meax_all_user");
                e.HasKey(x => x.Id);

                e.Property(x => x.PcLoginId).HasColumnName("pc_login_id").HasMaxLength(50).IsRequired();
                e.Property(x => x.Username).HasColumnName("username");
                e.Property(x => x.Email).HasColumnName("email");
                e.Property(x => x.Department).HasColumnName("department");
                e.Property(x => x.Position).HasColumnName("position");
                e.Property(x => x.Dep2).HasColumnName("dep_2");
                e.Property(x => x.Auth).HasColumnName("auth");
            });

            // meax_all_system
            modelBuilder.Entity<MeaxAllSystem>(e =>
            {
                e.ToTable("meax_all_system");
                e.HasKey(x => x.Id);

                e.Property(x => x.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                e.Property(x => x.Url).HasColumnName("url").HasMaxLength(200).IsRequired();
                e.Property(x => x.IsActive).HasColumnName("is_active");
            });

            // meax_all_user_system
            modelBuilder.Entity<MeaxAllUserSystem>(e =>
            {
                e.ToTable("meax_all_user_system");
                e.HasKey(x => x.Id);

                e.Property(x => x.UserId).HasColumnName("user_id");
                e.Property(x => x.SystemId).HasColumnName("system_id");
                e.Property(x => x.Role).HasColumnName("role");

                e.HasOne(x => x.User)
                    .WithMany(u => u.UserSystems)
                    .HasForeignKey(x => x.UserId)
                    .HasConstraintName("FK_meax_all_user_system_user");

                e.HasOne(x => x.System)
                    .WithMany(s => s.UserSystems)
                    .HasForeignKey(x => x.SystemId)
                    .HasConstraintName("FK_meax_all_user_system_system");
            });
        }
    }
}
