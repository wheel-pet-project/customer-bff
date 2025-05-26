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
        var (identityCfg,
            bookingCfg,
            vehicleCheckCfg,
            drivingLicenseCfg,
            rentCfg,
            vehicleFleetCfg) = DistributeBalancerСonfigs(configs);


        services
            .AddGrpcClient<Identity.IdentityClient>(c => { c.Address = new Uri($"static:///{identityCfg.Uri}"); })
            .ConfigureChannel(ConfigureByStandard(identityCfg))
            .AddInterceptor<CorrelationInterceptor>();

        services.AddGrpcClient<Booking.BookingClient>(c => { c.Address = new Uri($"static:///{bookingCfg.Uri}"); })
            .ConfigureChannel(ConfigureByStandard(bookingCfg))
            .AddInterceptor<CorrelationInterceptor>();

        services.AddGrpcClient<VehicleCheck.VehicleCheckClient>(c =>
            {
                c.Address = new Uri($"static:///{vehicleCheckCfg.Uri}");
            })
            .ConfigureChannel(ConfigureByStandard(vehicleCheckCfg))
            .AddInterceptor<CorrelationInterceptor>();

        services.AddGrpcClient<DrivingLicense.DrivingLicenseClient>(c =>
            {
                c.Address = new Uri($"static:///{drivingLicenseCfg.Uri}");
            })
            .ConfigureChannel(ConfigureByStandard(drivingLicenseCfg))
            .AddInterceptor<CorrelationInterceptor>();

        services.AddGrpcClient<Rent.RentClient>(c => { c.Address = new Uri($"static:///{rentCfg.Uri}"); })
            .ConfigureChannel(ConfigureByStandard(rentCfg))
            .AddInterceptor<CorrelationInterceptor>();

        services.AddGrpcClient<VehicleFleet.VehicleFleetClient>(c =>
            {
                c.Address = new Uri($"static:///{vehicleFleetCfg.Uri}");
            })
            .ConfigureChannel(ConfigureByStandard(vehicleFleetCfg))
            .AddInterceptor<CorrelationInterceptor>();


        var factory = new StaticResolverFactory(uri =>
        {
            var host = uri.Host;
            if (host == identityCfg.Uri.Host) return [new BalancerAddress(identityCfg.Uri.Host, identityCfg.Uri.Port)];
            if (host == bookingCfg.Uri.Host) return [new BalancerAddress(bookingCfg.Uri.Host, bookingCfg.Uri.Port)];
            if (host == vehicleCheckCfg.Uri.Host)
                return [new BalancerAddress(vehicleCheckCfg.Uri.Host, vehicleCheckCfg.Uri.Port)];
            if (host == drivingLicenseCfg.Uri.Host)
                return [new BalancerAddress(drivingLicenseCfg.Uri.Host, drivingLicenseCfg.Uri.Port)];
            if (host == rentCfg.Uri.Host) return [new BalancerAddress(rentCfg.Uri.Host, rentCfg.Uri.Port)];
            if (host == vehicleFleetCfg.Uri.Host)
                return [new BalancerAddress(vehicleFleetCfg.Uri.Host, vehicleFleetCfg.Uri.Port)];

            throw new ArgumentException("Unknown host");
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
        VehicleFleetChannelConfig) DistributeBalancerСonfigs(List<ClientChannelConfig?> configs)
    {
        if (configs.Count == 0 || configs.Any(x => x == null))
            throw new ArgumentException("configs list is empty or any config is null");

        var identityCfg =
            configs.FirstOrDefault(x => x is IdentityChannelConfig) as IdentityChannelConfig ??
            throw new ArgumentException("IdentityBalancerConfig not found");
        var bookingCfg =
            configs.FirstOrDefault(x => x is BookingChannelConfig) as BookingChannelConfig ??
            throw new ArgumentException("BookingBalancerConfig not found");
        var checkCfg =
            configs.FirstOrDefault(x => x is VehicleCheckChannelConfig) as VehicleCheckChannelConfig ??
            throw new ArgumentException("VehicleCheckBalancerConfig not found");
        var drivingLicenseCfg =
            configs.FirstOrDefault(x => x is DrivingLicenseChannelConfig) as DrivingLicenseChannelConfig ??
            throw new ArgumentException("DrivingLicenseBalancerConfig not found");
        var rentCfg =
            configs.FirstOrDefault(x => x is RentChannelConfig) as RentChannelConfig ??
            throw new ArgumentException("RentBalancerConfig not found");
        var vehicleFleetCfg =
            configs.FirstOrDefault(x => x is VehicleFleetChannelConfig) as VehicleFleetChannelConfig ??
            throw new ArgumentException("VehicleFleetBalancerConfig not found");

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