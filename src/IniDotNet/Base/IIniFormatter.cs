using IniDotNet.Linq;

namespace IniDotNet.Base;

public interface IIniFormatter<T>
{
    /// <summary>
    ///     Produces an string for a given <see cref="IniObject"/> structure
    /// </summary>
    /// <returns>String that represents an <see cref="IniObject"/>.</returns>
    /// <param name="data">Ini data.</param>
    /// <param name="config">
    ///     Configuration used by this formatter when converting IniObject
    ///     to an string
    /// </param>
    string Format(T data, IniFormatterConfig config);
}