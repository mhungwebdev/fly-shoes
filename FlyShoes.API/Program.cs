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
builder.Services.AddTransient<IFirebaseAuthClient, FirebaseAuthService>();
builder.Services.AddTransient<IDatabaseService, DatabaseService>();
builder.Services.AddTransient<IStorageService, StorageService>();
builder.Services.AddTransient<IFirestoreService, FirestoreService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IBrandBL, BrandBL>();
builder.Services.AddTransient<ICartDetailBL, CartDetailBL>();
builder.Services.AddTransient<ICategoryBL, CategoryBL>();
builder.Services.AddTransient<IColorBL, ColorBL>();
builder.Services.AddTransient<IEmailTemplateBL, EmailTemplateBL>();
builder.Services.AddTransient<IOrderShoesBL, OrderShoesBL>();
builder.Services.AddTransient<IOrderDetailBL, OrderDetailBL>();
builder.Services.AddTransient<IShoesBL, ShoesBL>();
builder.Services.AddTransient<IShoesDetailBL, ShoesDetailBL>();
builder.Services.AddTransient<ISizeBL, SizeBL>();
builder.Services.AddTransient<IUserBL, UserBL>();
builder.Services.AddTransient<IVoucherBL, VoucherBL>();
builder.Services.AddTransient<IVNPayService, VNPayService>();
builder.Services.AddTransient<IPaymentInfoBL, PaymentInfoBL>();
builder.Services.AddTransient<IReportBL, ReportBL>();
builder.Services.AddTransient(typeof(IBaseBL<>), typeof(BaseBL<>));


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
