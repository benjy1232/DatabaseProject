using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Cms;

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
                                   SELECT song_title, A.album_name, A.release_year, M.artist_name, G.name, AA.artist_name FROM Song
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
                string genre = reader[4].ToString() ?? "N/A";
                genre = genre.Length == 0 ? "N/A" : genre;
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
                                   SELECT id, album_name, A.artist_name FROM Album
                                   """;
            using var albumReader = albumCmd.ExecuteReader();
            while (albumReader.Read())
            {
                albums.Add(new SelectListItem($"{albumReader[2]} - {albumReader[1]}", albumReader[0].ToString()));
            }

            ViewBag.Albums = albums;
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