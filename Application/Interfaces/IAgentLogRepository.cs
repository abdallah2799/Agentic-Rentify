using Agentic_Rentify.Core.Entities;

namespace Agentic_Rentify.Application.Interfaces;

public interface IAgentLogRepository
{
    Task SaveLogAsync(AgentExecutionLog log, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentExecutionLog>> GetLogsByUserAsync(string userId, int pageNumber = 1, int pageSize = 20);
}
