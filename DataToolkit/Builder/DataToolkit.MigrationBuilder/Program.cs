using DataToolkit.MigrationBuilder.Configuration;
using DataToolkit.MigrationBuilder.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MigrationOptions>(
    builder.Configuration.GetSection(
        MigrationOptions.SectionName));

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services AddDataToolkitSample
builder.Services.AddBuilderServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();