using CustomExpeditionEvents.Data;
using CustomExpeditionEvents.Extensions;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomExpeditionEvents.Converters
{
    public abstract class ItemDataBaseJsonConverter<T> : JsonConverter<T>
        where T : ItemDataBase, new()
    {
        public sealed override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            else if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Expected {JsonTokenType.StartObject}, instead got {reader.TokenType}");
            }

            T value = new T();
            string propertyName = string.Empty;

            bool readingProperty = true;
            while (reader.Read())
            {
                if (readingProperty)
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException($"Expected {JsonTokenType.PropertyName}, instead got {reader.TokenType}");
                    }

                    propertyName = reader.GetString()!;
                    readingProperty = !readingProperty;
                    continue;
                }

                if (options.ArePropertiesEqual(propertyName, nameof(ItemDataBase.DebugName)))
                {
                    reader.AssertPropertyType(nameof(ItemDataBase.DebugName), JsonTokenType.String);

                    value.DebugName = reader.GetString()!;
                }
                else if (options.ArePropertiesEqual(propertyName, nameof(ItemDataBase.Disabled)))
                {
                    reader.AssertPropertyType(nameof(ItemDataBase.Disabled), JsonTokenType.True, JsonTokenType.False);

                    value.Disabled = reader.GetBoolean();
                }
                else if (!this.ReadProperty(value, propertyName, ref reader, options))
                {
                    throw new JsonException("Unknown property name '" + propertyName + "'.");
                }

                readingProperty = !readingProperty;
            }

            if (!this.IsItemValid(value))
            {
                throw new JsonException("Unexpected end of input.");
            }

            return value;
        }

        /// <summary>
        /// Returns whether the given item is in a valid state. This is called
        /// after <see cref="JsonTokenType.EndObject"/> is detected.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// <see langword="true"/> if the item is valid, or
        /// <see langword="false"/> if the item isn't, and as such a
        /// JSON Exception will be thrown.
        /// </returns>
        protected abstract bool IsItemValid(T item);

        /// <summary>
        /// Read a property called <paramref name="propertyName"/> and apply the value
        /// to <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The current item.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="reader">The JSON Reader</param>
        /// <param name="options">Options for JSON</param>
        /// <returns>
        /// <see langword="true"/> if the property was read successfully, otherwise
        /// <see langword="false"/> if the property isn't recognized.
        /// </returns>
        protected abstract bool ReadProperty(T item, string propertyName, ref Utf8JsonReader reader, JsonSerializerOptions options);

        public sealed override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            this.WriteProperties(writer, value, options);

            if (value.DebugName != null)
            {
                writer.WriteString(nameof(ItemDataBase.DebugName), value.DebugName);
            }
            writer.WriteBoolean(nameof(ItemDataBase.Disabled), value.Disabled);

            writer.WriteEndObject();
        }

        protected abstract void WriteProperties(Utf8JsonWriter writer, T value, JsonSerializerOptions options);
    }
}
