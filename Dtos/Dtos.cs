using System.ComponentModel.DataAnnotations;

namespace VulnTrack.Dtos;

// DTOs define what a request body must contain.
// The [Required]/[Range] attributes auto-validate input — rejecting bad data
// before it reaches your logic (defense in depth).

public class RegisterDto
{
    [Required, MinLength(3)]
    public string Username { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "Analyst";
}

public class LoginDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class CreateVulnerabilityDto
{
    [Required, MinLength(3)]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Range(0.0, 10.0)]
    public double CvssScore { get; set; }

    public string? AssignedTo { get; set; }
}
