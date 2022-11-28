using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniDotNet.Integrated.Model;

namespace IniDotNet.Integrated.Handler;

public class ClassSectionHandler : ISectionHandler
{
    readonly TypeRecord _record;


    object? _data;
    Dictionary<string, object?> _params;

    public ClassSectionHandler(TypeRecord record)
    {
        _record = record;
        _params = new();
    }

    public void Start()
    {
        if (_record.HasNoParamConstructor)
            _data = _record.Constructor.Invoke(new object?[0]);
        else
            _params = new();
    }

    public void Put(string key, string value)
    {
        var ser = _record.TryGetSerializer(key);
        if (ser == null)
            return;

        var vobj = ser.Deserialize(value);
        PutObject(key, vobj);
    }

    public void PutObject(string key, object? value)
    {

        if (_record.HasNoParamConstructor)
            _record.TryGetProperty(key)?.SetValue(_data, value);
        else
            _params[key] = value;
    }

    public object? End()
    {
        if (_record.HasNoParamConstructor)
            return _data;

        object?[] ctorParams = new object?[_record.ConstructorParamLength];
        foreach (var (key, value) in _params)
        {
            var pos = _record.GetConstructorPosition(key);
            if (pos >= 0 && pos < ctorParams.Length)
                ctorParams[pos] = value;
        }
        return _record.Constructor.Invoke(ctorParams);

    }
}