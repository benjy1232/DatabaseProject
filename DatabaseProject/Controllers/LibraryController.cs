using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace DatabaseProject.Controllers;

// LibraryController is basically the Song Controller
public class LibraryController(ILogger<LibraryController> logger, IConfiguration configuration) : Controller
{
    // GET
    public IActionResult Index()
    {
        CreateSongsTable();
        return View();
    }

    private void CreateSongsTable()
    {
        string result = "";
        // Okay using raw SQL here bc no user input is needed - no security issue here
        string commandString = """
                               SELECT song_title, A.album_name, A.release_year, M.artist_name, G.name, AA.artist_name FROM Song
                               INNER JOIN Album A ON A.id = album_id
                               INNER JOIN Artist M ON M.id = Song.artist_id
                               INNER JOIN Artist AA ON AA.id = A.artist_id
                               LEFT JOIN Genre G ON G.id = Song.genre_id
                               """;
        try
        {
            using var connection = new MySqlConnection(configuration.GetConnectionString("mySqlConn"));
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = commandString;
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
        }

        ViewBag.Table = result;
    }


}