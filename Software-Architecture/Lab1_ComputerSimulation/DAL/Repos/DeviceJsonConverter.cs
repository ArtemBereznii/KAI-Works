using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using HardwareSim.BLL.Entities.Base;

namespace HardwareSim.DAL.Repositories
{
    public class DeviceJsonConverter : JsonConverter<Device>
    {
        public override Device? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                if (doc.RootElement.TryGetProperty("$type", out JsonElement typeElement))
                {
                    string typeName = typeElement.GetString() ?? "";

                    Type? actualType = Assembly.GetAssembly(typeof(Device))?
                        .GetTypes()
                        .FirstOrDefault(t => t.Name == typeName && typeof(Device).IsAssignableFrom(t));

                    if (actualType != null)
                    {
                        return (Device?)JsonSerializer.Deserialize(doc.RootElement.GetRawText(), actualType, options);
                    }
                }
                throw new NotSupportedException("Device type discriminator missing or class not found.");
            }
        }

        public override void Write(Utf8JsonWriter writer, Device value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("$type", value.GetType().Name);

            using (JsonDocument doc = JsonSerializer.SerializeToDocument(value, value.GetType(), options))
            {
                foreach (var property in doc.RootElement.EnumerateObject())
                {
                    property.WriteTo(writer);
                }
            }

            writer.WriteEndObject();
        }
    }
}