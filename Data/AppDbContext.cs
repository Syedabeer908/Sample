using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<RefreshToken> RefreshToken { get; set; }
    public DbSet<UserLoginHistory> UserLoginHistory { get; set; }
    public DbSet<Risk> Risk { get; set; }
    public DbSet<Control> Control { get; set; }
    public DbSet<RiskControl> RiskControl { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<ExternalLogin> ExternalLogin { get; set; }
    public DbSet<Role> Role { get; set; }
   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            // Unique index for Name
            entity.HasIndex(n => n.Name)
                .IsUnique();
            entity.HasIndex(r => r.RoleId);
            entity.HasIndex(id => id.IsDeleted);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasOne(r => r.Role)
                  .WithMany(u => u.Users)
                  .HasForeignKey(ur => ur.RoleId)
                  .HasPrincipalKey(r => r.RoleId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Unique indexes for Email
            entity.HasIndex(u => u.Email)
                  .IsUnique();

            entity.HasIndex(u => u.UserId);
            entity.HasIndex(r => r.RoleId);
            entity.HasIndex(ia => ia.IsActive);
            entity.HasIndex(id => id.IsDeleted);
        });

        modelBuilder.Entity<ExternalLogin>(entity =>
        {
            entity.HasOne(u => u.User)
                  .WithMany(e => e.ExternalLogins)
                  .HasForeignKey(r => r.UserId)
                  .HasPrincipalKey(u => u.UserId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(e => e.ExternalLoginId);
            entity.HasIndex(u => u.UserId);
            entity.HasIndex(e => new { e.Provider, e.ProviderKey }).IsUnique();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasOne(r => r.User)
                  .WithMany(r => r.RefreshTokens)
                  .HasForeignKey(r => r.UserId)
                  .HasPrincipalKey(u => u.UserId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(id => id.RefreshTokenId);
            entity.HasIndex(id => id.UserId);
            entity.HasIndex(n => n.Token);
        });

        modelBuilder.Entity<UserLoginHistory>(entity =>
        {
            entity.HasOne(r => r.User)
                  .WithMany(r => r.UserLoginHistories)
                  .HasForeignKey(r => r.UserId)
                  .HasPrincipalKey(u => u.UserId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(id => id.UserLoginHistoryId);
            entity.HasIndex(id => id.UserId);
            entity.HasIndex(ip => ip.IpAddress);
            entity.HasIndex(d => d.DeviceInfo);
        });

        modelBuilder.Entity<Risk>(entity =>
        {
            entity.HasOne(u => u.User)
                  .WithMany(r => r.Risks)
                  .HasForeignKey(u => u.UserId)
                  .HasPrincipalKey(u => u.UserId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(r => r.RiskId);
            entity.HasIndex(u => u.UserId);
            entity.HasIndex(id => id.IsDeleted);
        });

        modelBuilder.Entity<Control>(entity =>
        {
            entity.HasOne(u => u.User)
                  .WithMany(c => c.Controls)
                  .HasForeignKey(u => u.UserId)
                  .HasPrincipalKey(u => u.UserId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(c => c.ControlId);
            entity.HasIndex(u => u.UserId);
            entity.HasIndex(id => id.IsDeleted);
        });

        modelBuilder.Entity<RiskControl>(entity =>
        {
            entity.HasOne(u => u.User)
                  .WithMany(r => r.RiskControls)
                  .HasForeignKey(u => u.UserId)
                  .HasPrincipalKey(u => u.UserId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(r => r.Risk)
                  .WithMany(rc => rc.RiskControls)
                  .HasForeignKey(r => r.RiskId)
                  .HasPrincipalKey(r => r.RiskId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(c => c.Control)
                  .WithMany(rc => rc.RiskControls)
                  .HasForeignKey(c => c.ControlId)
                  .HasPrincipalKey(c => c.ControlId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(rc => rc.RiskControlId);
            entity.HasIndex(u => u.UserId);
            entity.HasIndex(r => r.RiskId);
            entity.HasIndex(c => c.ControlId);
            entity.HasIndex(id => id.IsDeleted);
        });
    }
}

