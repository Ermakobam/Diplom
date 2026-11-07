using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Athena.Service.ResearchEvents.Utils
{
    class JsonPathConverter : JsonConverter
    {
        private readonly DefaultContractResolver _contractResolver;
        public JsonPathConverter(DefaultContractResolver defaultContractResolver = null)
        {
            defaultContractResolver ??= new() { NamingStrategy = new CamelCaseNamingStrategy() };
            _contractResolver = defaultContractResolver;
        }
        public override object ReadJson(JsonReader reader, Type objectType,
                                        object existingValue, JsonSerializer serializer)
        {
            var jo = JToken.Load(reader);
            object targetObj = Activator.CreateInstance(objectType);

            foreach (PropertyInfo prop in objectType.GetProperties()
                                                    .Where(p => p.CanRead && p.CanWrite))
            {
                JsonPropertyAttribute att = prop.GetCustomAttributes(true)
                                                .OfType<JsonPropertyAttribute>()
                                                .FirstOrDefault();

                string jsonPath = att != null ? att.PropertyName : _contractResolver.GetResolvedPropertyName(prop.Name);
                JToken token = jo.SelectToken(jsonPath);

                if (token != null && token.Type != JTokenType.Null)
                {
                    object value = token.ToObject(prop.PropertyType, serializer);
                    prop.SetValue(targetObj, value, null);
                }
            }

            return targetObj;
        }

        public override bool CanConvert(Type objectType)
        {
            return !typeof(IEnumerable).IsAssignableFrom(objectType) && !objectType.IsPrimitive && !objectType.Equals(typeof(string)) && !objectType.IsValueType;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value,
                                       JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
