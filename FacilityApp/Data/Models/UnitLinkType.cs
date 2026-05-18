namespace FacilityApp.Data.Models;

public enum UnitLinkType
{
    /// <summary>User owns this unit.</summary>
    Owner,

    /// <summary>User physically lives in this unit. Grants visitor pre-registration.</summary>
    Occupant
}
