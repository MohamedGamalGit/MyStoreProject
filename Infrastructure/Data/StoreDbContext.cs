using Infrastructure.Configrations;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Models.Models.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Product { get; set; } = null!;
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Page> Page => Set<Page>();
        public DbSet<ActionEntity> ActionEntity => Set<ActionEntity>();
        public DbSet<PageAction> PageAction => Set<PageAction>();
        public DbSet<RolePageAction> RolePageAction => Set<RolePageAction>();
        public DbSet<UserPageAction> UserPageAction => Set<UserPageAction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // ← هنا EF Core يبني الجداول الافتراضية (زي Users لو عندك Identity)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


            modelBuilder.Entity<Page>().HasIndex(p => p.Key).IsUnique();
            modelBuilder.Entity<ActionEntity>().HasIndex(a => a.Key).IsUnique();

            modelBuilder.Entity<PageAction>()
                .HasOne(pa => pa.Page).WithMany(p => p.PageActions).HasForeignKey(pa => pa.PageId);

            modelBuilder.Entity<PageAction>()
                .HasOne(pa => pa.ActionEntity).WithMany(a => a.PageActions).HasForeignKey(pa => pa.ActionEntityId);

            modelBuilder.Entity<RolePageAction>()
                .HasOne(rp => rp.Role).WithMany(r => r.RolePageActions).HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePageAction>()
                .HasOne(rp => rp.PageAction).WithMany().HasForeignKey(rp => rp.PageActionId);

            // optional: composite unique to avoid duplicates
            modelBuilder.Entity<RolePageAction>()
                .HasIndex(rp => new { rp.RoleId, rp.PageActionId }).IsUnique();
        }
    }

}
