using System;
using System.Collections.Generic;
using IniDotNet.Base;

namespace IniDotNet.Linq;


/// <summary>
///     Information associated to a section in a INI File
///     Includes both the properties and the comments associated to the section.
/// </summary>
public class IniSection : IDeepCloneable<IniSection>
{
    #region Initialization

    public IniSection(string sectionName)
        : this(sectionName, EqualityComparer<string>.Default)
    {

    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IniSection"/> class.
    /// </summary>
    public IniSection(string sectionName, IEqualityComparer<string> searchComparer)
    {
        _searchComparer = searchComparer;

        if (string.IsNullOrEmpty(sectionName))
            throw new ArgumentException("section name can not be empty", nameof(sectionName));

        Properties = new IniPropertyCollection(_searchComparer);
        Name = sectionName;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IniSection"/> class
    ///     from a previous instance of <see cref="IniSection"/>.
    /// </summary>
    /// <remarks>
    ///     Data is deeply copied
    /// </remarks>
    /// <param name="ori">
    ///     The instance of the <see cref="IniSection"/> class 
    ///     used to create the new instance.
    /// </param>
    /// <param name="searchComparer">
    ///     Search comparer.
    /// </param>
    public IniSection(IniSection ori, IEqualityComparer<string>? searchComparer = null)
    {
        Name = ori.Name;

        _searchComparer = searchComparer ?? EqualityComparer<string>.Default;
        Comments = ori.Comments;
        Properties = new IniPropertyCollection(ori.Properties, searchComparer ?? ori._searchComparer);
    }

    #endregion


    /// <summary>
    ///     Gets or sets the name of the section.
    /// </summary>
    /// <value>
    ///     The name of the section
    /// </value>
    public string Name
    {
        get
        {
            if (_name == null)
                return "";
            return _name;
        }

        set
        {
            if (!string.IsNullOrEmpty(value))
                _name = value;
        }
    }

    /// <summary>
    ///     Gets or sets the comment list associated to this section.
    /// </summary>
    /// <value>
    ///     A list of strings.
    /// </value>
    public List<string> Comments
    {
        get
        {
            if (_comments == null)
            {
                _comments = new List<string>();
            }

            return _comments;
        }

        set
        {
            if (_comments == null)
            {
                _comments = new List<string>();
            }
            _comments.Clear();
            _comments.AddRange(value);
        }
    }

    /// <summary>
    ///     Gets or sets the properties associated to this section.
    /// </summary>
    /// <value>
    ///     A collection of IniProperty objects.
    /// </value>
    public IniPropertyCollection Properties { get; set; }

    /// <summary>
    ///     Deletes all comments and properties from this IniSection
    /// </summary>
    public void Clear()
    {
        ClearProperties();
        ClearComments();
    }

    /// <summary>
    ///     Deletes all comments in this section and in all the properties pairs it contains
    /// </summary>
    public void ClearComments()
    {
        Comments.Clear();
        Properties.ClearComments();
    }

    /// <summary>
    /// Deletes all the properties pairs in this section.
    /// </summary>
    public void ClearProperties()
    {
        Properties.Clear();
    }

    /// <summary>
    ///     Merges otherSection into this, adding new properties if they 
    ///     did not existed or overwriting values if the properties already 
    ///     existed.
    /// </summary>
    /// <remarks>
    ///     Comments are also merged but they are always added, not overwritten.
    /// </remarks>
    /// <param name="toMergeSection"></param>
    public void Merge(IniSection toMergeSection)
    {
        Properties.Merge(toMergeSection.Properties);

        foreach (var comment in toMergeSection.Comments)
            Comments.Add(comment);
    }

    #region IDeepCloneable<T> Members
    public IniSection DeepClone()
    {
        return new IniSection(this);
    }
    #endregion

    #region Fields
    private List<string>? _comments;
    private string? _name;
    private readonly IEqualityComparer<string> _searchComparer;
    #endregion
}