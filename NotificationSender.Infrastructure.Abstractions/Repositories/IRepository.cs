using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Domain.Entities;

namespace NotificationSender.Infrastructure.Abstractions.Repositories;

public interface IRepository<TEntity, TDto> 
    where TEntity : BaseEntity
    where TDto : BaseDto
{
    Task<TDto> GetByIdAsync(int id);
    Task<IEnumerable<TDto>> GetAllAsync();
    Task<TDto> AddAsync(TDto entity);
    Task Update(TDto entity);
    Task Remove(TDto entity);
}