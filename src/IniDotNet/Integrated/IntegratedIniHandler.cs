using IniDotNet.Base;
using IniDotNet.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IniDotNet.Integrated;


public class TypeRecord
{
    #region Fields
    readonly Type _t;

    readonly Dictionary<string, PropertyInfo> _propsKey;
    readonly Dictionary<string, PropertyInfo> _propsSection;

    readonly Dictionary<string, IniSerializerRecord> _serializerRecords;

    readonly bool _isCtorNoParam;
    readonly int _ctorParamLength;
    readonly Dictionary<string, int> _ctorParamPos;
    readonly ConstructorInfo _ctor;

    #endregion


    public Type Type => _t;

    public bool HasNoParamConstructor => _isCtorNoParam;
    public int ConstructorParamLength => _ctorParamLength;
    public ConstructorInfo Constructor => _ctor;

    public TypeRecord(Type t, bool inherit = true, bool omitIncomplete = true, bool forcePublic = true)
    {
        _t = t;
        _serializerRecords = new();

        (_propsKey, _propsSection) = AttributeUtil.GetTaggedPropertyList(t, inherit);
        var access = AttributeUtil.IsPropertiesFullyAccessible((_propsKey, _propsSection));
        if (!access)
        {
            var res = AttributeUtil.GetTaggedConstructor(t, (_propsKey, _propsSection), omitIncomplete, forcePublic);
            if (res.HasValue)
                (_ctorParamLength, _ctorParamPos, _ctor) = res.Value;
            else
                throw new InvalidOperationException("Unsupported type");
        }
        else
        {
            _ctorParamLength = 0;
            _ctorParamPos = new();
            _ctor = AttributeUtil.GetNoParamConstructor(t, forcePublic) ?? throw new InvalidOperationException("Unsupported type");
        }

        _isCtorNoParam = access;
        foreach (var (propName, propInfo) in _propsKey)
        {
            var cvrtType = propInfo.GetCustomAttribute<IniConverterAttribute>()?.Type;
            IniSerializerRecord? cvrt = null;
            if (cvrtType != null)
                cvrt = ConversionUtil.TryGetConverterInnerRefl(propInfo.PropertyType, cvrtType);
            else
                cvrt = ConversionUtil.TryGetConverterRefl(propInfo.PropertyType, AppDomain.CurrentDomain.GetAssemblies());
            if (cvrt == null && !omitIncomplete)
                throw new InvalidOperationException("Incompleted type");
            if (cvrt != null)
                _serializerRecords[propName] = cvrt;

        }

    }

    public IEnumerable<Type> GetReferenceTypes()
    {
        return _propsSection.Values
            .Select(x => x.DeclaringType)
            .Where(x => x != null)
            .Select(x => x!);
    }

    public IniSerializerRecord? TryGetSerializer(string name)
    {
        return _serializerRecords.TryGetValue(name, out var ret) ? ret : null;
    }

    public PropertyInfo? TryGetProperty(string name)
    {
        var succ = _propsKey.TryGetValue(name, out var ret);
        if (!succ)
            succ = _propsSection.TryGetValue(name, out ret);
        return succ ? ret : null; 
    }

    public bool IsKeyProperty(string x) => _propsKey.ContainsKey(x);
    public bool IsSectionProperty(string x) => _propsSection.ContainsKey(x);
    public int GetConstructorPosition(string x) => _ctorParamPos.TryGetValue(x.ToLower(), out var ret) ? ret : -1;

}

public class SubIniHandler
{
    readonly TypeRecord _record;

    public TypeRecord Record => _record;

    object? _data;
    Dictionary<string, object?> _params;

    public SubIniHandler(TypeRecord record)
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

public class IntegratedIniHandler : IIniDataHandler
{

    public const int MAX_SEARCH_DEPTH = 114514;

    object? _data;
    public object? Data => _data;


    readonly Dictionary<Type, TypeRecord> _records;


    public IntegratedIniHandler(Type baseType, IniParserConfiguration conf, IniScheme? scheme = null)
    {
        _data = null;
        _records = new();

        Queue<Type> _types = new();
        _types.Enqueue(baseType);

        int i = MAX_SEARCH_DEPTH;

        while (_types.Count > 0 && i > 0)
        {
            var type = _types.Dequeue();
            if (_records.ContainsKey(type))
                continue;

            i--;
            _records[type] = new(type, true, true, true);
            foreach (var subType in _records[type].GetReferenceTypes())
                _types.Enqueue(subType);
        }
        if (_types.Count > 0)
            throw new InvalidOperationException("Maximum search depth exceeded!");
    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        _data = null;
    }

    public void End()
    {
        throw new NotImplementedException();
    }


    public void EnterSection(string section, uint line)
    {
        throw new NotImplementedException();
    }

    public bool IsSectionEntered(string section, uint line)
    {
        throw new NotImplementedException();
    }

    public void HandleComment(string comment, uint line)
    {
        throw new NotImplementedException();
    }

    public void HandleMultilineProperty(string? key, string value, uint line)
    {
        throw new NotImplementedException();
    }

    public void HandleProperty(string key, string value, uint line)
    {
        throw new NotImplementedException();
    }


}