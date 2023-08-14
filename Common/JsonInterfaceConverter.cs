using System;
using System.Text.Json.Serialization;

namespace Common
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class JsonInterfaceConverterAttribute : JsonConverterAttribute
    {
        public JsonInterfaceConverterAttribute(Type convreterType) : base(convreterType)
        {
            
        }
    }
}
