using System.Collections.Generic;

namespace WebApiDemo.Dtos
// => Un modèle est un ensemble de classes qui représentent les données gérées par l’application. Le modèle de cette application est une classe unique.

//Un DTO (Data Transfer Object) est un objet qui définit comment les données seront envoyées entre les applications. (seulement ce quon veux montrer au front)

{  //source: https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-6.0&tabs=visual-studio
    public class UserDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }

        public ICollection<DeviceDto>? Devices { get; set; }
    }
}
