using Backend.Data;
using Backend.Entities;
using Backend.ExternalApiClients.Implements;
using Backend.ExternalApiClients.Interfaces;
using Backend.Mappers.Implements;
using Backend.Mappers.Interfaces;
using Backend.Repository.Implements;
using Backend.Repository.Interfaces;
using Backend.Services.Implements;
using Backend.Services.Interfaces;
using Backend.Utility;
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

builder.Services.AddControllers();

builder.Services.AddSingleton<IUserMapper, UserMapper>();
builder.Services.AddSingleton<IAddressMapper, AddressMapper>();

//Scoped per evitare errori di concorrenza quando si accede ai dati simultaneamente

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAddressService, AddressService>();

builder.Services.AddScoped<IUserExapiClient, UserExapiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

