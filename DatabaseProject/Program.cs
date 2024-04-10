using MySql.Data.MySqlClient;
using Org.BouncyCastle.Cms;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

if (builder.Configuration.GetConnectionString("mySqlConn") == null)
{
    return;
}

var connectionString = builder.Configuration.GetConnectionString("mySqlConn")!;
var connection = new MySqlConnection(connectionString);
connection.Open();
var command = connection.CreateCommand();
command.CommandText = """
                      SELECT song_title, A.album_name, A.release_year, M.artist_name FROM Song
                      INNER JOIN Album A ON A.id = album_id
                      INNER JOIN Artist M ON M.id = Song.artist_id
                      """;

var reader = command.ExecuteReader();

while (reader.Read())
{
    Console.WriteLine("Song Title: " + reader[0]);
    Console.WriteLine("Album Title: " + reader[1]);
    Console.WriteLine("Artist Name: " + reader[3]);
    Console.WriteLine("Release Year: " + reader[2]);
}

reader.Close();
connection.Close();
app.Run();