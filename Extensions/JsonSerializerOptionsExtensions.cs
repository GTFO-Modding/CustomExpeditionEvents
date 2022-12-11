using System.Text.Json;

namespace CustomExpeditionEvents.Extensions
{
    internal static class JsonSerializerOptionsExtensions
    {
        public static bool ArePropertiesEqual(this JsonSerializerOptions options, string propertyName, string propertyToCheck)
        {
            propertyName = options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName;
            propertyToCheck = options.PropertyNamingPolicy?.ConvertName(propertyToCheck) ?? propertyToCheck;

            if (options.PropertyNameCaseInsensitive)
            {
                return propertyName.Equals(propertyToCheck, System.StringComparison.OrdinalIgnoreCase);
            }


            return propertyName == propertyToCheck;
        }
    }
}
