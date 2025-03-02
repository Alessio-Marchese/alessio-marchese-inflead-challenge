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
    //Mentre invece se la query si aspetta un risultato preciso possiamo fare una ricerca nel DB locale per vedere se é giá presente e tentare di risparmiare la chiamata all'EXAPI
    //Questo é possibile con EMAIL e USERNAME che sono parametri univoci
    bool isSingleResult = false;

    //Costruiamo la query per filtrare i dati presenti nel DB
    IQueryable<User> query = dbContext.Users.Include(u => u.Address).AsQueryable();

    //La chiamata all'endpoint dará in output un singolo risultato solo quando nella query di filtraggio é presente l'email o la password
    //mentre invece se si filtra per gender o si invia la chiamata senza filtri questa dará in output piú risultati e quindi isSingleResult rimarrá 'false'
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
        //Esegue la query e filtra i dati del DB
        List<User> users = query.ToList();
        //Se la query torna una lista non vuota significa che ha trovato il dato che cerchiamo
        if(!users.IsNullOrEmpty())
        {
            return Results.Ok(UserMapper.DbToMyApiDTO(users[0]));
        }
        else
        {
            //Altrimenti (se l'utente non é presente nel DB dobbiamo recuperarlo dall'EXAPI)
            var response = await httpClient.GetAsync("https://random-data-api.com/api/users/random_user?size=10");

            var exapiUsers = await response.Content.ReadFromJsonAsync<List<ExapiUserDTO>>();

            if(exapiUsers is null)
            {
                return Results.NotFound();
            }

            //Costruiamo la query di filtraggio per la lista di dati che ci arriva dall'EXAPI
            IQueryable<ExapiUserDTO> queryForExapiList = exapiUsers.AsQueryable();

            //Questa volta filtriamo solo email e password o eventualmente entrambi. Possiamo dare per scontato  
            //che ci possono essere solo questi due filtri dato che ci troviamo nell'if del risultato singolo
            if (!String.IsNullOrEmpty(email))
                queryForExapiList = queryForExapiList.Where(u => u.Email.Equals(email));
            if (!String.IsNullOrEmpty(username))
                queryForExapiList = queryForExapiList.Where(u => u.Username.Equals(username));

            //Prima di lavorare sul risultato controlliamo se questo é null o vuoto.
            //L'EXAPI ci da 10 utenti random, tra questi potrebbe non esserci il dato che cerchiamo
            var filteredExapiUser = queryForExapiList.ToList().IsNullOrEmpty() ? null : queryForExapiList.ToList()[0];

            if(filteredExapiUser is null)
            {
                return Results.NotFound();
            }
            
            //Mappiamo l'utente da Exapi a Db
            var mappedDbUser = UserMapper.ExapiToDb(filteredExapiUser);

            //Salviamo l'indirizzo e l'utente nel nostro db locale
            dbContext.Addresses.Add(mappedDbUser.Address);
            dbContext.Users.Add(mappedDbUser);
            dbContext.SaveChanges();

            //Mappiamo da exapi a myApi per mostrato il risultato nel form che ci é stato chiesto
            var mappedMyApiUser = UserMapper.ExapiToMyApiDTO(filteredExapiUser);
            mappedMyApiUser.AddressId = mappedDbUser.Address.Id.ToString();
            return Results.Ok(mappedMyApiUser);
        }
    }
    else
    {
        //Altrimenti (se non si tratta di un risultato singolo)

        //Inviamo direttamente la chiamata all'exapi
        var response = await httpClient.GetAsync("https://random-data-api.com/api/users/random_user?size=10");

        //Ho notato che alcune volte ricevo una risposta con content type diverso da JSON, ho pensato di anticipare l'errore che darebbe la riga 126 ritornando un codice di errore
        if (response.Content.Headers.ContentType == null || response.Content.Headers.ContentType.MediaType == null || !response.Content.Headers.ContentType.MediaType.Equals("application/json"))
        {
            return Results.UnprocessableEntity();
        }

        //Converto il JSON in exapiUsers
        var exapiUsers = await response.Content.ReadFromJsonAsync<List<ExapiUserDTO>>();

        //Se non riceviamo dati ritorna not found
        if (exapiUsers.IsNullOrEmpty() || exapiUsers is null)
        {
            return Results.NotFound();
        }

        List<MyApiUserDTO> mappedMyApiUsers = [];


        foreach (var exapiUser in exapiUsers)
        {
            //Converte ogni exapiUser in exapiUsers
            var dbUser = UserMapper.ExapiToDb(exapiUser);

            //Lo salva nel DB
            dbContext.Addresses.Add(dbUser.Address);
            dbContext.Users.Add(dbUser);
            dbContext.SaveChanges();

            //Converte da exapiUser in myApiUser
            var mappedMyApiUser = UserMapper.ExapiToMyApiDTO(exapiUser);
            mappedMyApiUsers.Add(mappedMyApiUser);
            mappedMyApiUser.AddressId = dbUser.Address.Id.ToString();
        }

        //L'unico filtro che possiamo aspettarci é quello del gender, quindi controliamo se é presente ed eventualmente lo eseguiamo
        if (!String.IsNullOrEmpty(gender))
        {
            return Results.Ok(mappedMyApiUsers.Where(u => u.Gender.Equals(gender)));
        }

        //Ritorna gli usenti
        return Results.Ok(mappedMyApiUsers);
    }
});

app.Run();

