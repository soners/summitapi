using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var studentScores = new Dictionary<string, StudentScore>
{
    { "68b3nak", new StudentScore("68b3nak", 70) }
};

app.MapGet("/api/students/{studentId}/score",
    ([FromRoute] string studentId) => studentScores.TryGetValue(studentId, out var studentScore)
        ? Results.Ok(studentScore)
        : Results.NotFound("STUDENT_SCORE_NOT_FOUND"));

app.MapPost("/api/students/{studentId}/score", ([FromRoute] string studentId, [FromQuery] int score) =>
{
    studentScores.Add(studentId, new StudentScore(studentId, score));

    return Results.Ok("SUCCESS");
});

app.Run();

internal record StudentScore(string StudentId, int Score);