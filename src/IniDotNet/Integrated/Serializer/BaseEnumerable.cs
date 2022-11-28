using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IniDotNet.Integrated.Serializer;

public static class EnumerableSerializationUtils
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
    public static readonly Regex StringCommaRegex = new(@"(?<=[^,])(?:,,)*(,)");

    public IEnumerable<string> Deserialize(string? value)
    {
        if (value == null)
            return Enumerable.Empty<string>();

        List<string> ans = new();

        var values = StringCommaRegex.Matches(value);
        int lastStart = 0;
        for (int i = 0; i <= values.Count; ++i)
        {
            var end = i == values.Count ? value.Length : values[i].Groups[1].Index;
            ans.Add(value.Substring(lastStart, end - lastStart).RestoreIniEscapedString());
            lastStart = i == values.Count ? end : values[i].Groups[1].Index + values[i].Groups[1].Length;
        }

        return ans.AsReadOnly();
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
    private IntSerializer _ic = new();
    public int[] Deserialize(string? value)
    {
        return _sec.Deserialize(value).Select(x => _ic.Deserialize(x)).ToArray();
    }

    public string Serialize(int[]? value)
    {
        return _sec.Serialize(value?.Select(x => x.ToString()));
    }
}