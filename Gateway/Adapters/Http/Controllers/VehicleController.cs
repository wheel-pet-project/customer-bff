using Clients.Clients.VehicleFleet;
using Gateway.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;
using Proto.VehicleFleetV1;
using Color = OpenApiContractV1.Models.Color;
using Location = OpenApiContractV1.Models.Location;
using Role = Proto.IdentityV1.Role;
using Status = OpenApiContractV1.Models.Status;

namespace Gateway.Adapters.Http.Controllers;

[Authorization(Role.CustomerUnspecified)]
public class VehicleController(VehicleFleetClientWrapper clientWrapper) : VehicleApiController
{
    public override async Task<IActionResult> GetVehicleById(Guid vehicleId)
    {
        var statusEnumMapper = new VehicleStatusEnumMapper();
        var colorEnumMapper = new ColorEnumMapper();
        
        var serviceResponse = await clientWrapper.GetVehicleById(new GetVehicleByIdReq { VehicleId = vehicleId.ToString() });

        return Ok(MapToResponse(serviceResponse));

        GetVehicleByIdResponse MapToResponse(GetVehicleByIdRes grpcResponse)
        {
            return new GetVehicleByIdResponse
            {
                VehicleId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.VehicleId),
                Status = statusEnumMapper.ToContract(grpcResponse.Status),
                Brand = grpcResponse.Brand,
                Color = colorEnumMapper.ToContract(grpcResponse.Color),
                CarModel = grpcResponse.CarModel,
                PlateNumber = grpcResponse.PlateNumber,
                FuelLevelPercents = grpcResponse.FuelLevelPercents,
                Location = new Location
                    { Latitude = grpcResponse.Location.Latitude, Longitude = grpcResponse.Location.Longitude },
                PricePerMinute = new decimal(grpcResponse.PricePerMin),
                PricePerHour = new decimal(grpcResponse.PricePerHour)
            };
        }
    }

    public override async Task<IActionResult> GetVehiclesInSquare(GetVehiclesInSquareRequest request)
    {
        var colorMapper = new ColorEnumMapper();
        var statusMapper = new VehicleStatusEnumMapper();
        
        
        var serviceResponse = await clientWrapper.GetVehiclesInSquare(new GetVehiclesInSquareReq
        {
            FilteringStatus = statusMapper.ToProto(request.Status),
            UpperLeftLocation = new Proto.VehicleFleetV1.Location
                { Latitude = request.UpperLeftLocation.Latitude, Longitude = request.UpperLeftLocation.Longitude },
            LowerRightLocation = new Proto.VehicleFleetV1.Location
                { Latitude = request.LowerRightLocation.Latitude, Longitude = request.LowerRightLocation.Longitude }
        });

        return Ok(MapToResponse(serviceResponse));
        
        GetVehiclesInSquareResponse MapToResponse(GetVehiclesInSquareRes grpcResponse)
        {
            return new GetVehiclesInSquareResponse
            {
                Vehicles = grpcResponse.Vehicles
                    .Select(x => new VehicleInSquareShortView
                    {
                        VehicleId = Guid.Parse((ReadOnlySpan<char>)x.VehicleId),
                        Brand = x.Brand,
                        CarModel = x.CarModel,
                        Color = colorMapper.ToContract(x.Color),
                        Location = new Location{Latitude = x.Location.Latitude, Longitude = x.Location.Longitude}
                    }).ToList()
            };
        }
    }
}

class VehicleStatusEnumMapper
{
    public Status ToContract(Proto.VehicleFleetV1.Status s)
    {
        return s switch
        {
            Proto.VehicleFleetV1.Status.AddingInProgress => Status.AddingInProgressEnum,
            Proto.VehicleFleetV1.Status.Added => Status.AddedEnum,
            Proto.VehicleFleetV1.Status.NotAdded => Status.NotAddedEnum,
            Proto.VehicleFleetV1.Status.ReadiedForRelease => Status.ReadiedForReleaseEnum,
            Proto.VehicleFleetV1.Status.ReleasedUnspecified => Status.ReleasedEnum,
            Proto.VehicleFleetV1.Status.Occupied => Status.OccupiedEnum,
            Proto.VehicleFleetV1.Status.Serviced => Status.ServicedEnum,
            Proto.VehicleFleetV1.Status.Deleted => Status.DeletedEnum,
            _ => throw new ArgumentOutOfRangeException(nameof(s), s, null)
        };
    }

    public Proto.VehicleFleetV1.Status ToProto(Status s)
    {
        return s switch
        {
            Status.AddingInProgressEnum => Proto.VehicleFleetV1.Status.AddingInProgress,
            Status.AddedEnum => Proto.VehicleFleetV1.Status.Added,
            Status.NotAddedEnum => Proto.VehicleFleetV1.Status.NotAdded,
            Status.ReadiedForReleaseEnum => Proto.VehicleFleetV1.Status.ReadiedForRelease,
            Status.ReleasedEnum => Proto.VehicleFleetV1.Status.ReleasedUnspecified,
            Status.OccupiedEnum => Proto.VehicleFleetV1.Status.Occupied,
            Status.ServicedEnum => Proto.VehicleFleetV1.Status.Serviced,
            Status.DeletedEnum => Proto.VehicleFleetV1.Status.Deleted,
            _ => throw new ArgumentOutOfRangeException(nameof(s), s, null)
        };
    }
}

public class ColorEnumMapper
{
    public Color ToContract(Proto.VehicleFleetV1.Color c)
    {
        return c switch
        {
            Proto.VehicleFleetV1.Color.WhiteUnspecified => Color.WhiteEnum,
            Proto.VehicleFleetV1.Color.Grey => Color.GreyEnum,
            Proto.VehicleFleetV1.Color.Black => Color.BlackEnum,
            Proto.VehicleFleetV1.Color.Blue => Color.BlueEnum,
            Proto.VehicleFleetV1.Color.Red => Color.RedEnum,
            Proto.VehicleFleetV1.Color.Yellow => Color.YellowEnum,
            Proto.VehicleFleetV1.Color.Orange => Color.OrangeEnum,
            Proto.VehicleFleetV1.Color.Green => Color.GreenEnum,
            Proto.VehicleFleetV1.Color.Beige => Color.BeigeEnum,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    }
}