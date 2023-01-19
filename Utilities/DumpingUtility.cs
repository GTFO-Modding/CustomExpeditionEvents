using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace CustomExpeditionEvents.Utilities
{
    internal static class DumpingUtility
    {
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
        public sealed class SkipAttribute : Attribute
        { }

        private static readonly Type[] builtInTypes = new Type[]
        {
            typeof(string),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(Guid),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(char)
        };

        public static void Dump(StringBuilder builder, Type objType, HashSet<Type>? documentedTypes = null)
        {
            HashSet<Type> allTypes = new();
            documentedTypes ??= new();
            documentedTypes.Add(objType);

            builder.Append("--- " + DumpingUtility.GetTypeNameNeat(objType));
            if (objType.IsEnum)
            {
                builder.Append(" (");
                builder.Append(Enum.GetUnderlyingType(objType).Name);
                builder.Append(')');
            }
            builder.AppendLine(" ---");

            DescriptionAttribute? objDescription = objType.GetCustomAttribute<DescriptionAttribute>();
            if (objDescription != null)
            {
                builder.AppendLine(objDescription.Description);
            }

            if (!objType.IsEnum)
            {
                IEnumerable<PropertyInfo> properties = objType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where((prop) => DumpingUtility.PropFilter(prop));

                foreach (PropertyInfo property in properties)
                {
                    if (property.GetCustomAttribute<SkipAttribute>() != null)
                    {
                        continue;
                    }

                    Type propertyType = property.PropertyType;
                    builder.Append("- ");
                    builder.Append(property.Name);
                    builder.Append(": ");
                    builder.AppendLine(DumpingUtility.GetTypeNameNeat(propertyType));

                    DescriptionAttribute? propertyDescription = property.GetCustomAttribute<DescriptionAttribute>();

                    if (propertyDescription == null)
                    {
                        builder.AppendLine("No description");
                    }
                    else
                    {
                        builder.AppendLine(propertyDescription.Description);
                    }
                    builder.AppendLine();

                    DumpingUtility.CollectTypes(propertyType, allTypes);
                }
            }
            else
            {
                Type underlingEnumType = Enum.GetUnderlyingType(objType);
                FieldInfo[] fields = objType.GetFields();
                // start at 1 because 0 is __value
                for (int index = 1; index < fields.Length; index++)
                {
                    FieldInfo field = fields[index];

                    if (field.GetCustomAttribute<SkipAttribute>() != null)
                    {
                        continue;
                    }

                    builder.Append("- ");
                    builder.Append(field.Name);
                    builder.Append(" [");
                    builder.Append(Convert.ChangeType(field.GetValue(null), underlingEnumType));
                    builder.AppendLine("]");

                    DescriptionAttribute? description = field.GetCustomAttribute<DescriptionAttribute>();
                    
                    if (description != null)
                    {
                        builder.AppendLine(description.Description);
                    }
                    builder.AppendLine();
                }
            }

            foreach (Type type in allTypes)
            {
                if (documentedTypes.Contains(type))
                {
                    continue;
                }

                DumpingUtility.Dump(builder, type, documentedTypes);
            }
        }

        private static string GetTypeNameNeat(Type type)
        {
            StringBuilder result = new();
            if (type.DeclaringType != null)
            {
                result.Append(DumpingUtility.GetTypeNameNeat(type.DeclaringType));
                result.Append('.');
            }
            else if (type.Namespace != null)
            {
                result.Append(type.Namespace);
                result.Append('.');
            }
            if (type.IsGenericType)
            {
                result.Append(type.Name.AsSpan(0, type.Name.LastIndexOf('`')));

                bool first = true;
                result.Append('<');
                foreach (Type t in type.GetGenericArguments())
                {
                    if (!first)
                    {
                        result.Append(", ");
                    }
                    result.Append(DumpingUtility.GetTypeNameNeat(t));
                }
                result.Append('>');
            }
            else
            {
                result.Append(type.Name);
            }
            return result.ToString();
        }

        private static void CollectTypes(Type type, ISet<Type> allTypes)
        {
            if (type.IsGenericParameter)
            {
                return;
            }

            if (type.IsGenericType)
            {
                foreach (Type parameter in type.GetGenericArguments())
                {
                    CollectTypes(parameter, allTypes);
                }
            }

            if (!DumpingUtility.IsDocumentableType(type))
            {
                return;
            }

            allTypes.Add(type);
        }

        private static bool IsDocumentableType(Type type)
        {
            if (DumpingUtility.builtInTypes.Contains(type))
            {
                return false;
            }

            if (type.GetCustomAttribute<SkipAttribute>() != null)
            {
                return false;
            }

            if (type.IsGenericType)
            {
                Type definition = type.GetGenericTypeDefinition();
                if (definition == typeof(List<>))
                {
                    return false;
                }
                else if (definition == typeof(Dictionary<,>))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool PropFilter(PropertyInfo property)
        {
            if (property.GetMethod == null || property.SetMethod == null)
            {
                return false;
            }

            if (property.GetMethod.IsPublic)
            {
                return property.GetCustomAttribute<JsonIgnoreAttribute>() == null;
            }

            return property.GetCustomAttribute<JsonIncludeAttribute>() != null;
        }
    }
}
