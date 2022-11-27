using System;
using System.Collections.Generic;
using IniDotNet.Exceptions;
using System.Collections.ObjectModel;
using System.IO;
using IniDotNet.Parser;
using IniDotNet.Model;
using IniDotNet.Base;

namespace IniDotNet
{
    /// <summary>
	/// 	Responsible for parsing an string from an ini file, and creating
	/// 	an <see cref="IniData"/> structure.
	/// </summary>
    public class IniParser
    {
        #region Initialization
        /// <summary>
        ///     Ctor
        /// </summary>
        public IniParser()
        {
            Configuration = new IniParserConfiguration();
            Scheme = new(
                Configuration.AllowNumberSignComments,
                Configuration.UseEscapeCharacters,
                Configuration.UseColonSeparator
                );
            _errorExceptions = new List<Exception>();
        }

        #endregion

        #region State

        public virtual IniParserConfiguration Configuration { get; protected set; }

        /// <summary>
        ///     Scheme that defines the structure for the ini file to be parsed
        /// </summary>
        public IniScheme Scheme { get; protected set; }

        /// <summary>
        /// True is the parsing operation encounter any problem
        /// </summary>
        public bool HasError { get { return _errorExceptions.Count > 0; } }

        /// <summary>
        /// Returns the list of errors found while parsing the ini file.
        /// </summary>
        /// <remarks>
        /// If the configuration option ThrowExceptionOnError is false it
        /// can contain one element for each problem found while parsing;
        /// otherwise it will only contain the very same exception that was
        /// raised.
        /// </remarks>
        public ReadOnlyCollection<Exception> Errors
        {
            get { return _errorExceptions.AsReadOnly(); }
        }
        #endregion

        public void Reset()
        {

            _errorExceptions.Clear();
            _currentLineNumber = 0;
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
        public void Parse(TextReader textReader, IIniDataHandler iniData)
        {
            Reset();
            iniData.Clear();

            iniData.Start();

            string? currentLine;
            while ((currentLine = textReader.ReadLine()) != null)
            {
                _currentLineNumber++;

                try
                {
                    ProcessLine(currentLine, iniData);
                }
                catch (Exception ex)
                {
                    _errorExceptions.Add(ex);
                    if (Configuration.ThrowExceptionsOnError)
                    {
                        throw;
                    }
                }
            }

            try
            {
                iniData.End();
            }
            catch (Exception ex)
            {
                _errorExceptions.Add(ex);
                if (Configuration.ThrowExceptionsOnError)
                {
                    throw;
                }
            }
            

            if (HasError)
            {
                iniData.Clear();
            }
        }

        #region Template Method Design Pattern
        // All this methods controls the parsing behaviour, so it can be
        // modified in derived classes.
        // See http://www.dofactory.com/Patterns/PatternTemplate.aspx for an
        // explanation of this pattern.
        // Probably for the most common cases you can change the parsing
        // behavior using a custom configuration object rather than creating
        // derived classes.
        // See IniDotNetConfiguration interface, and IniDataParser constructor
        // to change the default configuration.

  

        /// <summary>
        ///     Processes one line and parses the data found in that line
        ///     (section or key/value pair who may or may not have comments)
        /// </summary>
        protected virtual void ProcessLine(string currentLine,
                                           IIniDataHandler iniData)
        {
            if (string.IsNullOrWhiteSpace(currentLine)) return;

            // TODO: change this to a global (IniData level) array of comments
            // Extract comments from current line and store them in a tmp list

            if (ProcessComment(currentLine, iniData)) 
                return;

            if (ProcessInlineComment(currentLine, iniData, out var line))
                currentLine = line;

            if (ProcessSection(currentLine, iniData)) 
                return;

            if (ProcessProperty(currentLine, iniData)) 
                return;

            if (ProcessMultilineProperty(currentLine, iniData)) 
                return;

            // the current line belongs to none of the 3 types


            if (Configuration.SkipInvalidLines) return;

            var errorFormat = "Couldn't parse text: '{0}'. Please see configuration option {1}.{2} to ignore this error.";
            var errorMsg = string.Format(errorFormat,
                                         currentLine,
                                         nameof(Configuration),
                                         nameof(Configuration.SkipInvalidLines));

            throw new ParsingException(errorMsg,
                                       _currentLineNumber,
                                       currentLine);
        }

        protected virtual bool ProcessInlineComment(in string currentLine, IIniDataHandler iniData, out string currentLineWithoutComment)
        {
            currentLineWithoutComment = "";
            if (!Configuration.AllowInlineComments)
                return false;

            var matchRes = Scheme.InlineCommentPattern.Match(currentLine);
            if (!matchRes.Success)
                return false;

            var comment = matchRes.Groups[2].Value;
            currentLineWithoutComment = currentLine.Substring(0, matchRes.Index); // does not trim at this time

            if (!Configuration.ParseComments)
                return true;

            if (Configuration.TrimComments)
                comment.Trim();

            iniData.HandleComment(comment, _currentLineNumber);

            return true;
        }

        protected virtual bool ProcessComment(in string currentLine, IIniDataHandler iniData)
        {
            // Line is  med when it came here, so we only need to check if
            // the first characters are those of the comments

            var matchRes = Scheme.CommentPattern.Match(currentLine);
            if (!matchRes.Success)
                return false;

            if (!Configuration.ParseComments)
                return true;

            var comment = matchRes.Groups[2].Value;
            if (Configuration.TrimComments)
                comment.Trim();

            iniData.HandleComment(comment, _currentLineNumber);

            return true;
        }

        /// <summary>
        ///     Proccess a string which contains an ini section.%
        /// </summary>
        /// <param name="currentLine">
        ///     The string to be processed
        /// </param>
        protected virtual bool ProcessSection(in string currentLine, IIniDataHandler iniData)
        {
            var matchRes = Scheme.SectionPattern.Match(currentLine);
            if (!matchRes.Success)
                return false;

            // removed error:
            // "No closing section value. Please see configuration option {0}.{1} to ignore this error."

            var sectionName = matchRes.Groups[1].Value;
            if (Configuration.TrimSections)
                sectionName.Trim();
            

            //Checks if the section already exists
            if (!Configuration.AllowDuplicateSections)
            {
                if (iniData.IsSectionEntered(sectionName, _currentLineNumber))
                {
                    if (Configuration.SkipInvalidLines) return false;

                    var errorFormat = "Duplicate section with name '{0}'. Please see configuration option {1}.{2} to ignore this error.";
                    var errorMsg = string.Format(errorFormat,
                                                 sectionName,
                                                 nameof(Configuration),
                                                 nameof(Configuration.SkipInvalidLines));

                    throw new ParsingException(errorMsg,
                                               _currentLineNumber,
                                               currentLine);
                }
            }

            // If the section does not exists, add it to the ini data
            iniData.EnterSection(sectionName, _currentLineNumber);

            // Save comments read until now and assign them to this section
            // This shall be a task of the handler

            return true;
        }

        protected virtual bool ProcessProperty(in string currentLine, IIniDataHandler iniData)
        {
            var matchRes = Scheme.KeyValueSeparatorPattern.Match(currentLine);
            if (!matchRes.Success)
                return false;


            var key = currentLine.Substring(0, matchRes.Groups[1].Index);
            var value = currentLine.Substring(matchRes.Groups[1].Index + matchRes.Groups[1].Length);

            // Multi-line Related
            var multiFlag = false;
            if (Configuration.AllowMultilineProperties)
            {
                var multiRes = Scheme.MultilineIndicatorPattern.Match(value);
                if (multiRes.Success)
                {
                    multiFlag = true;
                    value = value.Substring(0, multiRes.Groups[1].Index);
                }
            }


            if (Configuration.TrimProperties)
            {
                key.Trim();
                value.Trim();
            }

            if (Configuration.UseEscapeCharacters)
            {
                key = EscapeCharacterUtil.ParseValue(key);
                value = EscapeCharacterUtil.ParseValue(value);
            }

            if (string.IsNullOrEmpty(key))
            {
                if (Configuration.SkipInvalidLines) return false;

                var errorFormat = "Found property without key. Please see configuration option {0}.{1} to ignore this error";
                var errorMsg = string.Format(errorFormat,
                                             nameof(Configuration),
                                             nameof(Configuration.SkipInvalidLines));

                throw new ParsingException(errorMsg,
                                           _currentLineNumber,
                                           currentLine);
            }

            // Add property to data
            if (multiFlag)
                iniData.HandleMultilineProperty(key, value, _currentLineNumber);
            iniData.HandleProperty(key, value, _currentLineNumber);

            // Check if we haven't read any section yet
            // This should also be a task of the handler

            return true;
        }


        protected virtual bool ProcessMultilineProperty(in string currentLine, IIniDataHandler iniData)
        {
            if (!Configuration.AllowMultilineProperties)
                return false;

            var multiRes = Scheme.MultilineIndicatorPattern.Match(currentLine);
            if (!multiRes.Success)
                return false;

            var value = currentLine.Substring(0, multiRes.Groups[1].Index);

            if (Configuration.UseEscapeCharacters)
                value = EscapeCharacterUtil.ParseValue(value);

            iniData.HandleMultilineProperty(null, value, _currentLineNumber);

            return true;
        }

        #endregion

        #region Fields
        uint _currentLineNumber;

        // Holds a list of the exceptions catched while parsing
        readonly List<Exception> _errorExceptions;

        #endregion
    }
}
