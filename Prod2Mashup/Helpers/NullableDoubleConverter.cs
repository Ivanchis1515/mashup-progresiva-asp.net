using Newtonsoft.Json;

namespace Prod2Mashup.Helpers
{
    //clase que hereda JsonConverter para convertir valores tipo double? en nulos
    public class NullableDoubleConverter : JsonConverter<double?>
    {
        public override double? ReadJson(JsonReader reader, Type objectType, double? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Si el valor en el JSON es nulo (JsonToken.Null), asignamos null al campo gust
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            // Si el valor es un número flotante o entero (JsonToken.Float o JsonToken.Integer), lo convertimos a double
            if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
            {
                return Convert.ToDouble(reader.Value);
            }

            // Si el valor no es ni nulo ni un número, retornamos null para manejar casos donde el campo no esté presente
            return null; // Si no se encuentra el campo o no es válido, retornamos null
        }

        // El método WriteJson se ejecuta cuando se serializa el objeto a JSON
        public override void WriteJson(JsonWriter writer, double? value, JsonSerializer serializer)
        {
            // Si el valor tiene un valor (es decir, no es null), lo escribimos al JSON
            if (value.HasValue)
            {
                writer.WriteValue(value.Value);
            }
            else
            {
                // Si el valor es null, escribimos un valor nulo en el JSON
                writer.WriteNull();
            }
        }
    }
}
