using BotMovieVort.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Domain.Repository.Abstract
{
    public interface IItemFilmRepository
    {
        Task<IQueryable<ItemFilm>> GetItemFilms();
        Task<ItemFilm> GetItemFilmById(Guid id);
        Task SaveItemFilm(ItemFilm entity);
        Task DeleteItemFilm(Guid id);
        Task UpdateItemFilm(ItemFilm entity);
        Task<ItemFilm> GetItemFilmByNameAndYear(string name, string year);
    }
}
