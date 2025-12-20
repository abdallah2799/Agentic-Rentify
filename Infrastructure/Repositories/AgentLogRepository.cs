using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Agentic_Rentify.Infrastructure.Repositories;

public class AgentLogRepository(ApplicationDbContext context) : IAgentLogRepository
{
    public async Task SaveLogAsync(AgentExecutionLog log, CancellationToken cancellationToken = default)
    {
        context.AgentExecutionLogs.Add(log);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AgentExecutionLog>> GetLogsByUserAsync(string userId, int pageNumber = 1, int pageSize = 20)
    {
        return await context.AgentExecutionLogs
            .Where(log => log.UserId == userId)
            .OrderByDescending(log => log.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }
}
