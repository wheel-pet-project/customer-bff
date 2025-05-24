using System.Reflection;
using Clients;
using Clients.Config;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using OpenApiContractV1.Formatters;
using OpenApiContractV1.OpenApi;

namespace Gateway;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterGrpcClientsAndConfigure(this IServiceCollection services, IConfiguration configuration)
    {
        var balancerConfigsSection = configuration.GetSection("BalancerConfigs");
        var identityCfg = balancerConfigsSection.GetSection("Identity").Get<IdentityBalancerConfig>();
        var bookingCfg = balancerConfigsSection.GetSection("Booking").Get<BookingBalancerConfig>();
        var vehicleCheckCfg = balancerConfigsSection.GetSection("VehicleCheck").Get<VehicleCheckBalancerConfig>();
        var drivingLicenseCfg = balancerConfigsSection.GetSection("DrivingLicense").Get<DrivingLicenseBalancerConfig>();
        var rentCfg = balancerConfigsSection.GetSection("RentBalancer").Get<RentBalancerConfig>();
        var vehicleFleetCfg = balancerConfigsSection.GetSection("VehicleFleet").Get<VehicleFleetBalancerConfig>();

        var configs = new List<MicroserviceBalancerConfig?>
        {
            identityCfg,
            bookingCfg,
            vehicleCheckCfg,
            drivingLicenseCfg,
            rentCfg,
            vehicleFleetCfg
        };
        
        services.AddGrpcClientsForMicroservices(configs);

        return services;
    }

    public static IServiceCollection RegisterSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "1.0",
                Title = "Customer BFF",
                Description = "BFF для работы с мобильным приложением клиента",
            });
            options.CustomSchemaIds(type => type.FriendlyId(true));
            options.IncludeXmlComments(
                $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly()?.GetName().Name}.xml");
        });
        
        services.AddSwaggerGenNewtonsoftSupport();

        return services;
    }

    public static IServiceCollection RegisterControllersWithNewtonsoft(this IServiceCollection services)
    {
        services.AddControllers(options => { options.InputFormatters.Insert(0, new InputFormatterStream()); })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.Converters.Add(new StringEnumConverter
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                });
            });

        return services;
    }
}