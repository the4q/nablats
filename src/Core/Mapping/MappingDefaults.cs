using System.Xml;
using System.Xml.Serialization;

namespace Nabla.TypeScript.Tool.Mapping
{
    public class MappingDefaults : IDateMapping
    {
        [XmlAttribute]
        public string? Variant { get; set; }

        public VariantTypeHandling? GetVariantHandling()
        {
            if (Variant != null)
                return System.Enum.Parse<VariantTypeHandling>(Variant, true);
            else
                return null;
        }

        [XmlElement]
        public EnumMapping? Enum { get; set; }

        [XmlAttribute]
        public string? Date { get; set; }

    }
}
