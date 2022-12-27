using System.Text;
using System.Text.Json;

namespace CustomExpeditionEvents.Extensions
{
    internal static class Utf8JsonReaderExtensions
    {
        public static void AssertPropertyType(this Utf8JsonReader reader, string propertyName, params JsonTokenType[] validTypes)
        {
            int validTypeCount = validTypes.Length;
            if (validTypeCount == 0)
            {
                return;
            }

            for (int index = 0; index < validTypeCount; index++)
            {
                if (reader.TokenType == validTypes[index])
                {
                    return;
                }
            }

            StringBuilder messageBuilder = new();
            messageBuilder.Append("Error reading property name '");
            messageBuilder.Append(propertyName);
            messageBuilder.Append("': Expected '");
            for (int index = 0; index < validTypeCount; index++)
            {
                if (index > 0)
                {
                    messageBuilder.Append(", ");
                }

                messageBuilder.Append('\'');
                messageBuilder.Append(validTypes[index]);
                messageBuilder.Append('\'');
            }
            messageBuilder.Append("'. Instead got '");
            messageBuilder.Append(reader.TokenType);
            messageBuilder.Append("'.");

            throw new JsonException(messageBuilder.ToString());
        }
    }
}
