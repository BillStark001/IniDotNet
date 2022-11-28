using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniDotNet.Integrated.Serializer;

namespace IniDotNet.Integrated;


public enum IniType
{
    Auto = 0, 
    Key = 1, 
    Section = 2, 
    // Comment = 4
}

[AttributeUsage(AttributeTargets.Class)]
public class IniModelAttribute : Attribute
{

}


[AttributeUsage(AttributeTargets.Property)]
public class IniPropertyAttribute : Attribute
{
    public string Name { get; set; } = "";
    public IniType Type { get; set; } = IniType.Auto;

    public IniPropertyAttribute()
    {
    }
    public IniPropertyAttribute(string name)
    {
        Name = name;
    }
    public IniPropertyAttribute(string name, IniType type) : this(name)
    {
        Type = type;
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
public class IniSerializerAttribute : Attribute
{
    public Type Type { get; set; }

    private static readonly Type IniType = typeof(IIniSerializer<string>).GetGenericTypeDefinition();

    public IniSerializerAttribute(Type type)
    {
        if (!IniType.IsAssignableFrom(type.GetGenericTypeDefinition()))
            throw new InvalidDataException("Invalid type");
        Type = type;
    }
}
