using Nabla.TypeScript.Tool;

namespace MyApp;

[TsEnumHandling(EnumHandling.Const, NamingPolicy = PropertyNamingPolicy.Unchanged)]
public enum WeatherKind
{
    Sunny,
    Cloudy,
    LightRain = 10,
    MediumRain,
    HeavyRain,
    LightSnow = 20,
    MediumSnow,
    HeavySnow
}