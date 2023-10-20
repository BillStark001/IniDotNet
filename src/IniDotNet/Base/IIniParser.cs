using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Base;

public interface IIniParser
{
    IniParserConfig Configuration { get; }
    void Reset();


    /// <summary>
    ///     Parses a string containing valid ini data
    /// </summary>
    /// <param name="textReader">
    ///     Text reader for the source string contaninig the ini data
    /// </param>
    /// <exception cref="ParsingException">
    ///     Thrown if the data could not be parsed
    /// </exception>  
    void Parse(TextReader textReader, IIniHandler iniData);
}