using Nabla.TypeScript.Tool;

namespace MyApp;

public class WeatherForecast
{
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }

    public WeatherKind Kind { get; set; }

    public Location? Location { get; set; }

    [TsTypeOverride("Tuple", typeof(int), "string | undefined", ArrayDepth = 1)]
    public object? OverrideDemo { get; set; }
}
