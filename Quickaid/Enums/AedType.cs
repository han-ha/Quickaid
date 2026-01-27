using System.Text.Json.Serialization;

namespace Quickaid.Enums
{
    /// <summary>
    /// Typ AED – określa źródło danych AED.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AedType
    {
        Internal,
        External
    }
}
