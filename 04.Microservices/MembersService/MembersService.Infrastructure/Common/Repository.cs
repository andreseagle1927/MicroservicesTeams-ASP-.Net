﻿using MembersService.Domain.Common;
using MembersService.Domain.Exceptions;
using MembersService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace MembersService.Infrastructure.Common;

public class Repository<T> : IRepository<T> where T : EntityBase
{
    private readonly AppDbContext _appDbContext;

    public Repository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<T> AddAsync(T entity)
    {
        _appDbContext.Set<T>().Add(entity);
        await _appDbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _appDbContext.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _appDbContext.Set<T>().ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _appDbContext.Set<T>().FindAsync(id);
    }

    public async Task RemoveAsync(T entity)
    {
        var id = entity?.Id;
        _ = await _appDbContext.Set<T>().FindAsync(id) ?? throw new NotFoundException($"Member with Id={id} Not Found");

        _appDbContext.Set<T>().Remove(entity!);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(Expression<Func<T, bool>> predicate, int id)
    {
        var entities = await _appDbContext.Set<T>().Where(predicate).ToListAsync();

        if (entities.Count == 0)
        {
            throw new NotFoundException($"No entities found");
        }

        _appDbContext.Set<T>().RemoveRange(entities);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<T> UpdateAsync(T entity)
    {
        var id = entity?.Id;
        var original = await _appDbContext.Set<T>().FindAsync(id) ?? throw new NotFoundException($"Member with Id={id} Not Found");

        _appDbContext.Entry(original).CurrentValues.SetValues(entity!);
        await _appDbContext.SaveChangesAsync();

        return entity!;
    }
}