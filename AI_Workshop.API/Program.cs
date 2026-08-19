using System.Text.Json.Serialization;
using AI_Workshop.AI.DependencyInjection;
using AI_Workshop.Application.UseCases.CreateIdea;
using AI_Workshop.Application.UseCases.CloseTeamRegistration;
using AI_Workshop.Application.UseCases.GetEmployeeWorkshopTeam;
using AI_Workshop.Application.UseCases.GetIdeaById;
using AI_Workshop.Application.UseCases.GetIdeas;
using AI_Workshop.Application.UseCases.GetOrganizerDashboardSummary;
using AI_Workshop.Application.UseCases.GetSchedule;
using AI_Workshop.Application.UseCases.GetWorkshopSettings;
using AI_Workshop.Application.UseCases.GetWorkshopTeamById;
using AI_Workshop.Application.UseCases.GetWorkshopTeamByIdeaId;
using AI_Workshop.Application.UseCases.GetWorkshopTeams;
using AI_Workshop.Application.UseCases.JoinWorkshopTeam;
using AI_Workshop.Application.UseCases.LeaveWorkshopTeam;
using AI_Workshop.Application.UseCases.OpenTeamRegistration;
using AI_Workshop.Application.UseCases.SwitchWorkshopTeam;
using AI_Workshop.Application.UseCases.UpdateTeamRegistrationCloseDate;
using AI_Workshop.Application.UseCases.WorkshopTeams;
using AI_Workshop.Infrastructure.DependencyInjection;
using AI_Workshop.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("UiLocal", policy =>
    {
        policy
            .WithOrigins("http://localhost:5268", "https://localhost:7072")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddIdeaAssistant(builder.Configuration);

builder.Services.AddScoped<GetIdeasUseCase>();
builder.Services.AddScoped<GetIdeaByIdUseCase>();
builder.Services.AddScoped<CreateIdeaUseCase>();
builder.Services.AddScoped<GetScheduleUseCase>();
builder.Services.AddScoped<GetScheduleByDayUseCase>();
builder.Services.AddScoped<GetWorkshopTeamsUseCase>();
builder.Services.AddScoped<EnsureFinalistTeamsUseCase>();
builder.Services.AddScoped<GetWorkshopTeamByIdUseCase>();
builder.Services.AddScoped<GetWorkshopTeamByIdeaIdUseCase>();
builder.Services.AddScoped<GetEmployeeWorkshopTeamUseCase>();
builder.Services.AddScoped<JoinWorkshopTeamUseCase>();
builder.Services.AddScoped<LeaveWorkshopTeamUseCase>();
builder.Services.AddScoped<SwitchWorkshopTeamUseCase>();
builder.Services.AddScoped<GetOrganizerDashboardSummaryUseCase>();
builder.Services.AddScoped<GetWorkshopSettingsUseCase>();
builder.Services.AddScoped<OpenTeamRegistrationUseCase>();
builder.Services.AddScoped<CloseTeamRegistrationUseCase>();
builder.Services.AddScoped<UpdateTeamRegistrationCloseDateUseCase>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("UiLocal");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
