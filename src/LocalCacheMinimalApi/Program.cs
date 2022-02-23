using LocalCacheMinimalApi;
using LocalCacheMinimalApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<AppParameters>();
builder.Services.AddSingleton<RedisCacheOperations>();
builder.Services.AddTransient<UserRepositoty>();
builder.Services.AddTransient<UserRemoteCacheRepositoty>();
builder.Services.AddTransient<UserMixLocalRemoteCacheRepositoty>();
builder.Services.AddMemoryCache();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/user/fromDb/{id}", async (UserRepositoty userRepositoty, string id) =>
{
    var user = await userRepositoty.GetById(id);

    return user is not null ? Results.Ok(user) : Results.NotFound();
})
.WithName("GetUser from DB");

app.MapGet("/user/fromRemoteCache/{id}", async  (UserRemoteCacheRepositoty userRemoteCacheRepositoty, string id) =>
{
    var user = await userRemoteCacheRepositoty.GetById(id);

    return user is not null ? Results.Ok(user) : Results.NotFound();
})
.WithName("GetUser from remote cache");

app.MapGet("/user/fromMixLocalRemoteCache/{id}", async (UserMixLocalRemoteCacheRepositoty userMixLocalRemoteCacheRepositoty, string id) =>
{
    var user = await userMixLocalRemoteCacheRepositoty.GetById(id);

    return user is not null ? Results.Ok(user) : Results.NotFound();
})
.WithName("GetUser from local and remote cache");

app.Run();
