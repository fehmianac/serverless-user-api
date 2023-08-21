using System.Reflection;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.Configuration.SystemsManager;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SimpleNotificationService;
using Api.Extensions;
using Api.Infrastructure.Context;
using Domain.Options;
using Domain.Providers;
using Domain.Repositories;
using Domain.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Providers;
using Infrastructure.Repositories;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddSystemsManager(config =>
{
    config.Path = "/user-api";
    config.ParameterProcessor = new JsonParameterProcessor();
    config.ReloadAfter = TimeSpan.FromMinutes(5);
    config.Optional = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<SmsSettings>(builder.Configuration.GetSection("SmsSettings"));
builder.Services.Configure<UniqueKeySettings>(builder.Configuration.GetSection("UniqueKeySettings"));
builder.Services.Configure<UserVerificationSettings>(builder.Configuration.GetSection("UserVerificationSettings"));
builder.Services.Configure<EventBusSettings>(builder.Configuration.GetSection("EventBusSettings"));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddScoped<IApiContext, ApiContext>();
builder.Services.AddScoped<IEventBusManager, EventBusManager>();
builder.Services.AddScoped<ILookupRepository, LookupRepository>();
builder.Services.AddScoped<IOtpCodeRepository, OtpCodeRepository>();
builder.Services.AddScoped<IUniqueKeyRepository, UniqueKeyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVerifyLogRepository, VerifyLogRepository>();
builder.Services.AddScoped<ISmsProvider, DummySmsProvider>();
builder.Services.AddScoped<IMailProvider, DummyEmailProvider>();
builder.Services.AddScoped<IUserVerificationService, UserVerificationService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddAWSService<IAmazonSimpleNotificationService>();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
builder.Services.AddDefaultAWSOptions(new AWSOptions
{
    Profile = "serverless",
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEndpointsCore(AppDomain.CurrentDomain.GetAssemblies());

app.Run();

static IEnumerable<Assembly> GetAssembly()
{
    yield return typeof(Program).Assembly;
}