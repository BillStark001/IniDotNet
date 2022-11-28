using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Integrated.Serializers;

public class StringSerializer : IIniSerializer<string>
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

public class BoolSerializer : IIniSerializer<bool>
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

public class IntSerializer : IIniSerializer<int>
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


public class LongSerializer : IIniSerializer<long>
{
    public long Deserialize(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;
        try
        {
            return long.Parse(value);
        }
        catch (FormatException)
        {
            return 0;
        }
    }

    public string Serialize(long value)
    {
        return value.ToString();
    }
}

public class DoubleSerializer : IIniSerializer<double>
{
    public double Deserialize(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;
        try
        {
            return double.Parse(value);
        }
        catch (FormatException)
        {
            return 0;
        }
    }

    public string Serialize(double value)
    {
        return value.ToString();
    }
}

