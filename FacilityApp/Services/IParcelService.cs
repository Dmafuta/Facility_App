using FacilityApp.Data.Models;

namespace FacilityApp.Services;

public interface IParcelService
{
    Task<List<Parcel>> GetAllAsync(ParcelStatus? status = null);
    Task<List<Parcel>> GetForUnitAsync(Guid unitId);
    Task<Parcel> ReceiveAsync(Guid? unitId, string recipientName, string? courierName, string description, string receivedById, string? notes);
    Task MarkCollectedAsync(Guid id, string collectedByName);
    Task MarkReturnedAsync(Guid id);
    Task<int> GetPendingCountAsync();
}
