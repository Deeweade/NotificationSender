using NotificationSender.Infrastructure.Abstractions.QueryObjects;
using NotificationSender.Infrastructure.Abstractions.Repositories;
using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Infrastructure.Contexts;
using NotificationSender.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using AutoMapper.QueryableExtensions;
using AutoMapper;

namespace NotificationSender.Infrastructure.Persistence.Repositories;

public class NotificationTemplateRepository : BaseRepository<NotificationTemplate, NotificationTemplateDto>, 
    INotificationTemplateRepository
{
    private readonly IMapper _mapper;

    public NotificationTemplateRepository(ApplicationDbContext context, IMapper mapper)
        : base(context, mapper)
    {
        _mapper = mapper;
    }

    public async Task<TResult> GetSingleByQuery<TResult>(
        NotificationTemplateQuery queryDto,
        Expression<Func<NotificationTemplateDto, TResult>> select = null)
    {
        var dbQuery = _context.NotificationTemplates
            .AsNoTracking()
            .ProjectTo<NotificationTemplateDto>(_mapper.ConfigurationProvider);

        if (queryDto.SystemEventIds != null && queryDto.SystemEventIds.Any())
        {
            dbQuery = dbQuery.Where(x => queryDto.SystemEventIds.Contains(x.SystemEventId));
        }

        if (queryDto.ChannelTypes != null && queryDto.ChannelTypes.Any())
        {
            dbQuery = dbQuery.Where(x => queryDto.ChannelTypes.Contains(x.NotificationChannelId));
        }

        if (select != null)
        {
            return await dbQuery.Select(select).FirstOrDefaultAsync();
        }

        return await dbQuery.Cast<TResult>().FirstOrDefaultAsync();
    }

    public async Task<List<TResult>> GetCollectionByQuery<TResult>(
        NotificationTemplateQuery query, 
        Expression<Func<NotificationTemplateDto, TResult>> select = null)
    {
        var dbQuery = _context.NotificationTemplates
            .AsNoTracking()
            .ProjectTo<NotificationTemplateDto>(_mapper.ConfigurationProvider);

        if (query.ChannelTypes != null && query.ChannelTypes.Any())
        {
            dbQuery = dbQuery.Where(x => query.ChannelTypes.Contains(x.NotificationChannelId));
        }

        if (query.SystemEventIds != null && query.SystemEventIds.Any())
        {
            dbQuery = dbQuery.Where(x => query.SystemEventIds.Contains(x.SystemEventId));
        }

        if (select != null)
        {
            return await dbQuery.Select(select).ToListAsync();
        }

        return await dbQuery.Cast<TResult>().ToListAsync();
    }
}