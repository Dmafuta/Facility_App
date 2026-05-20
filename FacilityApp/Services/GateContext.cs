namespace FacilityApp.Services;

public class GateContext
{
    public Guid? EntranceId { get; set; }
    public string EntranceName { get; set; } = "";
    public bool IsSelected => EntranceId.HasValue && EntranceId.Value != Guid.Empty;
}
