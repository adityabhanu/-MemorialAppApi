namespace MemorialAppApi.Core.Entities;

public class Burial
{
    public Guid Id { get; set; }
    public string? BurialType { get; set; }
    public string? PlotNumber { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public string? Inscription { get; set; }
    public string? Gravesite { get; set; }
    public bool Cenotaph { get; set; }
    public bool Monument { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
