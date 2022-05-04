using System.Xml;
using System.Xml.Serialization;

namespace Nabla.TypeScript.Tool.Mapping
{
    public class MappingBase
    {

        [XmlAttribute]
        public string Source { get; set; } = null!;

        [XmlAttribute]
        public string? Target { get; set; }


    }
}
