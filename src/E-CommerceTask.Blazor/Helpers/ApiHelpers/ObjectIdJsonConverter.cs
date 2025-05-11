using System.Text.Json.Serialization;

namespace E_CommerceTask.Blazor.Helpers.ApiHelpers;

public class ObjectIdJsonConverter : JsonConverter<ObjectId>
{
    public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var idString = reader.GetString();
        return ObjectId.TryParse(idString, out var objectId) ? objectId : ObjectId.Empty;
    }

    public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}