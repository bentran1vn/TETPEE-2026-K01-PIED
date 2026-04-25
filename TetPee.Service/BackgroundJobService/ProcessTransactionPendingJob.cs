
using Quartz;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TetPee.Repository;

namespace TetPee.Service.BackgroundJobService;

[DisallowConcurrentExecution]
public class ProcessTransactionPendingJob : IJob
{
    private const string PendingStatus = "Pending";
    private const string CancelledStatus = "Cancelled";
    private static readonly TimeSpan DefaultPendingTimeout = TimeSpan.FromMinutes(2);

    private readonly AppDbContext _dbContext;
    private readonly ILogger<ProcessTransactionPendingJob> _logger;

    public ProcessTransactionPendingJob(
        AppDbContext dbContext,
        ILogger<ProcessTransactionPendingJob> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var pendingTimeoutMinutes = (int)DefaultPendingTimeout.TotalMinutes;

        var now = DateTimeOffset.UtcNow;
        var threshold = now.AddMinutes(-pendingTimeoutMinutes);

        var expiredPendingOrders = await _dbContext.Orders
            .Where(o => o.Status == PendingStatus && o.CreatedAt <= threshold)
            .ToListAsync(context.CancellationToken);

        if (expiredPendingOrders.Count == 0)
        {
            _logger.LogInformation("ProcessTransactionPendingJob completed: no expired pending orders found.");
            return;
        }

        foreach (var order in expiredPendingOrders)
        {
            order.Status = CancelledStatus;
            order.UpdatedAt = now;
        }
        
        _dbContext.Orders.UpdateRange(expiredPendingOrders);
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation(
            "ProcessTransactionPendingJob completed: cancelled {CancelledCount} pending orders older than {PendingTimeoutMinutes} minutes.",
            expiredPendingOrders.Count,
            pendingTimeoutMinutes);
    }
}