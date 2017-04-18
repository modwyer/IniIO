using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IniIO
{
    /// <summary>
    /// Interface for the main class that users of the dll will interact with.
    /// </summary>
    public interface IConfigurationFileIO
    {
        /// <summary>
        /// Establish a connection to file on disk and initialize object.
        /// </summary>
        /// <param name="configurationFile">Path to the ini file on disc</param>
        /// <param name="fileFound">False if the file can't be read</param>
        void Initialize(string configurationFile, out bool fileFound);
        /// <summary>
        /// Return all of the Section names.
        /// </summary>
        /// <returns>All of the Section names as a collection.</returns>
        IEnumerable<string> GetSectionNames();
        /// <summary>
        /// Check if the configuration file contains a Section name.
        /// </summary>
        /// <param name="sectionName">Name of the Section to search for.</param>
        /// <returns>
        /// True if the Section exists
        /// False if the Section does not exist
        /// </returns>
        bool ContainsSection(string sectionName);
        /// <summary>
        /// Check if a Property exists.
        /// </summary>
        /// <param name="name">
        /// Name of the Property to check for.
        /// </param>
        /// <returns>
        /// True if the file contains the Property
        /// False if the file does not contain the Property
        /// </returns>
        bool ContainsProperty(string name);
        /// <summary>
        /// Check to see if a Property exists.
        /// <br/><br/>
        /// Output parameters for the name of the Section the Property is found in and
        /// the Value of the Property.
        /// </summary>
        /// <param name="property">Name of the Property to search for.</param>
        /// <param name="sectionName">Name of the Section the Property was found in.</param>
        /// <param name="value">Value of the Property (if available).</param>
        /// <returns>
        /// True if the file contains the Property
        /// False the file does not contain the Property
        /// </returns>
        bool ContainsProperty(string property, out string sectionName, out string value);
        /// <summary>
        /// Retrieve a Value for a Property.
        /// </summary>
        /// <param name="sectionName">Name of the Section to look in.</param>
        /// <param name="property">Name of the Property to read value for.</param>
        /// <returns>The Value of the Property</returns>
        string ReadValue(string sectionName, string property);
        /// <summary>
        /// Retrieve the Value of a Property as a Boolean.
        /// </summary>
        /// <param name="sectionName">Name of the Section to look in.</param>
        /// <param name="property">Name of the Property to read value for.</param>
        /// <param name="value">
        /// The Value of the Property taken from the ini file.
        /// It will be true or false.
        /// </param>
        /// <returns>
        /// True if the Value from the ini file can be parsed as a boolean.
        /// </returns>
        bool ReadValueAsBoolean(string sectionName, string property, out bool value);
        /// <summary>
        /// Retrieve the Value of a Property as an Integer.
        /// </summary>
        /// <param name="sectionName">Name of the Section to look in.</param>
        /// <param name="property">Name of the Property to read value for.</param>
        /// <param name="value">
        /// The Value of the Property taken from the ini file.
        /// It will be zero if the parse fails and the integer value if successful.
        /// </param>
        /// <returns>
        /// True if the Value from the ini file can be parsed as an Integer.
        /// </returns>
        bool ReadValueAsInteger(string sectionName, string property, out int value);
        /// <summary>
        /// Retrieve the Value of a Property as a long.
        /// </summary>
        /// <param name="sectionName">Name of the Section to look in.</param>
        /// <param name="property">Name of the Property to read the value for.</param>
        /// <param name="value">
        /// The Value of the Property taken from the ini file.
        /// </param>
        /// <returns>
        /// True if the Value from the ini file can parsed as a long.
        /// </returns>
        bool ReadValueAsLong(string sectionName, string property, out long value);
        /// <summary>
        /// Add a Value to a Property, if the Property exists, or add a 
        /// Property/Value pair.
        /// </summary>
        /// <param name="sectionName">Name of the Section to look in.</param>
        /// <param name="property">Name of the Property to write.</param>
        /// <param name="value">Value to write for the Property.</param>
        /// <returns>
        /// True: write successful
        /// False: write unsuccessful
        /// </returns>
        bool WriteEntry(string sectionName, string property, string value);       
        /// <summary>
        /// Remove a Property and its Value from a Section.
        /// </summary>
        /// <param name="sectionName">Name of the Section to look in.</param>
        /// <param name="property">Name of the Property to write</param>
        /// <returns>
        /// True if the Property and Value were successfully removed,
        /// False if not.
        /// </returns>
        bool RemoveEntry(string sectionName, string property);        
        /// <summary>
        /// Return the contents of the ini file as a string.
        /// </summary>
        string toString();
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
        Dictionary<string, Dictionary<string, string>> toDictionary();
    }
}
