using SupremoWeb.Repository;
using SupremoWeb.Views.Shared;
using FluentValidation.AspNetCore;
using SupremoWeb.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

//*Config Ler appsettings
var provider = builder.Services.BuildServiceProvider();
var configuration = provider.GetRequiredService<IConfiguration>();

//*Config Injecao de Depend�ncia
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ILoggerRepository, LoggerRepository>();
builder.Services.AddScoped<IAutenticacaoRepository, AutenticacaoRepository>();
builder.Services.AddScoped<ITelaClientesRepository, TelaClientesRepository>();
builder.Services.AddSingleton<IConfiguration>(provider =>new ConfigurationBuilder()  
    .AddJsonFile("appsettings.json")
    .Build());

// Adicione os servi�os ao container.
builder.Services.AddControllersWithViews()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ClienteModelValidator>());

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(name: "default",pattern: "{controller=Login}/{action=Index}/{id?}");

// Inicialize a classe est�tica
ShareFunctions.Initialize(app.Services.GetRequiredService<IConfiguration>(), app.Services.GetRequiredService<ILoggerRepository>());

app.Run();
