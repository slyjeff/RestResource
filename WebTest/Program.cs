using Slysoft.RestResource.AspNetCoreUtils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => {
    options.OutputFormatters.Insert(0, new ResourceHtmlFormatter());
    options.OutputFormatters.Insert(1, new ResourceHalJsonFormatter());
    options.OutputFormatters.Insert(2, new ResourceHalXmlFormatter());
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
