using System.Xml;
using System.Xml.Serialization;

namespace Nabla.TypeScript.Tool.Mapping
{
    public class MappingTypeOverriding : ITypeOverrideInfo
    {
        [XmlAttribute]
        public string TypeName { get; set; } = null!;

        [XmlAttribute]
        public string? ModuleName { get; set; }

        [XmlAttribute]
        public int ArrayDepth { get; set; }

        [XmlElement(ElementName = "Parameter")]
        public OverridingParameter[]? Parameters { get; set; }

        object[]? ITypeOverrideInfo.TypeParameters => Parameters?.Select(x => x.GetParameter()).ToArray();
    }
}
