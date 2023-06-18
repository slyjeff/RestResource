using Slysoft.RestResource.AspNetCoreUtils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => {
    options.OutputFormatters.Clear();
    options.OutputFormatters.Add(new ResourceHalJsonFormatter());
    options.OutputFormatters.Add(new ResourceHalXmlFormatter());
    options.OutputFormatters.Add(new ResourceHtmlFormatter());
    options.RespectBrowserAcceptHeader = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
}
app.UseRouting();
app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});

app.Run();
