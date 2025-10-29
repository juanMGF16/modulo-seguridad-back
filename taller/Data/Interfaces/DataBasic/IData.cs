﻿namespace Data.Interfaces.DataBasic
{
    public interface IData<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetDeletes();
        Task<T?> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteLogicAsync(int id);
        Task<bool> RestoreAsync(int id);

    }
}
