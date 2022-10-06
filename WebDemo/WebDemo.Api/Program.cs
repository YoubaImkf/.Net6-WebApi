#region ( https:/SOURCE )
//source : source : https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-6.0&tabs=visual-studio

//source : https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-6.0&tabs=visual-studio
// depuis .Net 6 le STARTUP est dans le program.cs (surcouche);
#endregion

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using WebDemo.Api.Options;
using WebDemo.Core.Interfaces;
using WebDemo.Infrastructure;
using WebDemo.Infrastructure.Data;
using WebDemo.Infrastructure.IndentityData;

try // source: https://stackoverflow.com/questions/63642991/serilog-extensions-hosting-diagnosticcontext-while-attempting-to-activate-ser
{
    var builder = WebApplication.CreateBuilder(args);
    Microsoft.Extensions.Configuration.ConfigurationManager config = builder.Configuration; //AUTH

    // Add services to the container
    builder.Services.AddControllers();  // !!!!Add = ajout d'un middleware dans le pipeline!!!!
    builder.Services.AddEndpointsApiExplorer();

    #region//--------Versioning------before_Build----Swagger--
    builder.Services.AddSwaggerGen();//SWAGGER
    builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
    builder.Services.AddApiVersioning(config =>
    {
        config.DefaultApiVersion = new ApiVersion(1, 0);
        config.AssumeDefaultVersionWhenUnspecified = true;
        //options.ApiVersionReader = new HeaderApiVersionReader
        config.ReportApiVersions = true; //Display api supported version in header
        config.ApiVersionReader = new UrlSegmentApiVersionReader();
    });
    builder.Services.AddVersionedApiExplorer(setup =>
    {
        //how it will work
        setup.GroupNameFormat = "'v'VVVV";
        setup.SubstituteApiVersionInUrl = true;
    });
    #endregion

    builder.Services.AddMyServices();

    #region //--------Connection String----------
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<WebApiDbContext>(opt => opt.UseSqlServer(connectionString));
    builder.Services.AddScoped<IWebApiDbContext>(s => s.GetService<WebApiDbContext>());
    builder.Services.AddControllersWithViews();
    #endregion

    #region //--------Add AutoMapper-----------
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    #endregion

    #region //--------OPTIONS------------
    // IOptions
    builder.Services.Configure<PositionOption>(
        builder.Configuration.GetSection(PositionOption.Position));

    // IOptionsSnapshot
    builder.Services.Configure<PositionOption>(
        builder.Configuration.GetSection("Position"));
    #endregion

    #region //--------SeriLog--------
    /*// https://github.com/serilog/serilog/wiki/Configuration-Basics
     * (==SANS APP SETTING==)
    Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console()
        .WriteTo.File("log.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .Enrich.WithThreadId() //Les événements écrits dans le journal porteront une propriété ThreadId avec                        l'id du thread géré qui les a écrits.   
     .WriteTo.MSSqlServer(connectionString,sinkOptions: new MSSqlServerSinkOptions { TableName = "LogEvents"})
        .CreateLogger();
    ==============================================*/
    /*
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
            .Build();*/

    var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();
    builder.Host.UseSerilog(logger);

    /*logger.Information("Application is RUNNING..... follow her x)");*/
    #endregion


    #region //--------For Entity Framework-----AUTH__AuthDbContext
    builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
    #endregion

    #region //--------For Identity------AUTH
    builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<AuthDbContext>()//nugget AddEntityFrameworkStores
                    .AddDefaultTokenProviders();
    #endregion

    #region //--------Add Authentication-------AUTH
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })

    #endregion

    #region //---------Adding JWT Bearer-----ATUH
    .AddJwtBearer(options =>
     {
         options.SaveToken = true;
         options.RequireHttpsMetadata = false;
         options.TokenValidationParameters = new TokenValidationParameters()
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidAudience = config["JWT:ValidAudience"],
             ValidIssuer = config["JWT:ValidIssuer"],
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]))
         };
     });
    #endregion


    #region//--------Builder-------
    var app = builder.Build();
    using var scope = app.Services.CreateScope(); //pour pres charger 
    var services = scope.ServiceProvider;
    #endregion

    #region//--------DbContext--------
    var webApiDbContext = services.GetRequiredService<WebApiDbContext>(); //recup le dbcontext
    var initializeData = new InitializeData(webApiDbContext);
    initializeData.MigrateDataBase();
    initializeData.SeedData();
    #endregion

    #region//--------Versioning---after_Build---useSwagger---
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json", description.ApiVersion.ToString()
                );
            options.RoutePrefix = string.Empty; //si absent redirige vers index pas swagger/index
        }
    });
    #endregion

    app.UseAuthentication();//authentication

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete !!!!! ");
    Log.CloseAndFlush();
}