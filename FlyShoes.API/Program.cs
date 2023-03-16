using Firebase.Auth;
using FirebaseAdmin;
using FlyShoes.API.FirebaseHandler;
using FlyShoes.API.MiddleWareHandler;
using FlyShoes.DAL.Implements;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers((o) =>
{
    o.Filters.Add<HttpResponseFilter>();
});
builder.Services.AddSingleton(FirebaseApp.Create());
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<AuthenticationSchemeOptions,FirebaseAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme,(o) => { });

builder.Services.AddScoped<IFirebaseAuthClient,FirebaseAuthService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
