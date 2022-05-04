using System.Xml;
using System.Xml.Serialization;

namespace Nabla.TypeScript.Tool.Mapping
{
    public class VariantMapping
    {
        [XmlAttribute]
        public string? GenericParameterName { get; set; }

        [XmlAttribute]
        public VariantTypeHandling Handling { get; set; }

        internal VariantTypeInfo CreateVairantInfo()
        {
            return new(Handling, GenericParameterName);
        }

    }
}
