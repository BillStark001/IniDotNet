using IniDotNet.Base;
using IniDotNet.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Integrated;
public class IntegratedIniHandler<T> : IIniDataHandler<T>
{

    T? _data;
    public T Data => _data ?? throw new InvalidOperationException("No parsed data");

    Dictionary<string, PropertyInfo> _propsKey;
    Dictionary<string, PropertyInfo> _propsSection;

    bool _isCtorNoParam;
    int _ctorParamLength;
    Dictionary<string, int> _ctorParamPos;
    ConstructorInfo _ctor;


    public IntegratedIniHandler(IniParserConfiguration conf, IniScheme? scheme = null)
    {
        _data = default(T);

        var inherit = true;
        (_propsKey, _propsSection) = AttributeUtil.GetTaggedPropertyList(typeof(T), inherit);
        var access = AttributeUtil.IsPropertiesFullyAccessible((_propsKey, _propsSection));
        if (!access)
        {
            var res = AttributeUtil.GetTaggedConstructor(typeof(T), (_propsKey, _propsSection), true, true);
            if (res.HasValue)
                (_ctorParamLength, _ctorParamPos, _ctor) = res.Value;
            else
                throw new InvalidOperationException("Unsupported type");
        }
        else
        {
            _ctorParamLength = 0;
            _ctorParamPos = new();
            _ctor = AttributeUtil.GetNoParamConstructor(typeof(T), true) ?? throw new InvalidOperationException("Unsupported type");
        }

        _isCtorNoParam = access;
    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public void End()
    {
        throw new NotImplementedException();
    }

    public T Get()
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