using Microsoft.EntityFrameworkCore;
using WebApiDemo.Core.Models;
using WebDemo.Core.Interfaces;
using WebDemo.Core.Models;


//source https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-6.0&tabs=visual-studio

namespace WebDemo.Infrastructure.Data
{
    /*DbContext c'est un pont entre votre domaine ou vos classes d'entité et la base de données.DbContext est la classe principale responsable de l'interaction avec la base de données. ... Requête: convertit les requêtes LINQ-to-Entities en requête SQL et les envoie à la base de données.*/
    public class WebApiDbContext : DbContext, IWebApiDbContext
    {
        public WebApiDbContext(DbContextOptions<WebApiDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Device> Device { get; set; }



    }
}
