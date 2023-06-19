using Slysoft.RestResource.AspNetCoreUtils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => {
    //register the formatters for the browser, json, and xml. Customer formatters can also be written and registered for other types
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

//enforce consistent handling of payload data- we can always expect a body, even if it's coming from a browser
//this is also supports the "tunneling" of "put" and "delete" so those options can be emulated in the browser- they are converted from post to the correct verb based on a hidden field
app.TransformUrlEncodedFormsToJson();

app.UseRouting();
app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});

app.Run();
