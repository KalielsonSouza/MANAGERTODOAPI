using MANAGERTODOAPI.DiagnosticRule;
using MANAGERTODOAPI.DiagnosticRules;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Project1.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuração do serviço de autenticação
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "8K9C82T0",
                ValidAudience = "5TMLJGGX",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("2J7D2TIR"))
            };
        });
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>();
var app = builder.Build();
//// -> 
await TestAnalyzerOnSolutionAsync(new IFANALISER());
await TestAnalyzerOnSolutionAsync(new PasswordDecryptionAnalyzer());
await TestAnalyzerOnSolutionAsync(new SwitchCaseCountAnalyzer());
await TestAnalyzerOnSolutionAsync(new PadraoVar());
/// <-



static async Task TestAnalyzerOnSolutionAsync(DiagnosticAnalyzer analyzer)
{
    var analiser = new startDiagnosis();
    await analiser.TestAnalyzerOnSolutionAsync(analyzer);
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors(builder => builder
        .WithOrigins("http://localhost:4200") // altere para o endereço do seu frontend
        .AllowAnyMethod()
        .AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


