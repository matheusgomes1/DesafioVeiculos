namespace DesafioVeiculos.Infra.Configuration
{
    public static class ApiConfig
    {
        public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            //  CORS (Cross Origin Resource Sharing) Compartilhamento de Recursos de Origem Cruzada 
            //  Permitir qualquer Header, Method, Credentials e Origins
            //https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-6.0
            string[] policys = configuration["Origins"].Split(",");

            services.AddCors(opt => opt.AddPolicy("CorsPolicy",
              builder =>
              {
                  builder
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials()
                      .WithOrigins(policys);
              }));
        }
    }
}
