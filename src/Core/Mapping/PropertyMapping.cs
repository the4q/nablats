using System.Xml;
using System.Xml.Serialization;

namespace Nabla.TypeScript.Tool.Mapping
{
    public class PropertyMapping : MappingBase, IDateMapping
    {
        [XmlAttribute]
        public bool Export { get; set; }

        [XmlAttribute]
        public int TupleOrder { get; set; }

        [XmlAttribute]
        public string? Date { get; set; }

        [XmlElement]
        public MappingTypeOverriding? Overriding { get; set; }

        [XmlElement]
        public VariantMapping? Variant { get; set; }
    }
}
