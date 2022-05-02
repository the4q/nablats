using Nabla.TypeScript.Tool;

namespace MyApp;

[TsTuple]
public struct Location
{
    [TsTupleOrder(3)]
    public string Address { get; set; }

    public double Longitude { get; set; }

    public double Latitude { get; set; }
}