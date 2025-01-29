using NotificationSender.Infrastructure.Abstractions.QueryObjects;
using NotificationSender.Infrastructure.Abstractions.DTOs;
using System.Linq.Expressions;

namespace NotificationSender.Infrastructure.Abstractions.Repositories;

public interface IQueryRepository<TDto, TQuery> 
    where TDto : BaseDto 
    where TQuery : QueryObject
{
    Task<TResult> GetSingleByQuery<TResult>(TQuery queryDto, Expression<Func<TDto, TResult>> select = null);
    Task<List<TResult>> GetCollectionByQuery<TResult>(TQuery queryDto, Expression<Func<TDto, TResult>> select = null);
}