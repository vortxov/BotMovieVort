using BotMovieVort.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Domain.Repository.Abstract
{
    public interface IItemSerialsRepository
    {
        Task<IQueryable<ItemSerials>> GetItemSerials();
        Task<ItemSerials> GetItemSerialsById(Guid id);
        Task SaveItemSerials(ItemSerials entity);
        Task DeleteItemSerials(Guid id);
        Task UpdateItemSerials(ItemSerials entity);
        Task<ItemSerials> GetItemSerialsByNameAndYear(string name, string year);
    }
}
