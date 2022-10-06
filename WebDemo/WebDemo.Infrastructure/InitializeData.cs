using Microsoft.EntityFrameworkCore;
using WebApiDemo.Core.Models;
using WebDemo.Core.Models;
using WebDemo.Infrastructure.Data;

namespace WebDemo.Infrastructure
{
    public class InitializeData
    {
        private readonly WebApiDbContext _webApiDbContext;

        public InitializeData(WebApiDbContext webApiDbContext)
        {
            _webApiDbContext = webApiDbContext;
        }

        public void MigrateDataBase()
        {
            _webApiDbContext.Database.Migrate();
        }

        public void SeedData()
        {
            //Inserer données
            //Création elements... si aucun element est semblable 
            if (!_webApiDbContext.User.Any())
            {
                var users = new List<User>
                {
                    new User{FirstName="Carson",LastName="Alexander", Email="Zouhir@gmail.com"},
                    new User{FirstName="pupup",LastName="youba", Email="fzafazuhir@gmail.com"},
                    new User{FirstName="Zouhir",LastName="lop", Email="Z234hir@gmail.com"},
                    new User{FirstName="Zidane",LastName="Alexander", Email="Zidane@gmail.com"},
                };
                //ajout des donnée
                users.ForEach(s => _webApiDbContext.User.Add(s));
                _webApiDbContext.SaveChanges();
            }
            //---------------------------

            if (!_webApiDbContext.Device.Any())
            {
                var device = new List<Device>
                {
                    new Device {Type="Computer", Marque="Dell", ModelName="Latitude 5420",UserId=4},
                    new Device {Type="Computer", Marque="Msi", ModelName="Titan GT77 - 12U", UserId=4},
                    new Device {Type="Smartphone", Marque="Iphone", ModelName="Iphone 14", UserId=7},

                };
                //ajout des donnée             
                device.ForEach(d => _webApiDbContext.Device.Add(d));
                _webApiDbContext.SaveChanges();
            }

        }
    }
}
