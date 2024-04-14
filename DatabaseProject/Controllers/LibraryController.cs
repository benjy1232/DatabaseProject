using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;

namespace DatabaseProject.Controllers;

// LibraryController is basically the Song Controller
public class LibraryController(ILogger<LibraryController> logger, IConfiguration configuration) : Controller
{
    // GET
    public IActionResult Index()
    {
        CreateSongsTable(null);
        CreateAllLists();
        return View();
    }

    [Route("Library/Index/{artistName}")]
    public IActionResult Index(string artistName)
    {
        CreateSongsTable(artistName);
        CreateAllLists();
        return View();
    }

    private void CreateSongsTable(string? artistName)
    {
        string result = "";
        // Okay using raw SQL here bc no user input is needed - no security issue here
        try
        {
            using var connection = new MySqlConnection(configuration.GetConnectionString("mySqlConn"));
            connection.Open();
            string commandString = """
                                   SELECT song_title, A.album_name, A.release_year, M.artist_name, G.genre_name, AA.artist_name FROM Song
                                   INNER JOIN Album A ON A.id = album_id
                                   INNER JOIN Artist M ON M.id = Song.artist_id
                                   INNER JOIN Artist AA ON AA.id = A.artist_id
                                   LEFT JOIN Genre G ON G.id = Song.genre_id
                                   """;
            using var command = connection.CreateCommand();
            if (!string.IsNullOrEmpty(artistName))
            {
                commandString += " WHERE AA.artist_name = @artistName OR M.artist_name = @artistName";
                command.CommandText = commandString;
                command.Parameters.AddWithValue("artistName", artistName);
                command.Prepare();
            }
            else
            {
                command.CommandText = commandString;
            }
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                // Genre should be N/A when string is null or empty - otherwise return the value of reader[4].ToString()
                string genre = string.IsNullOrEmpty(reader[4].ToString()) ? "N/A" : reader[4].ToString()!;
                result += $"""
                           <tr>
                             <td>{reader[0]}</td>
                             <td>{reader[1]}</td>
                             <td>{reader[2]}</td>
                             <td>{reader[5]}</td>
                             <td>{reader[3]}</td>
                             <td>{genre}</td>
                           </tr>
                           """;
            }

            reader.Close();
        }
        catch (MySqlException ex)
        {
            logger.LogError($"Error {ex.Number} has occurred: {ex.Message}");
            throw;
        }

        ViewBag.Table = result;
    }

    public IActionResult AddSong(string songName, uint albumId, uint artistId, uint? genreId)
    {
        try
        {
            using var connection = new MySqlConnection(configuration.GetConnectionString("mySqlConn"));
            connection.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = """
                              INSERT INTO Song (song_title, artist_id, album_id, genre_id) VALUES
                              (@songName, @artistId, @albumId, @genreId)
                              """;
            cmd.Parameters.AddWithValue("@songName", songName);
            cmd.Parameters.AddWithValue("@albumId", albumId);
            cmd.Parameters.AddWithValue("@artistId", artistId);
            cmd.Parameters.AddWithValue("@genreId", genreId);
            cmd.Prepare();

            cmd.ExecuteNonQuery();
        }
        catch (MySqlException ex)
        {
            if (ex.Number == 1065)
            {
                logger.LogError($"Entry {songName} - {albumId} already exists");
                return RedirectToAction("Index");
            }

            logger.LogError($"Error {ex.Number} has occurred: {ex.Message}");
            throw;
        }
        return RedirectToAction("Index");
    }

    public IActionResult DeleteSong(uint songId)
    {
        try
        {
            using var connection = new MySqlConnection(configuration.GetConnectionString("mySqlConn"));
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Song WHERE id = @id";
            command.Parameters.AddWithValue("@id", songId);
            command.Prepare();

            command.ExecuteNonQuery();
        }
        catch (MySqlException ex)
        {
            logger.LogError($"Error {ex.Number} has occurred: {ex.Message}");
            throw;
        }

        return RedirectToAction("Index");
    }

    public IActionResult UpdateSong(uint songId, string? songName, uint? albumId, uint? artistId, ushort? genreId, bool updateGenre)
    {
        try
        {
            using var connection = new MySqlConnection(configuration.GetConnectionString("mySqlConn"));
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT song_title, album_id, artist_id, genre_id FROM Song WHERE id = @id";
            command.Parameters.AddWithValue("@id", songId);
            command.Prepare();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                songName = string.IsNullOrEmpty(songName) ? reader[0].ToString()! : songName;
                albumId ??= reader.GetUInt32(1);
                artistId ??= reader.GetUInt32(2);
                genreId ??= reader.GetUInt16(3);
            }
            reader.Close();


            string commandStr;
            if (updateGenre)
            {
                commandStr = """
                             UPDATE Song
                             SET song_title = @songName, album_id = @albumId, artist_id = @artistId, genre_id = @genreId
                             WHERE id = @id
                             """;     
            }
            else
            {
                commandStr = """
                             UPDATE Song
                             SET song_title = @songName, album_id = @albumId, artist_id = @artistId
                             WHERE id = @id
                             """;
            }

            command.CommandText = commandStr;
            command.Parameters.AddWithValue("@songName", songName);
            command.Parameters.AddWithValue("@albumId", albumId);
            command.Parameters.AddWithValue("@artistId", artistId);
            if (updateGenre)
                command.Parameters.AddWithValue("@genreId", genreId);

            command.ExecuteNonQuery();
            logger.LogInformation("Command Successful");
        }
        catch (MySqlException ex)
        {
            if (ex.Number == 1065)
            {
                logger.LogError($"Entry {songName} - {albumId} already exists");
                return RedirectToAction("Index");
            }
            logger.LogError($"Error {ex.Number} has occurred: {ex.Message}");
            throw;
        }
        return RedirectToAction("Index");
    }

    private void CreateAllLists()
    {
        try
        {
            using var connection = new MySqlConnection(configuration.GetConnectionString("mySqlConn"));
            connection.Open();
            ViewBag.Artists = GenerateList(connection, "Artist");
            ViewBag.Genres = GenerateList(connection, "Genre");
            List<SelectListItem> albums = [];

            using var albumCmd = connection.CreateCommand();
            albumCmd.CommandText = """
                                   SELECT Album.id, album_name, A.artist_name FROM Album
                                   INNER JOIN Artist A ON A.id = artist_id
                                   """;
            using var albumReader = albumCmd.ExecuteReader();
            while (albumReader.Read())
            {
                albums.Add(new SelectListItem($"{albumReader[2]} - {albumReader[1]}", albumReader[0].ToString()));
            }
            albumReader.Close();

            ViewBag.Albums = albums;

            List<SelectListItem> songs = [];
            using var songCmd = connection.CreateCommand();
            songCmd.CommandText = """
                                   SELECT Song.id, song_title, A.album_name FROM Song
                                   INNER JOIN Album A ON A.id = album_id
                                   """;
            using var songReader = songCmd.ExecuteReader();
            while (songReader.Read())
            {
                songs.Add(new SelectListItem($"{songReader[1]} - {songReader[2]}", songReader[0].ToString()));
            }            ViewBag.Songs = songs;
        }
        catch (MySqlException ex)
        {
            logger.LogError($"Error {ex.Number} has occurred: {ex.Message}");
            throw;
        }
    }

    private List<SelectListItem> GenerateList(MySqlConnection connection, string table)
    {
        List<SelectListItem> list = [];
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {table}";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new SelectListItem(reader[1].ToString(), reader[0].ToString()));
        }

        reader.Close();
        return list;
    }
}