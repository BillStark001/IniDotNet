using IniDotNet.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IniDotNet.Integrated;

public interface IIniConverter<T>
{

    public string Serialize(T? value);
    public T? Deserialize(string? value);
}

public class StringConverter : IIniConverter<string>
{
    public string? Deserialize(string? value)
    {
        return value;
    }

    public string Serialize(string? value)
    {
        return value ?? "";
    }
}

public class BoolConverter : IIniConverter<bool>
{
    public bool Deserialize(string? value)
    {
        return value?.ToLower().Trim() == "true";
    }

    public string Serialize(bool value)
    {
        return value ? "true" : "false";
    }
}

public class IntConverter : IIniConverter<int>
{
    public int Deserialize(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;
        try
        {
            return int.Parse(value);
        }
        catch (FormatException)
        {
            return 0;
        }
    }

    public string Serialize(int value)
    {
        return value.ToString();
    }
}



public class StringEnumerableConverter : IIniConverter<IEnumerable<string>>
{

    private static Regex CommaRegex = new Regex(@", *");

    public IEnumerable<string> Deserialize(string? value)
    {
        if (value == null)
            return Enumerable.Empty<string>();

        var values = CommaRegex.Split(value);
        return values.Select(x => x.RestoreIniEscapedString());
    }

    public string Serialize(IEnumerable<string>? values)
    {
        if (values == null || values.Count() == 0)
            return "";
        StringBuilder sb = new StringBuilder();
        bool flag = false;
        foreach (var value in values)
        {
            if (flag)
                sb.Append(',');
            flag = true;
            sb.Append(value.GetIniEscapedString());
        }
        return sb.ToString();
    }
}

public class StringArrayConverter : IIniConverter<string[]>
{
    private StringEnumerableConverter _sec = new();
    public string[] Deserialize(string? value)
    {
        return _sec.Deserialize(value).ToArray();
    }

    public string Serialize(string[]? value)
    {
        return _sec.Serialize(value);
    }
}

public class IntArrayConverter : IIniConverter<int[]>
{
    private StringEnumerableConverter _sec = new();
    private IntConverter _ic = new();
    public int[] Deserialize(string? value)
    {
        return _sec.Deserialize(value).Select(x => _ic.Deserialize(x)).ToArray();
    }

    public string Serialize(int[]? value)
    {
        return _sec.Serialize(value?.Select(x => x.ToString()));
    }
}
