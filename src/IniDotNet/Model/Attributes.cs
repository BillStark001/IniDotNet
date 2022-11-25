using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Model;



[AttributeUsage(
    AttributeTargets.Property |
    AttributeTargets.Class |
    AttributeTargets.Struct
    )]
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

public class IniConverterAttribute : Attribute
{
    public Type TargetType { get; set; } = typeof(string);
    public IniConverterAttribute(Type targetType)
    {
        TargetType = targetType ?? typeof(string);
    }
}
