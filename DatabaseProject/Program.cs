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

String connectionString = builder.Configuration.GetConnectionString("mySqlConn")!;
MySqlConnection connection = new MySqlConnection(connectionString);
connection.Open();
MySqlCommand command = connection.CreateCommand();
command.CommandText = "SELECT artist_name FROM Artist";
MySqlDataReader reader = command.ExecuteReader();

while (reader.Read())
{
    Console.WriteLine("Artist Name: " + reader[0]);
}

reader.Close();
connection.Close();
app.Run();