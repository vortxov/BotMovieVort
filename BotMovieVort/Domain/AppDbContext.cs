using BotMovieVort.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BotMovieVort.Domain
{
    public class AppDbContext : DbContext 
    {
        public DbSet<Genre> Genres { get; set; }
        public DbSet<ItemFilm> ItemFilms { get; set; }
        public DbSet<ItemSerials> ItemSerials { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Series> Series { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);

            var configuration = builder.Build(); 

            optionsBuilder.UseSqlServer(configuration["Project:ConnectionString"]); 
        }
    }
}
