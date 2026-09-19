var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/report", () => new[] {
    new { User = "vasia", Period = 1, Amount = 30 },
    new { User = "vasia", Period = 2, Amount = 45 },
    new { User = "vasia", Period = 1, Amount = 70 },
    new { User = "petia", Period = 1, Amount = 100 },
    new { User = "petia", Period = 3, Amount = 120 },
    new { User = "petia", Period = 3, Amount = 50 },
    new { User = "olga", Period = 3, Amount = 250 }
});

app.MapGet("/", () => "Hello World!");

app.Run();
