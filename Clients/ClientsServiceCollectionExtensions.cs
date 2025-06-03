using Api;
using Clients.Config;
using Clients.Interceptors;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Proto.DrivingLicenseV1;
using Proto.IdentityV1;
using Proto.VehicleFleetV1;

namespace Clients;

public static class ClientsServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcClientsForMicroservices(
        this IServiceCollection services,
        List<ClientChannelConfig?> configs)
    {
        const string identity = "IDENTITY";
        const string booking = "BOOKING";
        const string vehicleCheck = "VEHICLECHECK";
        const string drivingLicense = "DRIVINGLICENSE";
        const string rent = "RENT";
        const string vehicleFleet = "VEHICLEFLEET";
        
        var (identityCfg,
            bookingCfg,
            vehicleCheckCfg,
            drivingLicenseCfg,
            rentCfg,
            vehicleFleetCfg) = DistributeСonfigs(configs);


        services
            .AddGrpcClient<Identity.IdentityClient>(c => { c.Address = new Uri($"static:///{identity}"); })
            .ConfigureChannel(ConfigureByStandard(identityCfg))
            .AddInterceptor<CorrelationInterceptor>();

        services.AddGrpcClient<Booking.BookingClient>(c => { c.Address = new Uri($"static:///{booking}"); })
            .ConfigureChannel(ConfigureByStandard(bookingCfg))
            .AddInterceptor<CorrelationInterceptor>();

        services.AddGrpcClient<VehicleCheck.VehicleCheckClient>(c =>
            {
                c.CallOptionsActions.Add(callOptions =>
                {
                    callOptions.CallOptions = new CallOptions(new Metadata());
                });
                c.Address = new Uri($"static:///{vehicleCheck}");
            })
            .ConfigureChannel(ConfigureByStandard(vehicleCheckCfg))
            .AddInterceptor<CorrelationInterceptor>();

        services.AddGrpcClient<DrivingLicense.DrivingLicenseClient>(c =>
            {
                c.Address = new Uri($"static:///{drivingLicense}");
            })
            .ConfigureChannel(ConfigureByStandard(drivingLicenseCfg))
            .AddInterceptor<CorrelationInterceptor>();

        services.AddGrpcClient<Rent.RentClient>(c => { c.Address = new Uri($"static:///{rent}"); })
            .ConfigureChannel(ConfigureByStandard(rentCfg))
            .AddInterceptor<CorrelationInterceptor>();

        services.AddGrpcClient<VehicleFleet.VehicleFleetClient>(c =>
            {
                c.Address = new Uri($"static:///{vehicleFleet}");
            })
            .ConfigureChannel(ConfigureByStandard(vehicleFleetCfg))
            .AddInterceptor<CorrelationInterceptor>();


        var factory = new StaticResolverFactory(uri =>
        {
            var serviceName = uri.AbsolutePath[1..];

            return serviceName switch
            {
                identity => [new BalancerAddress(identityCfg.Uri.Host, identityCfg.Uri.Port)],
                booking => [new BalancerAddress(bookingCfg.Uri.Host, bookingCfg.Uri.Port)],
                vehicleCheck => [new BalancerAddress(vehicleCheckCfg.Uri.Host, vehicleCheckCfg.Uri.Port)],
                drivingLicense => [new BalancerAddress(drivingLicenseCfg.Uri.Host, drivingLicenseCfg.Uri.Port)],
                rent => [new BalancerAddress(rentCfg.Uri.Host, rentCfg.Uri.Port)],
                vehicleFleet => [new BalancerAddress(vehicleFleetCfg.Uri.Host, vehicleFleetCfg.Uri.Port)],
                _ => throw new ArgumentException("Unknown host")
            };
        });

        services.AddSingleton<ResolverFactory>(factory);

        return services;
    }

    private static Action<GrpcChannelOptions> ConfigureByStandard(ClientChannelConfig config)
    {
        return channel =>
        {
            channel.Credentials = ChannelCredentials.Create(ChannelCredentials.SecureSsl,
                CallCredentials.FromInterceptor((_, metadata) =>
                {
                    metadata.Add("X-Api-Key", config.ApiKey);
                    return Task.CompletedTask;
                }));
            channel.Credentials = ChannelCredentials.Insecure;
            channel.ServiceConfig = new ServiceConfig
            {
                LoadBalancingConfigs = { new RoundRobinConfig() }
            };
        };
    }

    private static (
        IdentityChannelConfig,
        BookingChannelConfig,
        VehicleCheckChannelConfig,
        DrivingLicenseChannelConfig,
        RentChannelConfig,
        VehicleFleetChannelConfig) DistributeСonfigs(List<ClientChannelConfig?> configs)
    {
        if (configs.Count == 0 || configs.Any(x => x == null))
            throw new ArgumentException("configs list is empty or any config is null");

        var identityCfg =
            configs.FirstOrDefault(x => x is IdentityChannelConfig) as IdentityChannelConfig ??
            throw new ArgumentException("IdentityConfig not found");
        var bookingCfg =
            configs.FirstOrDefault(x => x is BookingChannelConfig) as BookingChannelConfig ??
            throw new ArgumentException("BookingConfig not found");
        var checkCfg =
            configs.FirstOrDefault(x => x is VehicleCheckChannelConfig) as VehicleCheckChannelConfig ??
            throw new ArgumentException("VehicleCheckConfig not found");
        var drivingLicenseCfg =
            configs.FirstOrDefault(x => x is DrivingLicenseChannelConfig) as DrivingLicenseChannelConfig ??
            throw new ArgumentException("DrivingLicenseConfig not found");
        var rentCfg =
            configs.FirstOrDefault(x => x is RentChannelConfig) as RentChannelConfig ??
            throw new ArgumentException("RentConfig not found");
        var vehicleFleetCfg =
            configs.FirstOrDefault(x => x is VehicleFleetChannelConfig) as VehicleFleetChannelConfig ??
            throw new ArgumentException("VehicleFleetConfig not found");

        return (
            identityCfg,
            bookingCfg,
            checkCfg,
            drivingLicenseCfg,
            rentCfg,
            vehicleFleetCfg
        );
    }
}