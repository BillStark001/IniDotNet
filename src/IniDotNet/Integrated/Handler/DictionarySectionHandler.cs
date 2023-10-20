using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniDotNet.Integrated.Model;
using IniDotNet.Util;

namespace IniDotNet.Integrated.Handler;

public class DictionarySectionHandler<T> : ISectionHandler
{

    SerializerRecord _serializer;
    public DictionarySectionHandler(SerializerRecord? serializer = null)
    {
        _serializer = serializer ??
            ConvertUtil.TryGetConverter<T>(AppDomain.CurrentDomain.GetAssemblies()) ??
            throw new InvalidOperationException("Unsupported type");
    }

    Dictionary<string, T>? _data;

    public void Start()
    {
        _data = new();
    }

    public void Put(string key, string value)
    {
        var ser = _serializer.Deserialize(value);
        if (ser != null)
            _data![key] = (T)ser;
    }

    public void PutObject(string key, object? value)
    {
        if (value != null && typeof(T).IsAssignableFrom(value.GetType()))
            _data![key] = (T)value;
    }

    public object? End()
    {
        return _data;
    }

    public (ISectionHandler?, Type) GetSectionHandler(string section)
    {
        return (null, typeof(T));
    }
}
