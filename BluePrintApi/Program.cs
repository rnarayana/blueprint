namespace BluePrintApi;

public class Program
{
    protected Program()
    {
    }

    public static void Main(string[] args)
    {
        WebApplication app = ConfigureApp(args);

        app.Run();
    }

    internal static WebApplication ConfigureApp(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Configuration.LoadConfigurations();

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        return app;
    }
}
