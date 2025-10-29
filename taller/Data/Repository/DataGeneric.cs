﻿using Data.Repository;
using Entity.Domain.Models.Base;
using Entity.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositoy
{
    public class DataGeneric<T> : ABaseModelData<T> where T : BaseModel
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public DataGeneric(ApplicationDbContext context)
        {
            _dbSet = context.Set<T>();
            _context = context;
        }


        public override async Task<IEnumerable<T>> GetAllAsync()
        {
                return await _dbSet
                    .Where(e => e.IsDeleted == false)
                    .ToListAsync();
        }

        public override async Task<IEnumerable<T>> GetDeletes()
        {
                return await _dbSet
                    .Where(e => e.IsDeleted == true)
                    .ToListAsync();
        }

        public override async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.IsDeleted == false && e.Id == id);


        }
        public override async Task<T> CreateAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public override async Task<bool> UpdateAsync(T entity)
        {
            var existingEntity = await _dbSet.FindAsync(entity.Id);

            if (existingEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                _context.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }

            return await _context.SaveChangesAsync() > 0;
        }
        public override async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null) return false;

            _context.Set<T>().Remove(entity);
             return  await _context.SaveChangesAsync() > 0;
        }

        public override async Task<bool> DeleteLogicAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            _dbSet.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }


        public override async Task<bool> RestoreAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = false;
            _dbSet.Update(entity);
            return await _context.SaveChangesAsync() > 0;

        }


      


    }
}

