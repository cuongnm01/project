using Newtonsoft.Json.Linq;

namespace Common.Helpers
{
    public static class ConverterHelper
    {
        public static T TryGetValue<T>(this JObject jObject, params string[] keys)
        {
            try
            {
                JToken jToken = jObject;
                foreach (var key in keys)
                {
                    jToken = jToken?.SelectToken(key);
                }
                return jToken.Value<T>();
            }
            catch
            {
                return default;
            }
        }
    }
}
