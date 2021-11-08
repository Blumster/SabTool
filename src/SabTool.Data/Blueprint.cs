using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Data
{
    public partial class Blueprint
    {
        private Dictionary<string, object> Properties { get; } = new();

        public string Type { get; set; }
        public string Name { get; set; }

        public Blueprint(string type, string name)
        {
            Type = type;
            Name = name;
        }

        public void AddProperty(string name, object value)
        {
            Properties.Add(name, value);
        }

        public bool HasProperty(string name)
        {
            return Properties.ContainsKey(name);
        }

        public object GetProperty(string name)
        {
            return Properties[name];
        }

        public T GetProperty<T>(string name)
        {
            return (T)Properties[name];
            
        }

        public void AddProperty(uint crc, object value) => AddProperty(FormatCrc(crc), value);
        public bool HasProperty(uint crc) => HasProperty(FormatCrc(crc));
        public object GetProperty(uint crc) => GetProperty(FormatCrc(crc));
        public T GetProperty<T>(uint crc) => GetProperty<T>(FormatCrc(crc));

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Blueprint({Type}, {Name})");
            sb.AppendLine("{");

            foreach (var propPair in Properties)
            {
                sb.AppendLine($"  {propPair.Key} => {propPair.Value}");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
