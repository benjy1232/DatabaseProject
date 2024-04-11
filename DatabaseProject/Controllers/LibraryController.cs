using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace DatabaseProject.Controllers;

public class LibraryController(ILogger<LibraryController> logger, IConfiguration configuration) : Controller
{
    private ILogger<LibraryController> _logger = logger;
    private IConfiguration _configuration = configuration;

    // GET
    public IActionResult Index()
    {
        CreateSongsTable();
        return View();
    }

    private void CreateSongsTable()
    {
        string result = "";
        var connection = new MySqlConnection(_configuration.GetConnectionString("mySqlConn"));
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = """
                              SELECT song_title, A.album_name, A.release_year, M.artist_name, G.name, AA.artist_name FROM Song
                              INNER JOIN Album A ON A.id = album_id
                              INNER JOIN Artist M ON M.id = Song.artist_id
                              INNER JOIN Artist AA ON AA.id = A.artist_id
                              LEFT JOIN Genre G ON G.id = Song.genre_id
                              """;
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            string genre;
            if (reader[4] == null || reader[4].ToString().Length == 0)
                genre = "NULL";
            else
            {
                genre = reader[4].ToString();
            }
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
        connection.Close();

        ViewBag.Table = result;
    }
}