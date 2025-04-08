using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace playerService.Mapper
{
    public class DateTimeConverter : JsonConverter
    {
        // Override CanConvert to specify the type of object that can be converted
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            long ticks = (long)reader.Value;
            return new DateTime(ticks);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            DateTime dateTime = (DateTime)value;
            writer.WriteValue(dateTime.Ticks); // or dateTime.ToUniversalTime().Ticks if you want UTC ticks
        }
    }
}
