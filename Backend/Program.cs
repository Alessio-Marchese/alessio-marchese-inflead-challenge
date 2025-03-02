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

    IQueryable<User> query = dbContext.Users.Include(u => u.Address).AsQueryable();

    if (!String.IsNullOrEmpty(gender))
        query = query.Where(u => u.Gender.Equals(gender));
    if (!String.IsNullOrEmpty(email))
    {
        query = query.Where(u => u.Email.Equals(email));
        isSingleResult = true;
    }   
    if (!String.IsNullOrEmpty(username))
    {
        query = query.Where(u => u.Username.Equals(username));
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
                queryForExapiList = queryForExapiList.Where(u => u.Email.Equals(email));
            if (!String.IsNullOrEmpty(username))
                queryForExapiList = queryForExapiList.Where(u => u.Username.Equals(username));

            var filteredUser = queryForExapiList.ToList().IsNullOrEmpty() ? null : queryForExapiList.ToList()[0];

            if(filteredUser is null)
            {
                return Results.NotFound();
            }
            
            var mappedDbUser = UserMapper.ExapiToDb(filteredUser);

            var dbUsers = dbContext.Users.ToList();

            var address = new Address()
            {
                City = mappedDbUser.Address.City,
                Street = mappedDbUser.Address.Street,
                ZipCode = mappedDbUser.Address.ZipCode,
                State = mappedDbUser.Address.State
            };
            var dbAddresses = dbContext.Addresses.ToList();

            dbContext.Addresses.Add(address);
            mappedDbUser.Address = address;
            dbContext.Users.Add(mappedDbUser);
            dbContext.SaveChanges();

            var myApiUser = UserMapper.ExapiToMyApiDTO(filteredUser);
            myApiUser.AddressId = address.Id.ToString();
            return Results.Ok(myApiUser);
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

        if (exapiUsers.IsNullOrEmpty() || exapiUsers is null)
        {
            return Results.NotFound();
        }

        List<MyApiUserDTO> mappedMyApiUsers = [];

        foreach (var exapiUser in exapiUsers)
        {
            var dbUser = UserMapper.ExapiToDb(exapiUser);
            var address = new Address()
            {
                City = exapiUser.Address.City,
                Street = $"{exapiUser.Address.StreetName} {exapiUser.Address.StreetAddress}",
                ZipCode = exapiUser.Address.ZipCode,
                State = exapiUser.Address.State
            };
            var dbAddresses = dbContext.Addresses.ToList();

            dbContext.Addresses.Add(address);
            dbUser.Address = address;
            dbContext.Users.Add(dbUser);
            dbContext.SaveChanges();
            var mappedMyApiUser = UserMapper.DbToMyApiDTO(dbUser);
            mappedMyApiUsers.Add(mappedMyApiUser);
            mappedMyApiUser.AddressId = address.Id.ToString();
        }

        

        if (!String.IsNullOrEmpty(gender))
        {
            return Results.Ok(mappedMyApiUsers.Where(u => u.Gender.Equals(gender)));
        }
        return Results.Ok(mappedMyApiUsers);
    }
});

app.Run();

