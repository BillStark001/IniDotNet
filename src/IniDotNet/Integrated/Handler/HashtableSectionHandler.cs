using IniDotNet.Integrated.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Integrated.Handler;

public class HashtableSectionHandler : ISectionHandler
{
    public HashtableSectionHandler()
    {
    }

    Hashtable? _data;

    public void Start()
    {
        _data = new();
    }

    public void Put(string key, string value)
    {
        _data![key] = value;
    }

    public void PutObject(string key, object? value)
    {
        _data![key] = value;
    }

    public object? End()
    {
        return _data;
    }

    public (ISectionHandler?, Type) GetSectionHandler(string section)
    {
        return (new HashtableSectionHandler(), typeof(Hashtable));
    }
}
