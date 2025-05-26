namespace Clients.Config;

public class ClientChannelConfig
{
    public required Uri Uri { get; init; }
    public required string ApiKey { get; init; }
}

public class IdentityChannelConfig : ClientChannelConfig;

public class BookingChannelConfig : ClientChannelConfig;

public class VehicleCheckChannelConfig : ClientChannelConfig;

public class DrivingLicenseChannelConfig : ClientChannelConfig;

public class RentChannelConfig : ClientChannelConfig;

public class VehicleFleetChannelConfig : ClientChannelConfig;