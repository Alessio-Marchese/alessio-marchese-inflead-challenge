using Backend.Data;
using Backend.DTO.EXAPI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>((sp, o) =>
{
    o.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CodingExercise;Trusted_Connection=True");
});

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/user/filtered", async (string? gender, string? email, string? username, HttpClient httpClient) =>
{
    var response = await httpClient.GetAsync("https://random-data-api.com/api/users/random_user?size=10");

    var users = await response.Content.ReadFromJsonAsync<List<UserDTO>>();

    foreach (var user in users)
    {
        Console.WriteLine($"{user.Username} {user.Uid} {user.Id} {user.Email} {user.Username} {user.FirstName} {user.LastName} {user.ProfilePicUrl} {user.Gender} {user.PhoneNumber} {user.Employment.ToString()} {user.KeySkill} {user.Address.ToString()}");
    }
        

    List<string> parameters = new();
    if (gender is not null)
        parameters.Add(gender);
    if (email is not null)
        parameters.Add(email);
    if (username is not null)
        parameters.Add(username);
});

app.Run();

