using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using IniDotNet.Model;
using IniDotNet.Base;

namespace IniDotNet
{
    /// <summary>
	/// 	Responsible for parsing an string from an ini file, and creating
	/// 	an <see cref="IniData"/> structure.
	/// </summary>
    public partial class IniDataParser : IniParser
    {
        #region Initialization
        /// <summary>
        ///     Ctor
        /// </summary>
        public IniDataParser() : base()
        {
        }

        #endregion


        /// <summary>
        ///     Parses a string containing valid ini data
        /// </summary>
        /// <param name="iniString">
        ///     String with data in INI format
        /// </param>
        public IniData Parse(string iniString)
        {
            return Parse(new StringReader(iniString));
        }

        /// <summary>
        ///     Parses a string containing valid ini data
        /// </summary>
        /// <param name="textReader">
        ///     Text reader for the source string contaninig the ini data
        /// </param>
        /// <returns>
        ///     An <see cref="IniData"/> instance containing the data readed
        ///     from the source
        /// </returns>
        /// <exception cref="ParsingException">
        ///     Thrown if the data could not be parsed
        /// </exception>
        public IniData Parse(TextReader textReader)
        {
            DefaultIniDataHandler iniData = new();
            iniData.Configuration = Configuration;

            Parse(textReader, iniData);

            return iniData.Get();
        }

    }
}
