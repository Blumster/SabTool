using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Data
{
    using Utils;

    public partial class Blueprint
    {
        private Dictionary<string, List<object>> Properties { get; } = new();

        public string Type { get; set; }
        public string Name { get; set; }

        public Blueprint(string type, string name)
        {
            Type = type;
            Name = name;
        }

        public void AddProperty(string name, object value)
        {
            if (HasProperty(name))
            {
                Properties[name].Add(value);
            }
            else
            {
                Properties.Add(name, new() { value });
            }
        }

        public bool HasProperty(string name)
        {
            return Properties.ContainsKey(name);
        }

        public List<object> GetProperties(string name)
        {
            if (!HasProperty(name))
                throw new ArgumentOutOfRangeException(nameof(name));

            return Properties[name];
        }

        public object GetProperty(string name, int index = 0)
        {
            if (!HasProperty(name))
                throw new ArgumentOutOfRangeException(nameof(name));

            if (index < Properties[name].Count)
                return Properties[name][index];

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        public T GetProperty<T>(string name, int index = 0)
        {
            return (T)GetProperty(name, index);
        }

        public void AddProperty(Crc crc, object value) => AddProperty(crc.GetStringOrHexString(), value);
        public bool HasProperty(Crc crc) => HasProperty(crc.GetStringOrHexString());
        public List<object> GetProperties(Crc crc) => GetProperties(crc.GetStringOrHexString());
        public object GetProperty(Crc crc, int index = 0) => GetProperty(crc.GetStringOrHexString(), index);
        public T GetProperty<T>(Crc crc, int index = 0) => GetProperty<T>(crc.GetStringOrHexString(), index);

        public int GetPropertyCount() => Properties.Sum(p => p.Value.Count);

        public T GetAsBlueprint<T>() where T : Blueprint
        {
            return this as T;
        }

        public virtual bool DeserializePropertyRaw(BinaryReader reader, string propertyName, int propertySize)
        {
            return false;
        }

        public virtual bool SerializePropertyRaw(BinaryWriter writer, string propertyName)
        {
            return false;
        }

        public virtual bool DeserializePropertyJSON(/*TODO*/string propertyName, int propertySize)
        {
            return false;
        }

        public virtual bool SerializePropertyJSON(/*TODO*/ string propertyName)
        {
            return false;
        }

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
