using Agentic_Rentify.Core.Common;

namespace Agentic_Rentify.Application.Specifications;

public sealed class ActiveEntitySpecification<T>() : BaseSpecification<T>(e => !e.IsDeleted) where T : BaseEntity
{
}
