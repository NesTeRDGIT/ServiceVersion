using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ServiceLoaderMedpomData
{
    public class DecimalFormatAttribute : Attribute
    {
        public string FORMAT { get; set; }

    }


    public static class XmlSerializerExtensions
    {
        // the target format of the decimal precision, change to your needs


        public static void SerializeWithDecimalFormatting(this XmlSerializer serializer, XmlWriter writer, object o, XmlSerializerNamespaces ns)
        {
            IteratePropertiesRecursively(o);
            serializer.Serialize(writer, o, ns);
        }

        private static void IteratePropertiesRecursively(object o)
        {
            if (o == null)
                return;

            var type = o.GetType();
            var properties = type.GetProperties();

            // enumerate the properties of the type
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;

                // if property is a generic list
                if (propertyType.Name == "List`1")
                {
                    var val = property.GetValue(o, null);

                    if (val is IList elements)
                    {
                        // then iterate through all elements
                        foreach (var item in elements)
                        {
                            if (IsClass(item.GetType()))
                                IteratePropertiesRecursively(item);

                        }
                    }
                    continue;
                }

                if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
                {
                    // check if there is a property with name XXXSpecified, this is the case if we have a type of decimal?
                    SetFormatDecimal(property, type, o);
                    continue;
                }

                // if property is a XML class (contains XML in name) iterate through properties of this class
                if (IsClass(propertyType))
                {
                    IteratePropertiesRecursively(property.GetValue(o));
                }

            }
        }

        private static Type[] baseType = { typeof(int), typeof(int?), typeof(string), typeof(decimal), typeof(decimal?), typeof(DateTime), typeof(DateTime?) };

        private static bool IsClass(Type t)
        {
            return !baseType.Contains(t);
        }

        private static void SetFormatDecimal(PropertyInfo property, Type type, object o)
        {
            var Format = Attribute.GetCustomAttributes(property, typeof(DecimalFormatAttribute)).Select(x => (DecimalFormatAttribute)x).FirstOrDefault();


            if (Format != null)
            {
                var specifiedPropertyName = $"{property.Name}Specified";
                var isSpecifiedProperty = type.GetProperty(specifiedPropertyName);

                if (isSpecifiedProperty != null)
                {
                    // only apply the format if the value of XXXSpecified is true, otherwise we will get a nullRef exception for decimal? types
                    var isSpecifiedPropertyValue = isSpecifiedProperty.GetValue(o, null) as bool?;
                    if (isSpecifiedPropertyValue == true)
                    {
                        FormatDecimal(property, o, Format.FORMAT);
                    }
                }
                else
                {
                    // if there is no property with name XXXSpecified, we can safely format the decimal
                    FormatDecimal(property, o, Format.FORMAT);
                }
            }
        }
        private static void FormatDecimal(PropertyInfo p, object o, string FORMAT)
        {
            if (o != null)
            {
                // if property is decimal, apply correct number format
                var value = (decimal?)p.GetValue(o, null);
                if (value != null)
                {
                    var formattedString = value.Value.ToString(FORMAT, CultureInfo.InvariantCulture);
                    p.SetValue(o, Convert.ToDecimal(formattedString, CultureInfo.InvariantCulture), null);
                }

            }

        }

    }
}
