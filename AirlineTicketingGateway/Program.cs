using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.HttpOverrides; 

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);


builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
   
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();


app.UseForwardedHeaders();

app.Use((context, next) =>
{
    var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown_ip";
    context.Request.Headers["X-Client-IP"] = clientIp;
    return next();
});

app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
});

app.MapControllers();
await app.UseOcelot();

app.Run();