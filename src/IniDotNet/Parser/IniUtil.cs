using IniDotNet.Format;
using IniDotNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Parser;


using DSSP = Dictionary<(string, string), PropertyInfo>;




public static class IniUtils
{
    // TODO
    public static DSSP GetTaggedPropertyList(Type type)
    {
        DSSP ans = new();
        string defaultSection = "";
        foreach (var a in type.GetCustomAttributes<IniSectionAttribute>(true))
            defaultSection = a.Name ?? defaultSection;
        foreach (var prop in type.GetProperties())
        {
            if (prop.GetCustomAttributes<IniIgnoreAttribute>(true).Count() > 0)
                continue;

            string sec = defaultSection;
            string key = prop.Name;
            foreach (var a in prop.GetCustomAttributes<IniSectionAttribute>(true))
                sec = a.Name ?? sec;
            foreach (var a in prop.GetCustomAttributes<IniKeyAttribute>(true))
                key = a.Name;

            if (!string.IsNullOrWhiteSpace(key))
                ans[(sec, key)] = prop;
        }
        return ans;
    }

    public static IEnumerable<Type> GetTaggedTypes<T>(Assembly[] assembly) where T: Attribute
    {
        foreach (Type type in assembly.SelectMany(s => s.GetTypes()))
        {
            if (type.GetCustomAttributes(typeof(T), true).Length > 0)
            {
                yield return type;
            }
        }
    }

    public static IEnumerable<Type> GetImplementations<T>(Assembly[] assembly)
    {
        var type = typeof(T);
        var types = assembly.SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p));
        return types;
    }

    public static IIniConverter<T>? TryGetConverter<T>(Assembly[] assembly)
    {
        var types = GetImplementations<IIniConverter<T>>(assembly);
        if (types.Count() == 0)
            return null;
        var type = types.First();
        foreach (var ctor in type.GetConstructors())
        {
            if (ctor.GetParameters().Length == 0)
                return (IIniConverter<T>)ctor.Invoke(new object[] { });
        }
        // throw new InvalidOperationException($"No possible converter found for type {typeof(T).FullName}.");
        return null;
    }


    private static readonly MethodInfo _cvrt;
    static IniUtils()
    {
        _cvrt = typeof(IniUtils).GetMethod(nameof(TryGetConverter))!;
    }

    public static object? TryDeserialize(Assembly[] asm, string s, Type t)
    {
        var cvrt = _cvrt.MakeGenericMethod(new Type[] { t }).Invoke(null, new[] { asm });
        if (cvrt == null)
            throw new InvalidOperationException($"No possible converter found for type {t.FullName}.");
        return cvrt.GetType().GetMethod("Deserialize")!.Invoke(cvrt, new[] { s });
    }

    public static string TrySerialize(Assembly[] asm, object? o, Type t)
    {
        var cvrt = _cvrt.MakeGenericMethod(new Type[] { t }).Invoke(null, new[] { asm });
        if (cvrt == null)
            throw new InvalidOperationException($"No possible converter found for type {t.FullName}.");
        return (string) cvrt.GetType().GetMethod("Serialize")!.Invoke(cvrt, new[] { o })!;
    }

    public static string GetIniEscapedString(this string strIn)
    {
        return strIn.Replace(",", ",,");
    }


    public static string RestoreIniEscapedString(this string strIn)
    {
        return strIn.Replace(",,", ",");
    }

    public static void GetCtorPropertyList(Type type)
    {
        // TODO
    }
}