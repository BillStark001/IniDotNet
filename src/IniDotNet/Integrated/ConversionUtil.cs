using IniDotNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Integrated;


using DSSP = Dictionary<(string, string), PropertyInfo>;




public static class ConversionUtil
{

    public static IniSerializerRecord? TryGetConverter<T>(Assembly[] assembly)
    {
        var type = typeof(IIniSerializer<T>);
        var typeT = typeof(T);
        var typeCvrt = type.GetGenericTypeDefinition();
        var types = assembly.SelectMany(s => s.GetTypes())
            .Where(p =>
            {
                if (type.IsAssignableFrom(p)) // direct implementation
                    return true;

                // form of A<B> where A: IIniConverter, B: T
                // hence we can only return object
                // but I think this is not necessary after finished it
                /*
                var generics = p.GenericTypeArguments;
                if (generics.Length == 1 
                && typeCvrt.IsAssignableFrom(p.GetGenericTypeDefinition())
                && typeT.IsAssignableFrom(generics[0])) 
                {
                    return true;
                }
                */
                return false;
            });

        if (types.Count() == 0)
            return null;
        var type1 = types.First();

        return TryGetConverterInner<T>(type1);
    }


    public static IniSerializerRecord? TryGetConverterInner<T>(Type cvrtType)
    {
        // make sure the constructor is available
        foreach (var ctor in cvrtType.GetConstructors())
        {
            if (ctor.GetParameters().Length == 0 && ctor.IsPublic)
            {
                var cvrt = (IIniSerializer<T>)ctor.Invoke(new object[] { }) ?? throw new InvalidOperationException("Serializer construction failed");
                var record = new IniSerializerRecord<T>(cvrt, cvrt.Serialize, cvrt.Deserialize);

                return record;
            }

        }
        // throw new InvalidOperationException($"No possible converter found for type {typeof(T).FullName}.");
        return null;
    }


    private static readonly MethodInfo _cvrt;
    private static readonly MethodInfo _cvrt2;
    static ConversionUtil()
    {
        _cvrt = typeof(ConversionUtil).GetMethod(nameof(TryGetConverter))!;
        _cvrt2 = typeof(ConversionUtil).GetMethod(nameof(TryGetConverterInner))!;
    }

    public static IniSerializerRecord? TryGetConverterRefl(Type reflType, Assembly[] assembly)
    {
        return (IniSerializerRecord?)_cvrt.MakeGenericMethod(new Type[] { reflType }).Invoke(null, new[] { assembly });
    }

    public static IniSerializerRecord? TryGetConverterInnerRefl(Type reflType, Type cvrtType)
    {
        return (IniSerializerRecord?)_cvrt2.MakeGenericMethod(new Type[] { reflType }).Invoke(null, new[] { cvrtType });
    }


}