namespace FacilityApp.Data.Models;

public enum UserType
{
    /// <summary>Rents/occupies a unit. Always an occupant.</summary>
    Resident,

    /// <summary>Owns one or more units. May also personally occupy a unit.</summary>
    HomeOwner,

    /// <summary>Facility employee — sub-classified by Identity Role.</summary>
    Staff
}
