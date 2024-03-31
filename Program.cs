using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHangfire(x=>x.UseSqlServerStorage(builder.Configuration.GetConnectionString("SqlServerConnection")));
builder.Services.AddHangfireServer();

//  // Add your HangfireTutorialController dependencies
//     builder.Services.AddSingleton<IBackgroundJobClient, BackgroundJobClient>();
//     builder.Services.AddSingleton<IRecurringJobManager, RecurringJobManager>();

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

app.UseHangfireDashboard("/hangfire-dashboard");

app.UseAuthorization();

app.MapControllers();

app.Run();
