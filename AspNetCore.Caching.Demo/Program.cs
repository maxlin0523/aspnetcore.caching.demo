var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

// ���o appsettings ���� Redis �s�u�r��
var redisConStr = builder.Configuration.GetSection("RedisCache:ConnectionString").Value;
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConStr;
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
