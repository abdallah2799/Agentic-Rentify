using System.Linq.Expressions;
using Agentic_Rentify.Application.Interfaces;

namespace Agentic_Rentify.Application.Specifications;

public abstract class BaseSpecification<T>(
    Expression<Func<T, bool>>? criteria = null,
    Expression<Func<T, object>>? orderBy = null,
    Expression<Func<T, object>>? orderByDescending = null,
    int? skip = null,
    int? take = null,
    bool isPagingEnabled = false) : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; } = criteria;
    public List<Expression<Func<T, object>>> Includes { get; } = [];
    public List<string> IncludeStrings { get; } = [];
    public Expression<Func<T, object>>? OrderBy { get; private set; } = orderBy;
    public Expression<Func<T, object>>? OrderByDescending { get; private set; } = orderByDescending;
    public int? Take { get; private set; } = take;
    public int? Skip { get; private set; } = skip;
    public bool IsPagingEnabled { get; private set; } = isPagingEnabled;

    protected void AddInclude(Expression<Func<T, object>> includeExpression) => Includes.Add(includeExpression);
    protected void AddInclude(string includeString) => IncludeStrings.Add(includeString);
    protected void AddOrderBy(Expression<Func<T, object>> orderByExpression) => OrderBy = orderByExpression;
    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression) => OrderByDescending = orderByDescExpression;
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}
