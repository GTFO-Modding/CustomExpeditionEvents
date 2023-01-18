using CustomExpeditionEvents.Events.Common.Managers;
using CustomExpeditionEvents.Extensions;
using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomExpeditionEvents.Events.Common
{
    internal sealed class SetDataEvent : IEvent<SetDataEvent.Data> 
    {
        public string Name => "SetData";

        public void Activate(Data data)
        {
            switch (data.Type)
            {
                case DataType.Boolean:
                    DataStoreManager.SetBoolean(data.Key, (bool)data.Value);
                    break;
                case DataType.Double:
                    DataStoreManager.SetDouble(data.Key, (double)data.Value);
                    break;
                case DataType.Single:
                    DataStoreManager.SetSingle(data.Key, (float)data.Value);
                    break;
                case DataType.Int32:
                    DataStoreManager.SetInt32(data.Key, (int)data.Value);
                    break;
                case DataType.Int64:
                    DataStoreManager.SetInt64(data.Key, (long)data.Value);
                    break;
            }
        }

        [JsonConverter(typeof(DataJsonConverter))]
        public sealed class Data
        {
            [Description("The key to use to store the data.")]
            public string Key { get; set; }
            [Description("The type of data to store.")]
            public DataType Type { get; set; }
            [Description("The stored data. Should match the data type.")]
            public object Value { get; set; }
        }

        public enum DataType
        {
            [Description("True/False value")]
            Boolean,
            [Description("32-Bit Integer")]
            Int32,
            [Description("64-Bit Integer")]
            Int64,
            [Description("32-Bit (Single Precision) Floating Point Number")]
            Single,
            [Description("64-Bit (Double Precision) Floating Point Number")]
            Double
        }

        public sealed class DataJsonConverter : JsonConverter<Data>
        {
            public override Data? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    return null;
                }
                else if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException("Expected start object, but didn't get it");
                }

                bool readingProperty = true;
                string propertyName = string.Empty;
                bool hasType = false;
                bool hasValue = false;
                Data data = new();

                while (reader.Read())
                {
                    if (readingProperty)
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                        {
                            if (!hasType)
                            {
                                throw new JsonException("Unexpected end of SetDataEvent Data: Assign a Data Type");
                            }
                            if (!hasValue)
                            {
                                throw new JsonException("Unexpected end of SetDataEvent Data: Assign a Data Value");
                            }

                            return data;
                        }

                        propertyName = reader.GetString()!;
                    }
                    else
                    {
                        if (options.ArePropertiesEqual(propertyName, nameof(Data.Key)))
                        {
                            reader.AssertPropertyType(nameof(Data.Key), JsonTokenType.String);

                            data.Key = reader.GetString()!;
                        }
                        else if (options.ArePropertiesEqual(propertyName, nameof(Data.Type)))
                        {
                            reader.AssertPropertyType(nameof(Data.Type), JsonTokenType.String, JsonTokenType.Number);

                            data.Type = JsonSerializer.Deserialize<DataType>(ref reader, options);
                            hasType = true;
                        }
                        else if (options.ArePropertiesEqual(propertyName, nameof(Data.Value)))
                        {
                            if (!hasType)
                            {
                                throw new JsonException("Assign a type the data can be before assigning a value");
                            }

                            switch (data.Type)
                            {
                                case DataType.Boolean:
                                    reader.AssertPropertyType(propertyName, JsonTokenType.True, JsonTokenType.False);
                                    break;
                                case DataType.Double:
                                case DataType.Single:
                                case DataType.Int32:
                                case DataType.Int64:
                                    reader.AssertPropertyType(propertyName, JsonTokenType.Number);
                                    break;
                            }

                            hasValue = true;
                            switch (data.Type)
                            {
                                case DataType.Boolean:
                                    data.Value = reader.GetBoolean();
                                    break;
                                case DataType.Double:
                                    data.Value = reader.GetDouble();
                                    break;
                                case DataType.Int32:
                                    data.Value = reader.GetInt32();
                                    break;
                                case DataType.Int64:
                                    data.Value = reader.GetInt64();
                                    break;
                                case DataType.Single:
                                    data.Value = reader.GetSingle();
                                    break;
                                default:
                                    hasValue = false;
                                    break;
                            }
                        }
                        else
                        {
                            throw new JsonException("Unsupported property name '" + propertyName + "'");
                        }
                    }

                    readingProperty = !readingProperty;
                }

                throw new JsonException("Unexpected end of input");
            }

            public override void Write(Utf8JsonWriter writer, Data value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteString(nameof(Data.Key), value.Key);
                writer.WritePropertyName(nameof(Data.Type));
                JsonSerializer.Serialize(writer, value.Type, typeof(DataType), options);
                writer.WritePropertyName(nameof(Data.Value));

                switch (value.Type)
                {
                    case DataType.Boolean:
                        writer.WriteBooleanValue((bool)value.Value);
                        break;
                    case DataType.Double:
                        writer.WriteNumberValue((double)value.Value);
                        break;
                    case DataType.Int32:
                        writer.WriteNumberValue((int)value.Value);
                        break;
                    case DataType.Int64:
                        writer.WriteNumberValue((long)value.Value);
                        break;
                    case DataType.Single:
                        writer.WriteNumberValue((float)value.Value);
                        break;
                    default:
                        throw new JsonException("Unsupported data type '" + value.Type + "'");
                }

                writer.WriteEndObject();
            }
        }
    }
}
