using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using IniDotNet.Base;
using IniDotNet.Linq;

namespace IniDotNet
{
    /// <summary>
	/// 	Responsible for parsing an string from an ini file, and creating
	/// 	an <see cref="IniObject"/> structure.
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
        public IniObject Parse(string iniString)
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
        ///     An <see cref="IniObject"/> instance containing the data readed
        ///     from the source
        /// </returns>
        /// <exception cref="ParsingException">
        ///     Thrown if the data could not be parsed
        /// </exception>
        public IniObject Parse(TextReader textReader)
        {
            IniObjectHandler iniData = new();
            iniData.Configuration = Configuration;

            Parse(textReader, iniData);

            return iniData.Get();
        }

    }
}
