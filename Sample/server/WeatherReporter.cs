using Nabla.TypeScript.Tool;
using System.Text.Json.Serialization;

namespace MyApp;

[TsExport, TsTypeName("Reporter")]
public class WeatherReporter
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string?[]? Schedules { get; set; }

    public DayOfWeek[]? Workdays { get; set; }

    [JsonPropertyName("favorite")]
    public WeatherKind? FavoriteWeather { get; set; }
}