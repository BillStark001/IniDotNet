﻿using IniDotNet.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IniDotNet.Integrated;


public class StringConverter : IIniSerializer<string>
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

public class BoolConverter : IIniSerializer<bool>
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

public class IntConverter : IIniSerializer<int>
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


public static class StringSerializationUtils
{


    public static string GetIniEscapedString(this string strIn)
    {
        return strIn.Replace(",", ",,");
    }


    public static string RestoreIniEscapedString(this string strIn)
    {
        return strIn.Replace(",,", ",");
    }
}

public class StringEnumerableConverter : IIniSerializer<IEnumerable<string>>
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

public class StringArrayConverter : IIniSerializer<string[]>
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

public class IntArrayConverter : IIniSerializer<int[]>
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
