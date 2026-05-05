using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;
using WebApplication1.Interfaces;
using static WebApplication1.Data.Soft;

namespace WebApplication1.Repository
{
    public class SoftRepository : ISoftRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SoftRepository> _logger;

        public SoftRepository(AppDbContext context, ILogger<SoftRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        private async Task SoftAsync<TEntity>(
            IQueryable<TEntity> query, SoftValues values) where TEntity : BaseEntity
        {
            await query.ExecuteUpdateAsync(setters => setters
                .SetProperty(TEntity => TEntity.IsDeleted, values.Action)
                .SetProperty(TEntity => TEntity.DeletedAt, values.ActionAt)
                .SetProperty(TEntity => TEntity.DeletedBy, values.ActionBy));
        }


        public async Task SoftRoleAsync(Guid roleId, SoftValues values)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                await SoftAsync(_context.Role.Where(r => r.RoleId == roleId), values);
                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error: {Role Soft delete}", ex.ToString());
                throw;
            }
        }

        public async Task SoftUserAsync(Guid userId, SoftValues values)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                await SoftAsync(_context.User.Where(u => u.UserId == userId), values);

                await SoftAsync(_context.Risk.Where(r => r.UserId == userId), values);

                await SoftAsync(_context.Control.Where(c => c.UserId == userId), values);

                await SoftAsync(_context.RiskControl.Where(rc => rc.UserId == userId), values);

                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error: {User Soft delete}", ex.ToString());
                throw;
            }
        }

        public async Task SoftRiskAsync(Guid RiskId, SoftValues values)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                await SoftAsync(_context.Risk.Where(u => u.RiskId == RiskId), values);

                var riskControlIds = await _context.RiskControl
                    .Where(r => r.RiskId == RiskId).Select(rc => rc.RiskControlId).ToListAsync();

                await SoftAsync(_context.RiskControl.Where(rc => riskControlIds.Contains(rc.RiskControlId)), values);

                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error: {Risk Soft delete}", ex.ToString());
                throw;
            }
        }

        public async Task SoftControlAsync(Guid controlId, SoftValues values)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                await SoftAsync(_context.Control.Where(c => c.ControlId == controlId), values);

                var riskControlIds = await _context.RiskControl
                    .Where(c => c.ControlId == controlId).Select(rc => rc.RiskControlId).ToListAsync();

                await SoftAsync(_context.RiskControl.Where(rc => riskControlIds.Contains(rc.RiskControlId)), values);

                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error: {Control Soft delete}", ex.ToString());
                throw;
            }
        }

        public async Task SoftRiskControlAsync(Guid riskControlId, SoftValues values)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                await SoftAsync(_context.RiskControl.Where(rc => rc.RiskControlId == riskControlId), values);
                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error: {RiskControl Soft delete}", ex.ToString());
                throw;
            }
        }
    }
}
