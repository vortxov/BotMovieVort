using BotMovieVort.Domain.Entity;
using BotMovieVort.Domain.Repository.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Domain.Repository.EntityFramework
{
    public class UserRepository : IUserRepository
    {
        AppDbContext appDbContext;
        public UserRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task DeleteUser(Guid id)
        {
            appDbContext.Users.Remove(GetUserById(id).Result);
            await appDbContext.SaveChangesAsync();
        }

        public async Task<User> GetUserById(Guid id)
        {
            return appDbContext.Users.Include(x => x.ItemSerials).FirstOrDefault(x => x.Id == id);
        }

        public async Task<IQueryable<User>> GetUsers()
        {
            return appDbContext.Users;
        }

        public async Task SaveUser(User entity)
        {
            if (GetUserById(entity.Id).Result == null)
            {
                await appDbContext.Users.AddAsync(entity);
                await appDbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateUser(User entity)
        {
            if (GetUserById(entity.Id).Result != null)
            {
                appDbContext.Users.Update(entity);
                await appDbContext.SaveChangesAsync();
            }
        }
    }
}
