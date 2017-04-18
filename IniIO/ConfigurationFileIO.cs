using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace IniIO
{
    /// <summary>
    /// The main class that a user of this dll will interact with.
    /// <br/><br/>
    /// It contains methods which allow easy accessability to an ini file
    /// both for reading and writing entries.
    /// </summary>
    public class ConfigurationFileIO : IConfigurationFileIO
    {
        private string FILE_NAME;
        /// <summary>
        /// Instance of the Controller class.<br/>
        /// The access is set to Protected to allow the IniCreator
        /// class access to the inner class, MultiplePropertyController.
        /// </summary>
        protected Controller Helper;

        /// <summary>
        /// Default class constructor accepts the path to the ini
        /// file as the only parameter.
        /// </summary>
        /// <param name="configurationFilePath">
        /// Path to the ini file.
        /// </param>
        public ConfigurationFileIO(string configurationFilePath)
        {
            bool fileFound = false;
            Initialize(configurationFilePath, out fileFound);

            if (!fileFound)
            {
                throw new ArgumentException("Bad file or path.");
            }
            
            FILE_NAME = configurationFilePath;
        }
        

        #region IConfigurationFileIO Members

        /// <summary>
        /// Establish a connection to file on disk and initialize object.
        /// </summary>
        /// <param name="configurationFile">Path to the ini file on disc</param>
        /// <param name="fileFound">False if the file can't be read</param>
        public void Initialize(string configurationFile, out bool fileFound)
        {
            Helper = new Controller(configurationFile, out fileFound);
        }

        /// <summary>
        /// Return all of the Section names.
        /// </summary>
        /// <returns>All of the Section names as a collection.</returns>
        public IEnumerable<string> GetSectionNames()
        {           
            return Helper.GetSectionNames();
        }

        /// <summary>
        /// Check if the configuration file contains a Section name.
        /// </summary>
        /// <param name="sectionName">Name of the Section to search for</param>
        /// <returns>
        /// True if the Section exists
        /// False if the Section does not exist
        /// </returns>
        public bool ContainsSection(string sectionName)
        {
            return (GetSectionNames().ToList().Contains(sectionName)) 
                ? true : false;
        }

        /// <summary>
        /// Check if a Property exists.
        /// </summary>
        /// <param name="property">
        /// String value of the Property.
        /// </param>
        /// <returns>
        /// True if the file contains the Property<br/>
        /// False if the file does not contain the Property
        /// </returns>
        public bool ContainsProperty(string property)
        {
            return Helper.ContainsProperty(property);
        }

        /// <summary>
        /// Check to see if a Property exists.
        /// Output parameters for the name of the Section the Property is found in and
        /// the Value of the Property.
        /// </summary>
        /// <param name="property">Name of the Property to search for</param>
        /// <param name="sectionName">Name of the Section the Property was fourd in</param>
        /// <param name="value">Value of the Property (if available)</param>
        /// <returns>
        /// True if the file contains the Property
        /// False the file does not contain the Property
        /// </returns>
        public bool ContainsProperty(string property, out string sectionName, out string value)
        {
            bool retval = true;
            sectionName = "";
            value = "";

            var e = Helper.ContainsPropertyReturnInfo(property) as Entry;
            if (null == e) { retval = false; }
            else
            {
                sectionName = e.Section;
                value = e.Value;
            }
            return retval;
        }

        /// <summary>
        /// Retrieve a Value for a Property.
        /// </summary>
        /// <param name="sectionName">Name of the Section to look in.</param>
        /// <param name="property">Name of the Property to get the value for.</param>
        /// <returns>The Value of the Property</returns>
        public string ReadValue(string sectionName, string property)
        {
            return Helper.ReadPropertyValue(new Entry(sectionName, property));
        }

        /// <summary>
        /// Retrieve the Value of a Property as a Boolean.
        /// </summary>
        /// <param name="sectionName">Name of the Section to look in.</param>
        /// <param name="property">Name of the Property to get the Value for.</param>
        /// <param name="value">
        /// The Value of the Property taken from the ini file.<br/>
        /// It will be true or false.
        /// </param>
        /// <returns>
        /// True if the Value from the ini file can be parsed as a boolean.
        /// </returns>
        public bool ReadValueAsBoolean(string sectionName, string property, out bool value)
        {
            return Helper.GetValueAsBoolean(new Entry(sectionName, property), out value);
        }

        /// <summary>
        /// Retrieve the Value of a Property as an Integer.
        /// <br/><br/>
        /// If the Value is too small or large to fit into an Int32
        /// then this method will return false.
        /// </summary>
        /// <param name="sectionName">Name of the Section to look in.</param>
        /// <param name="property">Name of the Property to get the Value for.</param>
        /// <param name="value">
        /// The Value of the Property taken from the ini file.<br/>
        /// It will be zero if the parse fails and the integer value if successful.
        /// </param>
        /// <returns>
        /// True if the Value from the ini file can be parsed as an Integer.
        /// </returns>
        public bool ReadValueAsInteger(string sectionName, string property, out int value)
        {
            return Helper.GetValueAsInteger(new Entry(sectionName, property), out value);
        }

        /// <summary>
        /// Retrieve the Value of a Property as a long.
        /// <br/><br/>
        /// Use this method if you are unsure of whether you number is too
        /// large or not to fit in an Int32.
        /// </summary>
        /// <param name="sectionName">Name of the Section to look in.</param>
        /// <param name="property">Name of the Property holding the value.</param>
        /// <param name="value">
        /// The Value of the Property taken from the ini file.
        /// </param>
        /// <returns>
        /// True if the Value from the ini file can parsed as a long.
        /// </returns>
        public bool ReadValueAsLong(string sectionName, string property, out long value)
        {
            return Helper.GetValueAsLong(new Entry(sectionName, property), out value);
        }

        /// <summary>
        /// Add a Value to a Property, if the Property exists, or add a 
        /// Property/Value pair.
        /// </summary>
        /// <param name="sectionName">Name of the Section to look in.</param>
        /// <param name="property">Name of the Property to write to.</param>
        /// <param name="value">Value of the Property to write.</param>
        /// <returns>
        /// True: write successful
        /// False: write unsuccessful
        /// </returns>
        public bool WriteEntry(string sectionName, string property, string value)
        {
            return Helper.WriteProperty(new Entry(sectionName, property, value));
        }

        /// <summary>
        /// Remove a Property and its Value from a Section.
        /// </summary>
        /// <param name="sectionName">Name of the Section the Property is found in.</param>
        /// <param name="property">Name of the Property to remove.</param>
        /// <returns>
        /// True if the Property and Value were successfully removed,
        /// False if not.
        /// </returns>
        public bool RemoveEntry(string sectionName, string property)
        {
            return Helper.RemoveProperty(new Entry(sectionName, property));
        }

        /// <summary>
        /// Return the contents of the ini file as a string.
        /// </summary>
        /// <returns>
        /// The contents of the INI file as a string.
        /// </returns>
        public string toString() { return Helper.toString(); }

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
        public Dictionary<string, Dictionary<string, string>> toDictionary()
        {
            return Helper.GetContentsAsDictionary();
        }

        #endregion IConfigurationFileIO Members
    }
}
