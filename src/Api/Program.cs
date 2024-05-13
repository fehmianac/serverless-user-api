using System.Globalization;
using Amazon.DynamoDBv2;
using Amazon.Extensions.Configuration.SystemsManager;
using Amazon.Extensions.NETCore.Setup;
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
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
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
    .Enrich.WithProperty("Application", "User")
    .WriteTo.Console(new JsonFormatter())
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("AWSSDK", LogEventLevel.Warning)
    .MinimumLevel.Override("System.", LogEventLevel.Warning)
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


builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
builder.Services.AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
builder.Services.AddSingleton<IStringLocalizer>(provider =>
    provider.GetService<IStringLocalizerFactory>().Create("Resource", "Api"));

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("tr-TR")
    };
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
    {
        var languages = context.Request.Headers["x-culture"].ToString();
        var currentLanguage = languages.Split(',').FirstOrDefault();
        var defaultLanguage = string.IsNullOrEmpty(currentLanguage) ? "en-US" : currentLanguage;
        if (!supportedCultures.Where(s => s.Name.Equals(defaultLanguage)).Any())
        {
            defaultLanguage = "en-US";
        }

        return Task.FromResult(new ProviderCultureResult(defaultLanguage, defaultLanguage));
    }));
});
builder.Services.AddLocalization();
var option = builder.Configuration.GetAWSOptions();
builder.Services.AddDefaultAWSOptions(option);

var app = builder.Build();

var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationOptions.Value);

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
