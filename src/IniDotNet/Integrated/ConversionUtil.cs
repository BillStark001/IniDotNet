using IniDotNet.Integrated.Handler;
using IniDotNet.Integrated.Model;
using IniDotNet.Integrated.Serializer;
using IniDotNet.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Integrated;


using DSSP = Dictionary<(string, string), PropertyInfo>;




public static class ConversionUtil
{
    public static bool IsStringDictionary(Type t)
    {
        if (!typeof(IDictionary).IsAssignableFrom(t))
            return false;

        // hashtable
        if (t.IsEquivalentTo(typeof(Hashtable)))
            return true;

        if (t.GetGenericTypeDefinition().IsEquivalentTo(typeof(SortedDictionary<int, int>).GetGenericTypeDefinition()))
            throw new NotImplementedException();

        if (!(t.GetGenericArguments().Length == 2))
            return false;
        var args = t.GetGenericArguments();
        if (!args[0].IsEquivalentTo(typeof(string)))
            return false;

        return true;
    }


    public static ISectionHandler SelectTypeHandler(TypeRecord record)
    {
        if (record.Type.IsEquivalentTo(typeof(Hashtable)))
            return new HashtableSectionHandler();
        else if (IsStringDictionary(record.Type))
        {
            var dictType = typeof(DictionarySectionHandler<int>)
                .GetGenericTypeDefinition()
                .MakeGenericType(new Type[] { record.Type.GetGenericArguments()[1] });

            return (ISectionHandler)dictType.GetConstructors()[0].Invoke(new object?[] { null });
        }
        else
            return new ClassSectionHandler(record);
    }

    public static SerializerRecord? TryGetConverter<T>(Assembly[] assembly)
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


    public static SerializerRecord? TryGetConverterInner<T>(Type cvrtType)
    {
        // make sure the constructor is available
        foreach (var ctor in cvrtType.GetConstructors())
        {
            if (ctor.GetParameters().Length == 0 && ctor.IsPublic)
            {
                var cvrt = (IIniSerializer<T>)ctor.Invoke(new object[] { }) ?? throw new InvalidOperationException("Serializer construction failed");
                var record = new SerializerRecord<T>(cvrt, cvrt.Serialize, cvrt.Deserialize);

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

    public static SerializerRecord? TryGetConverterRefl(Type reflType, Assembly[] assembly)
    {
        return (SerializerRecord?)_cvrt.MakeGenericMethod(new Type[] { reflType }).Invoke(null, new[] { assembly });
    }

    public static SerializerRecord? TryGetConverterInnerRefl(Type reflType, Type cvrtType)
    {
        return (SerializerRecord?)_cvrt2.MakeGenericMethod(new Type[] { reflType }).Invoke(null, new[] { cvrtType });
    }


}