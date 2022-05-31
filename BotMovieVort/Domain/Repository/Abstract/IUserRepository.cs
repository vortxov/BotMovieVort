using BotMovieVort.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Domain.Repository.Abstract
{
    public interface IUserRepository
    {
        Task<IQueryable<User>> GetUsers();
        Task<User> GetUserById(Guid id);
        Task SaveUser(User entity);
        Task DeleteUser(Guid id);
        Task UpdateUser(User entity);
        Task<User> GetUserByIdUser(long id);
    }
}
