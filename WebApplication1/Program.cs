using DesafioVeiculos.Infra.Configuration;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
builder.Services.ResolveDependencies(configuration);
builder.Services.AddApiConfiguration(configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.UseCors("CorsPolicy");

app.Run();
