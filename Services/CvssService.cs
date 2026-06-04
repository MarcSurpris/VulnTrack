namespace VulnTrack.Services;

// Pure business logic: turn a CVSS score into a severity label and an SLA deadline.
// CVSS v3.1 severity bands are an industry standard (FIRST.org).
public static class CvssService
{
    public static string GetSeverity(double score) => score switch
    {
        >= 9.0 => "Critical",
        >= 7.0 => "High",
        >= 4.0 => "Medium",
        > 0.0  => "Low",
        _      => "None"
    };

    // Stricter SLA for more severe findings.
    public static DateTime GetSlaDueDate(string severity) => severity switch
    {
        "Critical" => DateTime.UtcNow.AddDays(7),
        "High"     => DateTime.UtcNow.AddDays(30),
        "Medium"   => DateTime.UtcNow.AddDays(90),
        _          => DateTime.UtcNow.AddDays(180)
    };
}
