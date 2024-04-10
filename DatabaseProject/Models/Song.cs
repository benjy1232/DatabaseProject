namespace DatabaseProject.Models;

public record struct Song(uint Id, string SongTitle, uint ArtistId, uint AlbumId, uint GenreId);