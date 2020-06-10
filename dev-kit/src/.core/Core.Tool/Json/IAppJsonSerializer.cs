using System;

namespace Core.Tool.Json
{
    public interface IAppJsonSerializer
    {
        string Serialize(object o);
        T Deserialize<T>(string str);
        object Deserialize(string str, Type type);
    }
}