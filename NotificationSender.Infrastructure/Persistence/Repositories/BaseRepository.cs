using NotificationSender.Infrastructure.Abstractions.Repositories;
using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Infrastructure.Contexts;
using NotificationSender.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AutoMapper;

namespace NotificationSender.Infrastructure.Persistence.Repositories;

public class BaseRepository<TEntity, TDto> : IRepository<TEntity, TDto>
    where TEntity : BaseEntity
    where TDto : BaseDto
{
    protected readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public BaseRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public virtual async Task<TDto> GetByIdAsync(int id)
    {
        var entity = await _context
            .Set<TEntity>()
            .AsNoTracking()
            .ProjectTo<TDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity is null ? null : _mapper.Map<TDto>(entity);
    }

    public virtual async Task<IEnumerable<TDto>> GetAllAsync()
    {
        return await _context
            .Set<TEntity>()
            .AsNoTracking()
            .ProjectTo<TDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public virtual async Task<TDto> AddAsync(TDto dto)
    {
        var entity = _mapper.Map<TEntity>(dto);

        _context.Set<TEntity>().Add(entity);

        await _context.SaveChangesAsync();

        return _mapper.Map<TDto>(entity);
    }

    public virtual async Task Update(TDto dto)
    {
        var entity = await _context
            .Set<TEntity>()
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        _mapper.Map(dto, entity);

        _context.Set<TEntity>().Update(entity);
    }

    public virtual async Task Remove(TDto dto)
    {
        var entity = await _context
            .Set<TEntity>()
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        _context.Set<TEntity>().Remove(entity);
    }
}