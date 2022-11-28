using IniDotNet.Base;
using IniDotNet.Integrated.Handler;
using IniDotNet.Integrated.Model;
using IniDotNet.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IniDotNet.Integrated;



public class IntegratedIniDataHandler : IIniDataHandler
{

    public const int MAX_SEARCH_DEPTH = 114514;

    object? _data;
    public object? Data => _data;


    readonly Dictionary<Type, TypeRecord> _records;


    readonly Type _baseType;
    readonly string[] _basePath;
    readonly bool _allowGlobal;
    // readonly bool _allowSubsection;


    ISectionHandler? _globalHandler;
    Type? _realBaseType;
    readonly Stack<(string, ISectionHandler, Type)> _handlers;

    ISectionHandler GetCurrentHandler() => (_handlers.TryPeek(out var res) ? res.Item2 : _globalHandler) ?? throw new InvalidOperationException();
    Type GetCurrentType() => _handlers.TryPeek(out var res) ? res.Item3 : _realBaseType ?? throw new InvalidOperationException();

    public IntegratedIniDataHandler(Type baseType, string[]? basePath = null, bool allowGlobal = false)
    {
        _data = null;
        _records = new();
        _handlers = new();
        _globalHandler = null;

        _baseType = baseType;
        _basePath = basePath ?? new string[0];
        _allowGlobal = allowGlobal;
        // _allowSubsection = allowSubsection;

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
        if (_basePath.Length == 0)
        {
            _realBaseType = _baseType;
            _globalHandler = ConversionUtil.SelectTypeHandler(_records[_baseType]);
        }
        else
        {
            _globalHandler = new HashtableSectionHandler();
            _realBaseType = typeof(Hashtable);
        }
           
        _data = null;
        _globalHandler.Start();
    }

    public void Clear()
    {
        _data = null;
        _globalHandler = null;
    }

    public void End()
    {
        LeaveSection(0xffffffff);
        _data = GetCurrentHandler().End();
    }


    public void EnterSection(string section, uint line)
    {
        LeaveSection(line);
        EnterSubsection(section, line);
    }

    public void LeaveSection(uint line)
    {
        while (_handlers.Count > 0)
        {
            LeaveSubsection(line);
        }
    }

    public bool IsSectionEntered(string section, uint line)
    {
        return false;
        // discarded
    }

    public void HandleComment(string comment, uint line)
    {
        // do nothing at this time
    }


    public void HandleProperty(string key, string value, uint line)
    {
        if (_handlers.Count == 0 && !_allowGlobal)
            throw new ParsingException($"Invalid global property {key} = {value} encountered at line {line}.", line);
        GetCurrentHandler().Put(key, value);
    }

    public void EnterSubsection(string section, uint line)
    {
        var (handler, type) = GetCurrentHandler().GetSectionHandler(section);
        _handlers.Push((
            section, 
            handler ?? ConversionUtil.SelectTypeHandler(_records[type]), 
            type
            ));
        GetCurrentHandler().Start();
    }

    public void LeaveSubsection(uint line)
    {
        var (section, handler, _) = _handlers.Pop();
        GetCurrentHandler().PutObject(section, handler.End());
    }
}