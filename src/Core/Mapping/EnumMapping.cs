using System.Xml;
using System.Xml.Serialization;

namespace Nabla.TypeScript.Tool.Mapping
{
    public class EnumMapping
    {
        [XmlAttribute]
        public string? Handling { get; set; }

        [XmlAttribute]
        public string? NamingPolicy { get; set; }

        public EnumHandling? GetHandling()
        {
            if (Handling != null)
                return Enum.Parse<EnumHandling>(Handling, true);
            else
                return null;
        }

        public PropertyNamingPolicy? GetNamingPolicy()
        {
            if (NamingPolicy != null)
                return Enum.Parse<PropertyNamingPolicy>(NamingPolicy, true);
            else
                return null;
        }
    }
}
