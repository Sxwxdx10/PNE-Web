using PNE_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNE_core.Services.Interfaces
{
    public interface IPneServices<T> where T : class
    {
        public Task<T?> GetByIdAsync(string id);
        public Task<List<T>> GetAllAsync();
        public Task<T?> GetForDetailAsync(string id);
        public Task CreateAsync(T entity);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(string id);
        public Task<bool> IsExist(string id);
    }
}
