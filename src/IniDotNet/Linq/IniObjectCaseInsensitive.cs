using System;
using IniDotNet.Base;

namespace IniDotNet.Linq;

/// <summary>
///     Represents all data from an INI file exactly as the <see cref="IniObject"/>
///     class, but searching for sections and keys names is done with
///     a case insensitive search.
/// </summary>
public class IniObjectCaseInsensitive : IniObject
{
    /// <summary>
    ///     Initializes an empty IniObject instance.
    /// </summary>
    public IniObjectCaseInsensitive()
    {
        Sections = new IniSectionCollection(StringComparer.OrdinalIgnoreCase);
        Global = new IniPropertyCollection(StringComparer.OrdinalIgnoreCase);
        _scheme = new IniScheme();
    }

    public IniObjectCaseInsensitive(IniScheme scheme)
    {
        Sections = new IniSectionCollection(StringComparer.OrdinalIgnoreCase);
        Global = new IniPropertyCollection(StringComparer.OrdinalIgnoreCase);
        _scheme = scheme.DeepClone();
    }


    /// <summary>
    /// Copies an instance of the <see cref="IniObjectCaseInsensitive"/> class
    /// </summary>
    /// <param name="ori">Original </param>
    public IniObjectCaseInsensitive(IniObject ori)
        : this()
    {
        Global = ori.Global.DeepClone();
        Configuration = ori.Configuration.DeepClone();
        Sections = new IniSectionCollection(ori.Sections, StringComparer.OrdinalIgnoreCase);
    }
}