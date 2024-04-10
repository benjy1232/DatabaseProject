namespace DatabaseProject.Models;

public record struct Album(uint Id, string Name, ushort ReleaseYear, uint ArtistId);