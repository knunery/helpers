/* v4 */
public class EmptyStringToNullConverter<T> : JsonConverter<T?> where T : class
{
    [ThreadStatic]
    private static bool _isInternalCall;

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!_isInternalCall)
        {
            if (reader.TokenType == JsonTokenType.String && reader.GetString() == "")
            {
                return null;
            }
        }

        try
        {
            _isInternalCall = true;
            return JsonSerializer.Deserialize<T>(ref reader, options);
        }
        finally
        {
            _isInternalCall = false;
        }
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}


/* v3 */
public class EmptyStringToNullConverter<T> : JsonConverter<T?> where T : class
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && reader.GetString() == "")
        {
            return null;
        }

        using (var jsonDoc = JsonDocument.ParseValue(ref reader))
        {
            var rootElement = jsonDoc.RootElement.Clone();
            return JsonSerializer.Deserialize<T>(rootElement.GetRawText());
        }
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}

/* v2 */
public class SecondaryConverter<T> : JsonConverter<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<T>(ref reader);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value);
    }
}

public class EmptyStringToNullConverter<T> : JsonConverter<T?> where T : class
{
    private readonly SecondaryConverter<T> _secondaryConverter = new SecondaryConverter<T>();

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && reader.GetString() == "")
        {
            return null;
        }
        return _secondaryConverter.Read(ref reader, typeToConvert, options);
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            _secondaryConverter.Write(writer, value, options);
        }
    }
}


/* v1 */
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class EmptyStringToNullConverter<T> : JsonConverter<T?> where T : class
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && reader.GetString() == "")
        {
            return null;
        }
        return JsonSerializer.Deserialize<T>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
