namespace Clients.Config;

public class MicroserviceBalancerConfig
{
    public Uri Uri { get; private set; }
    public string ApiKey { get; private set; }
}

public class IdentityBalancerConfig : MicroserviceBalancerConfig;

public class BookingBalancerConfig : MicroserviceBalancerConfig;

public class VehicleCheckBalancerConfig : MicroserviceBalancerConfig;

public class DrivingLicenseBalancerConfig : MicroserviceBalancerConfig;

public class RentBalancerConfig : MicroserviceBalancerConfig;

public class VehicleFleetBalancerConfig : MicroserviceBalancerConfig;