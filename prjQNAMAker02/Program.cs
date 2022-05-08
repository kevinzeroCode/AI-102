var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.MapControllerRoute
    (name:"default",
    pattern: "{Controller=Home}/{Action=Index}/{id?}");
app.Run();
