using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IniDotNet.Util;

public static class StringUtil
{



    public static readonly Regex ValueReplacePattern = new Regex(@"(?:\\[\\;#=:nrtbfa0]|\\u[0-9a-fA-F]{4}|\\x[0-9a-fA-F]{2}|\\u\{[0-9a-fA-F]+\})");
    public static string EvaluateMatch(Match match)
    {
        if (match.Length < 2)
            return match.Value;
        switch (match.Value[1])
        {
            case '\\':
            case ';':
            case '#':
            case '=':
            case ':':
                return match.Value[1].ToString();

            case '0':
                return "\0";
            case 'n':
                return "\n";
            case 'r':
                return "\r";
            case 't':
                return "\t";
            case 'f':
                return "\f";
            case 'b':
                return "\b";
            case 'a':
                return "\a";

            case 'x':
                return ((char)int.Parse(match.Value.Substring(2), System.Globalization.NumberStyles.HexNumber)).ToString();
            case 'u':
                if (match.Value[2] == '{')
                    return char.ConvertFromUtf32(int.Parse(match.Value.Substring(3, match.Length - 4), System.Globalization.NumberStyles.HexNumber));
                return char.ConvertFromUtf32(int.Parse(match.Value.Substring(2), System.Globalization.NumberStyles.HexNumber));
            default:
                throw new InvalidOperationException();
        }
    }

    public static string ParseValue(string str)
    {
        return ValueReplacePattern.Replace(str, EvaluateMatch);
    }

    public static string CreateValue(string str)
    {
        var ret = str
            .Replace("\\", "\\\\")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t")
            .Replace("\f", "\\f")
            .Replace("\b", "\\b");
        return ret;
    }

}