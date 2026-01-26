using System.Text.Json.Serialization;

namespace Quickaid.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AedType
    {
        Internal,
        External
    }
}
