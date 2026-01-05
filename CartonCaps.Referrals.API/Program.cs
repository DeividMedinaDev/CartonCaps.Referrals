using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Domain & Infrastructure bindings
builder.Services.AddSingleton<CartonCaps.Referrals.Core.Interfaces.IReferralRepository, CartonCaps.Referrals.Infrastructure.Data.MockReferralRepository>();
builder.Services.AddSingleton<CartonCaps.Referrals.Core.Interfaces.IDeepLinkService, CartonCaps.Referrals.Infrastructure.Services.MockDeepLinkService>();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddControllers();
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
