using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Integrated;
public class IniSerializerRecord
{
    private readonly MethodInfo _serializer;
    private readonly MethodInfo _deserializer;

    private readonly object _cvrt;

    public IniSerializerRecord(object cvrt, MethodInfo serializer, MethodInfo deserializer)
    {
        _cvrt = cvrt;
        _serializer = serializer;
        _deserializer = deserializer;
    }

    public virtual string Serialize(object? o)
    {
        return _serializer.Invoke(_cvrt, new[] { o })?.ToString() ?? "";
    }

    public virtual object? Deserialize(string? s)
    {
        return _deserializer.Invoke(_cvrt, new[] { s });
    }
}

public class IniSerializerRecord<T> : IniSerializerRecord
{
    private readonly Func<T?, string> _serializer;
    private readonly Func<string?, T?> _deserializer;

    public IniSerializerRecord(object cvrt, Func<T?, string> serializer, Func<string?, T?> deserializer) : base(cvrt, null!, null!)
    {
        _serializer = serializer;
        _deserializer = deserializer;
    }

    public override string Serialize(object? o)
    {
        return _serializer((T?)o);
    }

    public override object? Deserialize(string? s)
    {
        return _deserializer(s);
    }
}