using Backend.Data;
using Backend.Repository;
using Backend.Repository.Implement;
using Backend.Repository.Interface;
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

//Scoped per evitare errori di concorrenza quando si accede ai dati simultaneamente
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();

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

