using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace DatabaseProject.Controllers;

public class ArtistController(ILogger<ArtistController> logger, IConfiguration configuration) : Controller
{
    // GET
    public IActionResult Index()
    {
        CreateArtistsTable();
        return View();
    }
     private void CreateArtistsTable()
     {
         string result = "";
         // Okay using raw SQL here bc no user input is needed - no security issue here
         string commandString = "SELECT artist_name FROM Artist";
         try
         {
             using var connection = new MySqlConnection(configuration.GetConnectionString("mySqlConn"));
             connection.Open();
             using var command = connection.CreateCommand();
             command.CommandText = commandString;
             using var reader = command.ExecuteReader();
             while (reader.Read())
             {
                 result += $"""
                            <tr>
                              <td>{reader[0]}</td>
                            </tr>
                            """;
             }
         }
         catch (MySqlException ex)
         {
             logger.LogError($"Error {ex.Number} has occurred: {ex.Message}");
         }
 
         ViewBag.Table = result;
     }

    // Should be the same or similar for new album and new song
    public void AddNewArtist(string artistName)
    {
        try
        {
            using var connection = new MySqlConnection(configuration.GetConnectionString("mySqlConn"));
            connection.Open();
            using var command = connection.CreateCommand();
            // Don't add id because it is autogenerated
            command.CommandText = "INSERT INTO Artist (artist_name) VALUES (@artistName)";
            command.Parameters.AddWithValue("@artistName", artistName);
            command.Prepare();
            command.ExecuteNonQuery();
        }
        catch (MySqlException ex)
        {
            logger.LogError($"Error {ex.Number} has occurred: {ex.Message}");
            throw;
        }
    }

    public void DeleteArtist(string artistName)
    {
        try
        {
            using var connection = new MySqlConnection(configuration.GetConnectionString("mySqlConn"));
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Artist WHERE artist_name = @artistName";
            command.Parameters.AddWithValue("@artistName", artistName);
            command.Prepare();
            command.ExecuteNonQuery();
        }
        catch (MySqlException ex)
        {
            logger.LogError($"Error {ex.Number} has occurred: {ex.Message}");
            throw;
        }
    }

    public void UpdateArtist(string oldName, string newArtistName)
    {
        try
        {
            using var connection = new MySqlConnection(configuration.GetConnectionString("mySqlConn"));
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Artist SET artist_name = @newArtistName WHERE artist_name = @oldArtistName";
            command.Parameters.AddWithValue("@newArtistName", newArtistName);
            command.Parameters.AddWithValue("@oldArtistName", oldName);
            command.Prepare();
            command.ExecuteNonQuery();
        }
        catch (MySqlException ex)
        {
            logger.LogError($"Error {ex.Number} has occurred: {ex.Message}");
            throw;
        }
    }
}