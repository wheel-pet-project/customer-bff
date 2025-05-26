using System.Reflection;
using Clients;
using Clients.Clients.Booking;
using Clients.Clients.Check;
using Clients.Clients.DrivingLicense;
using Clients.Clients.Identity;
using Clients.Clients.Rent;
using Clients.Clients.VehicleFleet;
using Clients.Config;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using OpenApiContractV1.Formatters;
using OpenApiContractV1.OpenApi;

namespace Gateway;

public static class ServiceCollectionExtensions
{
    private static readonly GatewayConfiguration Configuration;
    
    public static IServiceCollection RegisterGrpcClientsAndConfigure(this IServiceCollection services)
    {
        var configs = new List<ClientChannelConfig?>
        {
            ParseToChannelConfig<IdentityChannelConfig>(Configuration.Identity),
            ParseToChannelConfig<BookingChannelConfig>(Configuration.Booking),
            ParseToChannelConfig<VehicleCheckChannelConfig>(Configuration.VehicleCheck),
            ParseToChannelConfig<DrivingLicenseChannelConfig>(Configuration.DrivingLicense),
            ParseToChannelConfig<RentChannelConfig>(Configuration.Rent),
            ParseToChannelConfig<VehicleFleetChannelConfig>(Configuration.VehicleFleet)
        };
        
        services.AddGrpcClientsForMicroservices(configs);

        return services;

        T ParseToChannelConfig<T>(ChannelConfiguration channelCfg)
        where T: ClientChannelConfig, new()
        {
            return new T { Uri = new Uri(channelCfg.Uri), ApiKey = channelCfg.ApiKey };
        }
    }

    public static IServiceCollection RegisterGrpcClientWrappers(this IServiceCollection services)
    {
        services.AddScoped<IdentityClientWrapper>();
        services.AddScoped<BookingClientWrapper>();
        services.AddScoped<CheckClientWrapper>();
        services.AddScoped<DrivingLicenseClientWrapper>();
        services.AddScoped<RentClientWrapper>();
        services.AddScoped<VehicleFleetClientWrapper>();

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
    
    
    static ServiceCollectionExtensions()
    {
        var env = Environment.GetEnvironmentVariables();
        
        var identityCfg = 
            new ChannelConfiguration(GetFromEnvOrThrow("IDENTITY_URL"), GetFromEnvOrThrow("IDENTITY_API_KEY"));
        var bookingCfg = 
            new ChannelConfiguration(GetFromEnvOrThrow("BOOKING_URL"), GetFromEnvOrThrow("BOOKING_API_KEY"));
        var vehicleCheckCfg =
            new ChannelConfiguration(GetFromEnvOrThrow("VEHICLECHECK_URL"), GetFromEnvOrThrow("VEHICLECHECK_API_KEY"));
        var drivingLicenseCfg =
            new ChannelConfiguration(GetFromEnvOrThrow("DRIVINGLICENSE_URL"), GetFromEnvOrThrow("DRIVINGLICENSE_API_KEY"));
        var rentCfg = 
            new ChannelConfiguration(GetFromEnvOrThrow("RENT_URL"), GetFromEnvOrThrow("RENT_API_KEY"));
        var vehicleFleetCfg = 
            new ChannelConfiguration(GetFromEnvOrThrow("VEHICLEFLEET_URL"), GetFromEnvOrThrow("VEHICLEFLEET_API_KEY"));

        Configuration = new GatewayConfiguration(
            identityCfg,
            bookingCfg,
            vehicleCheckCfg,
            drivingLicenseCfg,
            rentCfg,
            vehicleFleetCfg);
        
        string GetFromEnvOrThrow(string variable)
        {
            var value = env[variable];
            if (value == null) throw new ArgumentException($"'{variable}' isn't set");

            return (string)value;
        }
    }

    private record GatewayConfiguration(
        ChannelConfiguration Identity,
        ChannelConfiguration Booking,
        ChannelConfiguration VehicleCheck,
        ChannelConfiguration DrivingLicense,
        ChannelConfiguration Rent,
        ChannelConfiguration VehicleFleet
    );

    private record ChannelConfiguration(string Uri, string ApiKey);
}