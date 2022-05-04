using System.Xml;
using System.Xml.Serialization;

namespace Nabla.TypeScript.Tool.Mapping
{
    public class TypeMapping : MappingBase, IDateMapping
    {
        Dictionary<string, PropertyMapping>? _properties;

        [XmlAttribute]
        public bool Export { get; set; }

        [XmlAttribute]
        public bool UseTuple { get; set; }

        [XmlAttribute]
        public string? Date { get; set; }

        [XmlElement(ElementName = "Property")]
        public PropertyMapping[]? Properties { get; set; }

        [XmlElement]
        public EnumMapping? Enum { get; set; }

        [XmlIgnore]
        public MappingSchema? Schema { get; private set; }

        internal void Prepare(MappingSchema schema)
        {
            Schema = schema;
            _properties = Properties?.ToDictionary(x => x.Source);
        }

        public PropertyMapping? GetProperty(string name)
        {
            if (_properties?.TryGetValue(name, out var property) == true)
                return property;

            return null;
        }

    }
}
