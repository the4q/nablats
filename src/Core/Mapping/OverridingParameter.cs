using System.Xml;
using System.Xml.Serialization;

namespace Nabla.TypeScript.Tool.Mapping
{
    public sealed class OverridingParameter
    {
        [XmlAttribute]
        public string Value { get; set; } = null!;

        [XmlAttribute]
        public OverridingParameterKind Kind { get; set; }

        public object GetParameter()
        {
            if (Kind == OverridingParameterKind.Clr)
                return Type.GetType(Value, true)!;
            else
                return Value;
        }
    }
}
