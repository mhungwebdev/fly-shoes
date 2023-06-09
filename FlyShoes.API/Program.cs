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
using Google.Api;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using Google.Cloud.Firestore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IFirebaseAuthClient, FirebaseAuthService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddTransient<IFirestoreService, FirestoreService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBrandBL, BrandBL>();
builder.Services.AddScoped<ICartDetailBL, CartDetailBL>();
builder.Services.AddScoped<ICategoryBL, CategoryBL>();
builder.Services.AddScoped<IColorBL, ColorBL>();
builder.Services.AddScoped<IEmailTemplateBL, EmailTemplateBL>();
builder.Services.AddScoped<IOrderShoesBL, OrderShoesBL>();
builder.Services.AddScoped<IOrderDetailBL, OrderDetailBL>();
builder.Services.AddScoped<IShoesBL, ShoesBL>();
builder.Services.AddScoped<IShoesDetailBL, ShoesDetailBL>();
builder.Services.AddScoped<ISizeBL, SizeBL>();
builder.Services.AddScoped<IUserBL, UserBL>();
builder.Services.AddScoped<IVoucherBL, VoucherBL>();
builder.Services.AddScoped<IVNPayService, VNPayService>();
builder.Services.AddScoped<IPaymentInfoBL, PaymentInfoBL>();
builder.Services.AddScoped<IReportBL, ReportBL>();
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

var firebaseApp = FirebaseApp.Create(new AppOptions()
{
    Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile("./FirebaseConfig/firebase-config.json")
});
builder.Services.AddSingleton(firebaseApp);
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
