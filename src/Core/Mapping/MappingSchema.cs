using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Nabla.TypeScript.Tool.Mapping
{
    [XmlRoot(ElementName = "Mapping", Namespace = TypeConstants.MappingSchemaNamespace)]
    public class MappingSchema
    {
        Dictionary<string, TypeMapping>? _types;
        Dictionary<string, MappingBase>? _files;

        [XmlElement]
        public MappingDefaults? Defaults { get; set; }

        [XmlElement(ElementName = "File")]
        public MappingBase[]? Files { get; set; }

        [XmlElement(ElementName = "Type")]
        public TypeMapping[]? Types { get; set; }

        internal void Prepare()
        {
            if (Types != null)
            {
                _types = Types.ToDictionary(x => x.Source);
                foreach (var type in Types)
                    type.Prepare(this);
            }

            if (Files != null)
            {
                IEqualityComparer<string> comparer;

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    comparer = StringComparer.OrdinalIgnoreCase;
                else
                    comparer = StringComparer.Ordinal;

                _files = Files.ToDictionary(x => x.Source, comparer);
            }
        }

        public TypeMapping? GetTypeMapping(string key)
        {
            if (_types?.TryGetValue(key, out var typeMapping) == true)
                return typeMapping;

            return null;
        }

        public MappingBase? GetFileMapping(string key)
        {
            if (_files?.TryGetValue(key, out var mapping) == true)
                return mapping;

            return null;
        }

        public static MappingSchema Load(Stream input)
        {
            using var schemaStream = typeof(MappingSchema).Assembly.GetManifestResourceStream($"{typeof(MappingSchema).Namespace}.Mapping.xsd");
            using var schema = XmlReader.Create(schemaStream!);

            XmlSerializer xs = new(typeof(MappingSchema), TypeConstants.MappingSchemaNamespace);

            XmlReaderSettings settings = new();

            settings.ValidationType = ValidationType.Schema;
            settings.Schemas.Add(TypeConstants.MappingSchemaNamespace, schema);

            XmlReader reader = XmlReader.Create(input, settings);

            try
            {
                return (MappingSchema)xs.Deserialize(reader)!;
            }
            catch(InvalidOperationException ex)
            {
                if (ex.GetBaseException() is System.Xml.Schema.XmlSchemaValidationException valex)
                {
                    throw new CodeException("Found invalid content in mapping schema: " + valex.Message, valex);
                }

                throw;
            }
        }

        public static MappingSchema Load(string path)
        {
            using var input = File.OpenRead(path);

            return Load(input);
        }
    }
}
