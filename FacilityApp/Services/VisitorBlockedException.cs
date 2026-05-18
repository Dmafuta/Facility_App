using FacilityApp.Data.Models;

namespace FacilityApp.Services;

public class VisitorBlockedException : Exception
{
    public BlacklistEntry Entry { get; }

    public VisitorBlockedException(BlacklistEntry entry)
        : base($"Visitor is {entry.EntryType}: {entry.Reason}")
    {
        Entry = entry;
    }
}
