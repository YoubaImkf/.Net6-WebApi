using WebDemo.Core.Interfaces;
using WebDemo.Api.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        //Ajoute mes services
        public static IServiceCollection AddMyServices(this IServiceCollection services)
        {
            return services.AddTransient<IUserService, UserService>();
        }

    }
}