namespace ReadingRoom.ArticleCMS.Core.Constants;

/// <summary>
/// Date formats used when an article's publish date is shown.
/// </summary>
public static class DateFormats
{
    /// <summary>
    /// The machine-readable form for the datetime attribute of a time element, for example 2026-11-25.
    /// </summary>
    public const string MachineReadable = "yyyy-MM-dd";

    /// <summary>
    /// The form readers see, for example 25 November 2026.
    /// </summary>
    public const string Display = "d MMMM yyyy";
}
