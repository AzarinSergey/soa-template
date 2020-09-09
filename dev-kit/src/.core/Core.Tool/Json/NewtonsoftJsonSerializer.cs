using System;
using Newtonsoft.Json;

namespace Core.Tool.Json
{
    public class NewtonsoftJsonSerializer : IAppJsonSerializer
    {
        public string Serialize(object o) => JsonConvert.SerializeObject(o);

        public T Deserialize<T>(string str) => JsonConvert.DeserializeObject<T>(str);

        public object Deserialize(string str, Type type) => JsonConvert.DeserializeObject(str, type);
    }
}