using Firebase.Auth;
using FirebaseAdmin;
using FlyShoes.API.FirebaseHandler;
using FlyShoes.API.MiddleWareHandler;
using FlyShoes.BL;
using FlyShoes.BL.Base;
using FlyShoes.BL.Implements;
using FlyShoes.BL.Interfaces;
using FlyShoes.Core.Implements;
using FlyShoes.Core.Interfaces;
using FlyShoes.DAL.Implements;
using FlyShoes.DAL.Interfaces;
using FlyShoes.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IFirebaseAuthClient, FirebaseAuthService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IFirestoreService, FirestoreService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBrandBL, BrandBL>();
builder.Services.AddScoped<ICartDetailBL, CartDetailBL>();
builder.Services.AddScoped<ICategoryBL, CategoryBL>();
builder.Services.AddScoped<IColorBL, ColorBL>();
builder.Services.AddScoped<IEmailTemplateBL, EmailTemplateBL>();
builder.Services.AddScoped<IOrderBL, OrderBL>();
builder.Services.AddScoped<IOrderDetailBL, OrderDetailBL>();
builder.Services.AddScoped<IRankCustomerBL, RankCustomerBL>();
builder.Services.AddScoped<IShoesBL, ShoesBL>();
builder.Services.AddScoped<IShoesDetailBL, ShoesDetailBL>();
builder.Services.AddScoped<ISizeBL, SizeBL>();
builder.Services.AddScoped<IUserBL, UserBL>();
builder.Services.AddScoped<IVoucherBL, VoucherBL>();
builder.Services.AddScoped(typeof(IBaseBL<>), typeof(BaseBL<>));


// Add services to the container.

builder.Services.AddControllers((o) =>
{
    o.Filters.Add<HttpResponseFilter>();
});

builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.UseMemberCasing();
});

builder.Services.AddSingleton(FirebaseApp.Create());
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<AuthenticationSchemeOptions,FirebaseAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme,(o) => { });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors(opt => opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

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
