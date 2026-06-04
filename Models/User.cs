namespace VulnTrack.Models;

// Maps to the "Users" table. Stores the hashed password, never plaintext.
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;  // BCrypt hash
    public string Role { get; set; } = "Analyst";             // Analyst / Admin / Auditor
}
