using IniDotNet.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Linq;
public class IniObjectHandler : IIniHandler<IniObject>
{
    #region Fields & Properties

    // Temp list of comments
    public List<string> CurrentCommentListTemp
    {
        get
        {
            if (_currentCommentListTemp == null)
            {
                _currentCommentListTemp = new List<string>();
            }

            return _currentCommentListTemp;
        }

        internal set
        {
            _currentCommentListTemp = value;
        }
    }


    List<string>? _currentCommentListTemp;

    // Temp var with the name of the seccion which is being process
    string? _currentSectionNameTemp;

    // Temp var of the value being parsed
    IniObject? _value;
    IniObject iniData => _value ?? (_value = new());

    // configurations
    public IniParserConfig Configuration { get; set; }

    #endregion

    public IniObjectHandler()
    {
        Configuration = new IniParserConfig();
    }

    public IniObject Get()
    {
        return iniData;
    }


    public void Clear()
    {
        _value = null;
        _currentCommentListTemp = null;
        _currentSectionNameTemp = null;
    }


    public void Start()
    {
        _value = Configuration.CaseInsensitive ?
            new IniObjectCaseInsensitive() :
            new IniObject();
    }

    public void End()
    {
        // Orphan comments, assing to last section/key value
        if (Configuration.ParseComments && CurrentCommentListTemp.Count > 0)
        {
            if (iniData.Sections.Count > 0)
            {
                // Check if there are actually sections in the file
                var sections = iniData.Sections;
                var section = sections.FindByName(_currentSectionNameTemp!);
                section!.Comments.AddRange(CurrentCommentListTemp);
            }
            else if (iniData.Global.Count > 0)
            {
                // No sections, put the comment in the last key value pair
                // but only if the ini file contains at least one key-value pair
                iniData.Global.GetLast()!.Comments.AddRange(CurrentCommentListTemp);
            }

            CurrentCommentListTemp.Clear();
        }
    }

    public void EnterSection(string sectionName, uint line)
    {
        _currentSectionNameTemp = sectionName;

        // If the section does not exists, add it to the ini data
        iniData.Sections.Add(sectionName);

        // Save comments read until now and assign them to this section
        if (Configuration.ParseComments)
        {
            var sections = iniData.Sections;
            var sectionData = sections.FindByName(sectionName);
            sectionData!.Comments.AddRange(CurrentCommentListTemp);
            CurrentCommentListTemp.Clear();
        }
    }


    public void LeaveSection(uint line)
    {
        // do nothing
    }

    public void EnterSubsection(string section, uint line)
    {
        throw new InvalidOperationException();
    }

    public void LeaveSubsection(uint line)
    {
        throw new InvalidOperationException();
    }


    public void HandleComment(string comment, uint line)
    {
        CurrentCommentListTemp.Add(comment.ToString());
    }


    public void HandleProperty(string key, string value, uint line)
    {
        if (string.IsNullOrEmpty(_currentSectionNameTemp))
        {
            if (!Configuration.AllowKeysWithoutSection)
            {
                var errorFormat = "Properties must be contained inside a section. Please see configuration option {0}.{1} to ignore this error.";
                var errorMsg = string.Format(errorFormat,
                                            nameof(Configuration),
                                            nameof(Configuration.AllowKeysWithoutSection));

                throw new ParsingException(errorMsg,
                                           line,
                                           $"{key} = {value}");
            }
        }

        var props = string.IsNullOrEmpty(_currentSectionNameTemp) ?
            iniData.Global :
            iniData.Sections.FindByName(_currentSectionNameTemp)!.Properties;

        AddKeyToKeyValueCollection(
            key,
            value,
            props,
            _currentSectionNameTemp ?? "global",
            line);

    }



    public bool IsSectionEntered(string section, uint line)
    {
        return iniData.Sections.Contains(section);
    }


    #region Helpers

    /// <summary>
    ///     Adds a key to a concrete <see cref="IniPropertyCollection"/> instance, checking
    ///     if duplicate keys are allowed in the configuration
    /// </summary>
    /// <param name="key">
    ///     Key name
    /// </param>
    /// <param name="value">
    ///     Key's value
    /// </param>
    /// <param name="keyDataCollection">
    ///     <see cref="IniProperty"/> collection where the key should be inserted
    /// </param>
    /// <param name="sectionName">
    ///     Name of the section where the <see cref="IniPropertyCollection"/> is contained.
    ///     Used only for logging purposes.
    /// </param>
    private void AddKeyToKeyValueCollection(string key, string value, IniPropertyCollection keyDataCollection, string sectionName, uint line)
    {
        // Check for duplicated keys
        if (keyDataCollection.Contains(key))
        {
            // We already have a key with the same name defined in the current section
            HandleDuplicatedKeyInCollection(key, value, keyDataCollection, sectionName, line);
        }
        else
        {
            // Save the keys
            keyDataCollection.Add(key, value);
        }

        if (Configuration.ParseComments)
        {
            keyDataCollection.FindByKey(key)!.Comments = CurrentCommentListTemp;
            CurrentCommentListTemp.Clear();
        }

    }



    /// <summary>
    ///     Abstract Method that decides what to do in case we are trying 
    ///     to add a duplicated key to a section
    /// </summary>
    void HandleDuplicatedKeyInCollection(string key,
                                         string value,
                                         IniPropertyCollection keyDataCollection,
                                         string sectionName,
                                         uint line)
    {
        switch (Configuration.DuplicatePropertiesBehaviour)
        {
            case IniParserConfig.EDuplicatePropertiesBehaviour.DisallowAndStopWithError:
                var errorMsg = string.Format("Duplicated key '{0}' found in section '{1}", key, sectionName);
                throw new ParsingException(errorMsg, line);
            case IniParserConfig.EDuplicatePropertiesBehaviour.AllowAndKeepFirstValue:
                // Nothing to do here: we already have the first value assigned
                break;
            case IniParserConfig.EDuplicatePropertiesBehaviour.AllowAndKeepLastValue:
                // Override the current value when the parsing is finished we will end up
                // with the last value.
                keyDataCollection[key] = value;
                break;
            case IniParserConfig.EDuplicatePropertiesBehaviour.AllowAndConcatenateValues:
                keyDataCollection[key] += Configuration.ConcatenateDuplicatePropertiesString + value;
                break;
        }
    }

    #endregion
}
