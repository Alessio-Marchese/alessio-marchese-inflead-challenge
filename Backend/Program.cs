using Backend.Data;
using Backend.DTO.EXAPI;
using Backend.DTO.MYAPI;
using Backend.Entities;
using Backend.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

app.MapGet("/api/user/filtered", async (string? gender, string? email, string? username, HttpClient httpClient, ApplicationDbContext dbContext) =>
{
    //Se la query di ricerca si aspetta piú risultati non possiamo risparmiarci la chiamata all'EXAPI, dato che corriamo il rischio di non mostrare tutti i dati
    //Mentre invece se la query si aspetta un solo risultato possiamo fare una ricerca nel DB locale per vedere se é giá presente e tentare di risparmiare la chiamata all'EXAPI
    //Questo é possibile con EMAIL e USERNAME che sono parametri univoci

    bool isSingleResult = false;

    IQueryable<User> query = dbContext.Users.AsQueryable();

    if (!String.IsNullOrEmpty(gender))
        query.Where(u => u.Gender.Equals(gender));
    if (!String.IsNullOrEmpty(email))
    {
        query.Where(u => u.Email.Equals(email));
        isSingleResult = true;
    }   
    if (!String.IsNullOrEmpty(username))
    {
        query.Where(u => u.Username.Equals(username));
        isSingleResult = true;
    }
        

    if (isSingleResult)
    {

        List<User> users = query.ToList();
        if(!users.IsNullOrEmpty())
        {
            return Results.Ok(UserMapper.DbToMyApiDTO(users[0]));
        }
        else
        {
            var response = await httpClient.GetAsync("https://random-data-api.com/api/users/random_user?size=10");

            var exapiUsers = await response.Content.ReadFromJsonAsync<List<ExapiUserDTO>>();

            if(exapiUsers is null)
            {
                return Results.NotFound();
            }

            IQueryable<ExapiUserDTO> queryForExapiList = exapiUsers.AsQueryable();

            if (!String.IsNullOrEmpty(email))
                query.Where(u => u.Email.Equals(email));
            if (!String.IsNullOrEmpty(username))
                query.Where(u => u.Username.Equals(username));

            var filteredUser = queryForExapiList.ToList()[0];

            var mappedDbUser = UserMapper.ExapiToDb(filteredUser);

            var dbUsers = dbContext.Users.ToList();

            if(!dbUsers.IsNullOrEmpty())
            {
                var lastUser = dbUsers.OrderBy(u => u.Id).Last();
                mappedDbUser.Id = lastUser.Id++;    
            }
            else
            {
                mappedDbUser.Id = 1;
            }
            dbContext.Users.Add(mappedDbUser);

            return Results.Ok(UserMapper.ExapiToMyApiDTO(filteredUser));
        }
    }
    else
    {
        var response = await httpClient.GetAsync("https://random-data-api.com/api/users/random_user?size=10");

        if (response.Content.Headers.ContentType == null || response.Content.Headers.ContentType.MediaType == null || !response.Content.Headers.ContentType.MediaType.Equals("application/json"))
        {
            return Results.UnprocessableEntity();
        }

        var exapiUsers = await response.Content.ReadFromJsonAsync<List<ExapiUserDTO>>();

        if (exapiUsers is null)
        {
            return Results.NotFound();
        }

        List<MyApiUserDTO> mappedMyApiUsers = [];

        if (!String.IsNullOrEmpty(gender))
        {
            var filteredExapiUsers = exapiUsers.Where(u => u.Gender.Equals(gender)).ToList();

            foreach (var filteredExapiUser in filteredExapiUsers)
            {
                mappedMyApiUsers.Add(UserMapper.ExapiToMyApiDTO(filteredExapiUser));
            }

            return Results.Ok(mappedMyApiUsers);
        }
        else
        {
            foreach(var exapiUser in exapiUsers)
            {
                mappedMyApiUsers.Add(UserMapper.ExapiToMyApiDTO(exapiUser));
            }
            return Results.Ok(mappedMyApiUsers);
        }
    }
});

app.Run();

