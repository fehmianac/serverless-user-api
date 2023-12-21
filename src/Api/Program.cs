using Amazon.DynamoDBv2;
using Amazon.Extensions.Configuration.SystemsManager;
using Amazon.Rekognition;
using Amazon.SimpleNotificationService;
using Api.Extensions;
using Api.Infrastructure.Context;
using Api.Infrastructure.Middleware;
using Domain.Options;
using Domain.Providers;
using Domain.Repositories;
using Domain.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Providers;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

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
builder.Services.AddMemoryCache();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<SmsSettings>(builder.Configuration.GetSection("SmsSettings"));
builder.Services.Configure<UniqueKeySettings>(builder.Configuration.GetSection("UniqueKeySettings"));
builder.Services.Configure<UserVerificationSettings>(builder.Configuration.GetSection("UserVerificationSettings"));
builder.Services.Configure<EventBusSettings>(builder.Configuration.GetSection("EventBusSettings"));
builder.Services.Configure<ApiKeyValidationSettings>(builder.Configuration.GetSection("ApiKeyValidationSettings"));
builder.Services.Configure<VerificationS3Settings>(builder.Configuration.GetSection("VerificationS3Settings"));
builder.Services.Configure<OtpSettings>(builder.Configuration.GetSection("OtpSettings"));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Logging.ClearProviders();
// Serilog configuration        
var logger = new LoggerConfiguration()
    .WriteTo.Console(new JsonFormatter())
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .CreateLogger();
// Register Serilog
builder.Logging.AddSerilog(logger);


builder.Services.AddScoped<IApiContext, ApiContext>();
builder.Services.AddScoped<IEventBusManager, EventBusManager>();
builder.Services.AddScoped<ILookupRepository, LookupRepository>();
builder.Services.AddScoped<IOtpCodeRepository, OtpCodeRepository>();
builder.Services.AddScoped<IUniqueKeyRepository, UniqueKeyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVerifyLogRepository, VerifyLogRepository>();
builder.Services.AddScoped<IReasonRepository, ReasonRepository>();
builder.Services.AddScoped<IUserReportRepository, UserReportRepository>();
builder.Services.AddScoped<ISmsProvider, DummySmsProvider>();
builder.Services.AddScoped<IMailProvider, DummyEmailProvider>();
builder.Services.AddScoped<IUserVerificationService, UserVerificationService>();
builder.Services.AddScoped<IUserIdentityVerificationService, UserIdentityVerificationService>();
builder.Services.AddScoped<IUserDeviceRepository, UserDeviceRepository>();
builder.Services.AddScoped<ApiKeyValidatorMiddleware>();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddAWSService<IAmazonSimpleNotificationService>();
builder.Services.AddAWSService<IAmazonRekognition>();
builder.Services.AddAWSLambdaHosting(Environment.GetEnvironmentVariable("ApiGatewayType") == "RestApi" ? LambdaEventSource.RestApi : LambdaEventSource.HttpApi);

var option = builder.Configuration.GetAWSOptions();
builder.Services.AddDefaultAWSOptions(option);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context => await Results.Problem().ExecuteAsync(context)));
}

app.UseMiddleware<ApiKeyValidatorMiddleware>();

app.MapEndpointsCore(AppDomain.CurrentDomain.GetAssemblies());

app.Run();