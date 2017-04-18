using System;
using System.Collections.Generic;
namespace IniIO
{
    /// <summary>
    /// Interface for a Controller to the ConfigurationFileIO class.
    /// <br/><br/>
    /// The Controller class acts as an intermediary between the public
    /// ConfigurationFileIO class and all of the classes that do the work
    /// of reading and writing to the INI file.
    /// <br/><br/>
    /// A Controller class handles conversion of simple type parameters to
    /// any custom types or class objects.
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// Check to see if a property exists.
        /// </summary>
        /// <param name="property">Name of the Property to search for</param>
        /// <returns>
        /// An Entry object containing the info about the Property.
        /// </returns>
        bool ContainsProperty(string property);
        /// <summary>
        /// Check to see if a property exists.
        /// </summary>
        /// <param name="property">Name of the Property to search for</param>
        /// <returns>
        /// An Entry object containing the info about the Property.
        /// </returns>
        IEntry ContainsPropertyReturnInfo(string property);
        /// <summary>
        /// Get the collection of Section names pulled from ini file.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetSectionNames();
        /// <summary>
        /// Try to parse the Value as a Boolean.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <param name="value"></param>
        /// <returns>
        /// True if the Value can be parsed as a Boolean,
        /// False if not.
        /// </returns>
        bool GetValueAsBoolean(IEntry e, out bool value);
        /// <summary>
        /// Try to parse the Value as an Integer.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <param name="value"></param>
        /// <returns>
        /// True if the Value can be parsed as an Integer,
        /// False if not.
        /// </returns>
        bool GetValueAsInteger(IEntry e, out int value);
        /// <summary>
        /// Try to parse the Value as a long.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <param name="value"></param>
        /// <returns>
        /// True if the Value can be parsed as a long,
        /// False if not.
        /// </returns>
        bool GetValueAsLong(IEntry e, out long value);
        /// <summary>
        /// Retrieve a Value for a Property.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <returns>Value as a string</returns>
        string ReadPropertyValue(IEntry e);
        /// <summary>
        /// Remove a Property/Value pair from a Section.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <returns>
        /// True if the Property/Value was removed successfully,
        /// False if not.
        /// </returns>
        bool RemoveProperty(IEntry e);
        /// <summary>
        /// Return the contents of the file as a string.
        /// </summary>
        string toString();
        /// <summary>
        /// Write a Name/Value to a section.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <returns>False if the write was unsucessful</returns>
        bool WriteProperty(IEntry e);
        /// <summary>
        /// Return the contents of the ini file as a Dictionary.        
        /// </summary>
        /// <returns>
        /// Dictionary:<br/>
        /// <list type="bullet">
        ///     <item>KEY: string - Section name</item>
        ///     <item>VALUE: Dictionary
        ///         <list type="bullet">
        ///             <item>KEY: string - Property name</item>
        ///             <item>VALUE: string - Value of Property</item>
        ///         </list>
        ///     </item> 
        /// </list>
        /// </returns>
        Dictionary<string, Dictionary<string, string>> GetContentsAsDictionary();
    }
}
