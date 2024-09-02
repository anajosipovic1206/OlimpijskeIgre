using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OlimpijskeIgre
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _format = "dd/MM/yy";
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (DateTime.TryParseExact(reader.GetString(), _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }

            throw new JsonException($"Datum nije u ispravnom formatu: {reader.GetString()}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format));
        }
    }
    
        public class IzlozbeniMec
        {
            [JsonConverter(typeof(DateTimeConverter))]
            [JsonPropertyName("Date")]
            public DateTime Datum { get; set; }

            [JsonPropertyName("Opponent")]
            public string Protivnik { get; set; }

            [JsonPropertyName("Result")]
            public string Rezultat { get; set; }
        }
    
}
