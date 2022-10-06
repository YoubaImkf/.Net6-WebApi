// => Un modèle est un ensemble de classes qui représentent les données gérées par l’application. Le modèle de cette application est une classe unique.

//source: https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-6.0&tabs=visual-studio
using System.ComponentModel.DataAnnotations;
using WebDemo.Core.Models;

namespace WebApiDemo.Core.Models
{
    public class User //(TodoItem)
    {
        public User()
        {
            Devices = new HashSet<Device>(); //permet de charger les devices
        }
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }  //? : non-nullables
        public string? Email { get; set; }

        
        //source : https://www.tutorialspoint.com/entity_framework/entity_framework_relationships.htm#:~:text=Entity%20Framework%20%3A%20A%20Comprehensive%20Course&text=In%20relational%20databases%2C%20relationship%20is,the%20data%20in%20two%20tables.
        //Une convention consiste à inclure une propriété de navigation de collection dans l'entité principale
        public ICollection<Device> Devices { get; set; }

    }

}
