namespace Nabla.TypeScript.Tool.Mapping
{
    internal static class MappingExtensions
    {
        public static DateHandling? GetDateHandling(this IDateMapping mapping)
        {
            if (mapping.Date != null)
                return Enum.Parse<DateHandling>(mapping.Date, true);
            else
                return null;
        }
    }
}
