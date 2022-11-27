﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Integrated;



[AttributeUsage(AttributeTargets.Property)]
public class IniSectionAttribute : Attribute
{
    public string Name { get; set; } = "";

    public IniSectionAttribute()
    {
        Name = "";
    }
    public IniSectionAttribute(string name)
    {
        Name = name;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class IniKeyAttribute : Attribute
{
    public string Name { get; set; } = "";
    public IniKeyAttribute()
    {
        Name = "";
    }
    public IniKeyAttribute(string name)
    {
        Name = name;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class IniIgnoreAttribute : Attribute
{
    public IniIgnoreAttribute()
    {
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class IniConverterAttribute : Attribute
{
    public Type Type { get; set; }

    private static readonly Type IniType = typeof(IIniSerializer<string>).GetGenericTypeDefinition();

    public IniConverterAttribute(Type type)
    {
        if (!IniType.IsAssignableFrom(type.GetGenericTypeDefinition()))
            throw new InvalidDataException("Invalid type");
        Type = type;
    }
}
