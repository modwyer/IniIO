using System;
namespace IniIO
{
    /// <summary>
    /// Interface for an entry in the INI file.
    /// <br/><br/>
    /// An Entry consists of the following items:
    /// <list type="table">
    /// <item>
    ///     <description>Section</description>
    ///     <description>The bracketed name that holds Property/Value pairs</description>
    /// </item>
    /// <item>
    ///     <description>Property</description>
    ///     <description>The name of variable that has a Value (aka Field, Key)</description>
    /// </item>
    /// <item>
    ///     <description>Value</description>
    ///     <description>The content of the Property</description>
    /// </item>
    /// </list>
    /// <br/><br/>
    /// <remarks>
    /// This interface exposes three methods which act as alternate means of
    /// extracting the Value of a Property.  
    /// <list type="bullet">
    ///     <item>bool AsBoolean(out bool value)</item>
    ///     <item>bool AsInteger(out int value)</item>
    ///     <item>bool AsLong(out long value)</item>
    /// </list>
    /// All Properties are stored as String values in the file, but may actually
    /// represent a Boolean, Integer, or Long value. The three methods 
    /// allow you to attempt to retrieve the Value as one of those types.
    /// </remarks>
    /// </summary>
    public interface IEntry
    {
        /// <summary>
        /// The bracketed name of the group that the Property belongs to.<br/>
        /// <list type="bullet">
        ///     <item>ie: [debug]</item>
        /// </list>
        /// </summary>
        string Section { get; set; }
        /// <summary>
        /// The name of an item within a Section. (aka Field, Key)
        /// </summary>
        string Property { get; set; }
        /// <summary>
        /// The content stored for a specific Property.
        /// </summary>
        string Value { get; set; }
        /// <summary>
        /// Tries to return the Value of the Property as a Boolean.
        /// </summary>
        /// <returns>
        /// True is the Value can be parsed as Boolean=True.<br/>
        /// False if the Value can not be parsed as a Boolean.
        /// </returns>
        bool AsBoolean(out bool value);
        /// <summary>
        /// Tries to return the Value of the Property as an Integer.<br/>
        /// The Value as an Integer is returned as an out parameter. 
        /// </summary>
        /// <returns>
        /// True if the Value can be converted to an Integer.<br/>
        /// False if it can not be converted to an Integer.
        /// </returns>
        bool AsInteger(out int value);
        /// <summary>
        /// Tries to return the Value of the Property as a long.<br/>
        /// The Value as a long is returned as an out parameter.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>
        /// True if the Value can be converted to a Long,
        /// False if not.
        /// </returns>
        bool AsLong(out long value);
    }
}
