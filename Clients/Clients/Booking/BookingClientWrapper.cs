using Api;
using Grpc.Core;

namespace Clients.Clients.Booking;

public class BookingClientWrapper(Api.Booking.BookingClient client)
{
    public async Task<BookVehicleResponse> BookVehicle(BookVehicleRequest request, CancellationToken ct = default)
    {
        return await client.BookVehicleAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<CancelBookingResponse> CancelBooking(CancelBookingRequest request, CancellationToken ct = default)
    {
        return await client.CancelBookingVehicleAsync(request, new CallOptions([], cancellationToken: ct));
    }
}