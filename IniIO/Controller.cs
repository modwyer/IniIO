using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IniIO
{
    /// <summary>
    /// This is the main class that handles accessing all the other classes
    /// that hold the implementations of the various methods. 
    /// <br /><br />
    /// It handles dealing with custom types and objects which allows ConfigurationFileIO's
    /// methods to receive and return simple types.
    /// <br /><br />
    /// This class interacts mainly with the FileContents class.
    /// <br /><br />
    /// Class Hierarchy:
    /// <list type="table">
    ///     <listheader><term>Class</term><term>Description</term></listheader>
    ///     <item>
    ///         <description>ConfigurationFileIO</description>
    ///         <description>This is the main class the user interacts with.  This class has
    ///  a private member object of the Controller class.</description>
    ///     </item>
    ///     <item>
    ///         <description>Controller</description>
    ///         <description>This class has methods that ConfigurationFileIO calls.  It contains a
    ///     private member object of the FileContents class.  It also has a nested class
    ///     called MultiplePropertyController, which has methods that handle writing/removing
    ///     multiple entries at once.  The nested class and its methods are not accessable by 
    ///     default and an object of the nested class must be requested.</description>
    ///     </item>
    ///     <item>
    ///         <description>FileContents</description>
    ///         <description>This class handles the contents of the ini file.  It stores the contents in
    ///     memory and has methods for reading the contents in various ways.  It contains a private
    ///     member object of the FileModifier class.  This class reads in the contents of the ini
    ///     file and does not pass that IO job off to FileModifier.</description>
    ///     </item>
    ///     <item>
    ///         <description>FileModifier</description>
    ///         <description>Holds a FileSystemInfo object for the configuration file that is being 
    ///     manipulated and handles passing that onto FileIOManager.  Talks directly to the 
    ///     FileIOManager class to request the reading/writing of the ini file.</description>
    ///     </item>
    ///     <item>
    ///         <description>FileIOManager</description>
    ///         <description>A static class that is responsible for opening up an ini file and read/writing to it.  It 
    ///     tries to be thread safe and lock each ini file while manipulating it.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public sealed class Controller : IniIO.IController
    {
        private string FILE_NAME;
        private FileContents FILE_CONTENTS;
        
        /// <summary>
        /// Default class constructor.
        /// </summary>
        /// <param name="fileName">
        /// Path to the INI file.
        /// </param>
        /// <param name="fileFound">
        /// Boolean value:
        ///     True: INI file was found
        ///     False: INI file was not found
        /// </param>
        public Controller(string fileName, out bool fileFound)
        {
            FILE_NAME = fileName;
            // Create new FileContents.  Handles reading file and parsing its contents.
            FILE_CONTENTS = new FileContents(fileName);
            // Try and read in the contents of the file
            fileFound = FILE_CONTENTS.ReadFile();
            // If file is there, get all of its contents out.
            if (fileFound) { GetAllContent(); }
        }

        /// <summary>
        /// Create a new instance of the MultiplePropertyController.
        /// <br/><br/>
        /// The nested class implements the IMultiplePropertyIO interface which
        /// includes additional methods for writing entries to the ini file.  Instead
        /// of writing each entry separately you can store them up and "flush" them
        /// all at once.
        /// </summary>
        /// <returns>
        /// New instance of the MultiplePropertyController class.
        /// </returns>
        public MultiplePropertyController CreateMultiPropertyController()
        {
            return new MultiplePropertyController(this);
        }

        /// <summary>
        /// Get the contents, broken out by Section name with each Section
        /// containing all of its Properties (name/value pairs).
        /// <br/><br/>
        /// If there are any duplicate properties, the second one is ignored - assuming 
        /// that you have made a mistake by having two properties named the same thing.
        /// </summary>
        private void GetAllContent()
        {
            FILE_CONTENTS.GetAllContent();
        }


        #region IController Members
        
        /// <summary>
        /// Check to see if a property exists.
        /// </summary>
        /// <param name="property">Name of the Property to search for</param>
        /// <returns>
        /// An Entry object containing the info about the Property.
        /// </returns>
        public bool ContainsProperty(string property)
        {
            return FILE_CONTENTS.ContainsProperty(property);
        }

        /// <summary>
        /// Check to see if a property exists.
        /// </summary>
        /// <param name="property">Name of the Property to search for</param>
        /// <returns>
        /// An Entry object containing the info about the Property.
        /// </returns>
        public IEntry ContainsPropertyReturnInfo(string property)
        {
            return FILE_CONTENTS.ContainsPropertyReturnInfo(property); 
        }

        /// <summary>
        /// Try to parse the Value as a Boolean.
        /// </summary>
        /// <param name="e">        
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <param name="value">
        /// Out parameter that holds the returned Value as a Boolean.
        /// </param>
        /// <returns>
        /// True if the Value can be parsed as a Boolean,
        /// False if not.
        /// </returns>
        public bool GetValueAsBoolean(IEntry e, out bool value)
        {            
            e = FILE_CONTENTS.ReadEntry(e) as Entry;
            return e.AsBoolean(out value);
        }

        /// <summary>
        /// Try to parse the Value as an Integer.
        /// </summary>
        /// <param name="e">        
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <param name="value">
        /// Out parameter that holds the returned Value as an Integer.
        /// </param>
        /// <returns>
        /// True if the Value can be parsed as an Integer,
        /// False if not.
        /// </returns>
        public bool GetValueAsInteger(IEntry e, out int value)
        {
            e = FILE_CONTENTS.ReadEntry(e) as Entry;
            return e.AsInteger(out value);
        }

        /// <summary>
        /// Try to parse the Value as a long.
        /// </summary>
        /// <param name="e">        
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <param name="value">
        /// Out parameter that holds the returned Value as a Long.
        /// </param>
        /// <returns>
        /// True if the Value can be parsed as a long,
        /// False if not.
        /// </returns>
        public bool GetValueAsLong(IEntry e, out long value)
        {
            e = FILE_CONTENTS.ReadEntry(e) as Entry;
            return e.AsLong(out value);
        }

        /// <summary>
        /// Retrieve a Value for a Property.
        /// </summary>
        /// <param name="e">        
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <returns>Value as a string</returns>
        public string ReadPropertyValue(IEntry e)
        {
            return FILE_CONTENTS.ReadEntryValue(e);
        }

        /// <summary>
        /// Write a Name/Value to a section.
        /// </summary>
        /// <param name="e">        
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <returns>False if the write was unsucessful</returns>
        public bool WriteProperty(IEntry e)
        {
            return FILE_CONTENTS.WriteEntry(e);            
        }

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
        public bool RemoveProperty(IEntry e)
        {
            return FILE_CONTENTS.RemoveEntry(e);
        }

        /// <summary>
        /// Get the collection of Section names pulled from ini file.
        /// </summary>
        /// <returns>
        /// IEnumerable collection of strings, that are the names of the Sections.
        /// </returns>
        public IEnumerable<string> GetSectionNames() 
        {
            return FILE_CONTENTS.GetSectionNames();
        }

        /// <summary>
        /// Return the contents of the file as a string.
        /// </summary>
        /// <returns>
        /// The contents of the INI file as a string.
        /// </returns>
        public string toString() { return FILE_CONTENTS.ToString(); }

        /// <summary>
        /// Return the contents of the ini file as a Dictionary.        
        /// </summary>
        /// <returns>
        /// IDictionary:<br/>
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
        public Dictionary<string, Dictionary<string, string>> GetContentsAsDictionary()
        {
            return FILE_CONTENTS.Contents;
        }

        #endregion IController Members

        /// <summary>
        /// Class used to handle the writing of multiple entries to an ini file at once.
        /// <br/><br/>
        /// Not all implementations are going to need this capability for simple one-off 
        /// writes to the ini file.
        /// <br/><br/>
        /// This is implemented in the IniCreator class, used to create new ini files.
        /// </summary>
        public class MultiplePropertyController : IMultiplePropertyIO
        {
            private IniIO.Controller controller;

            /// <summary>
            /// Default class constructor.
            /// <br/><br/>
            /// Accepts the Controller object from the parent class.
            /// </summary>
            /// <param name="c">
            /// Controller object that is part of the parent class.
            /// </param>
            public MultiplePropertyController(IniIO.Controller c) 
            {
                controller = c;
            }

            /// <summary>
            /// Convert a Dictionary to a list of Entry objects.
            /// </summary>
            /// <param name="entries"></param>
            /// <returns>
            /// IList of Entry objects
            /// </returns>
            private IList<IEntry> toList(IDictionary<string, IDictionary<string, string>> entries)
            {
                IList<IEntry> elist = new List<IEntry>();
                IEntry entry;

                foreach (KeyValuePair<string, IDictionary<string, string>> kvp in entries)
                {
                    foreach (KeyValuePair<string, string> kvp2 in kvp.Value)
                    {
                        entry = new Entry(kvp.Key, kvp2.Key, kvp2.Value);
                        elist.Add(entry);
                    }
                }
                return elist;
            }


            #region IMultiplePropertyIO Members

            /// <summary>
            /// Store an entry to be written at once with other entries.
            /// </summary>
            /// <param name="sectionName">
            /// Name of the Section to save the Property in.
            /// </param>
            /// <param name="property">
            /// Name of the Property to store for write.
            /// </param>
            /// <returns>
            /// True if the entry was stored successfully,
            /// False if not.
            /// </returns>
            public bool SaveEntry(string sectionName, string property)
            {
                return controller.FILE_CONTENTS.FILE_MODIFIER.StoreEntryForWrite(new Entry(sectionName, property));
            }
            /// <summary>
            /// Store an entry to be written at once with other entries.
            /// Accepts a Value for the Property.
            /// </summary>
            /// <param name="sectionName">
            /// Name of the Section to save the Property in.
            /// </param>
            /// <param name="property">
            /// Name of the Property to store for write.
            /// </param>
            /// <param name="value">
            /// Value of the Property to be stored for write.
            /// </param>
            /// <returns>
            /// True if the entry was stored successfully,
            /// False if not.
            /// </returns>
            public bool SaveEntry(string sectionName, string property, string value)
            {
                return controller.FILE_CONTENTS.FILE_MODIFIER.StoreEntryForWrite(new Entry(sectionName, property, value));
            }
            /// <summary>
            /// Store multiple entries at once.
            /// </summary>
            /// <param name="entries">
            /// IDictionary:<br/>
            /// <list type="bullet">
            ///     <item>KEY: string - Section name</item>
            ///     <item>VALUE: Dictionary
            ///         <list type="bullet">
            ///             <item>KEY: string - Property name</item>
            ///             <item>VALUE: string - Value of Property</item>
            ///         </list>
            ///     </item> 
            /// </list>
            /// </param>
            /// <returns>
            /// True is the entries were stored successfully, False if not.
            /// </returns>
            public bool SaveEntries(IDictionary<string, IDictionary<string, string>> entries)
            {
                return controller.FILE_CONTENTS.FILE_MODIFIER.StoreEntriesForWrite(toList(entries));
            }
            /// <summary>
            /// Remove a stored entry from the list of entries waiting to be written.
            /// </summary>
            /// <param name="sectionName">
            /// Name of the Section to remove the Property from.
            /// </param>
            /// <param name="property">
            /// Name of the Property to remove.
            /// </param>
            /// <returns>
            /// True if the entry was stored successfully,
            /// False if not.
            /// </returns>
            public bool RemoveSavedEntry(string sectionName, string property)
            {
                return controller.FILE_CONTENTS.FILE_MODIFIER.RemoveStoredEntry(new Entry(sectionName, property));
            }
            /// <summary>
            /// Remove a stored entry from the list of entries waiting to be written.
            /// <br/><br/>
            /// Accepts a Value for the Property.
            /// </summary>
            /// <param name="sectionName">
            /// Name of the Section to remove the Property from.
            /// </param>
            /// <param name="property">
            /// Name of the Property to remove.
            /// </param>
            /// <param name="value">
            /// Value of the Property to be removed from list of entries to write.
            /// </param>
            /// <returns>
            /// True if the entry was stored successfully,
            /// False if not.
            /// </returns>
            public bool RemoveSavedEntry(string sectionName, string property, string value)
            {
                return controller.FILE_CONTENTS.FILE_MODIFIER.RemoveStoredEntry(new Entry(sectionName, property, value));
            }
            /// <summary>
            /// Removed multiple stored entries from the list of entries waiting to be
            /// written.
            /// </summary>
            /// <param name="entries">
            /// IDictionary:<br/>
            /// <list type="bullet">
            ///     <item>KEY: string - Section name</item>
            ///     <item>VALUE: Dictionary
            ///         <list type="bullet">
            ///             <item>KEY: string - Property name</item>
            ///             <item>VALUE: string - Value of Property</item>
            ///         </list>
            ///     </item> 
            /// </list>
            /// </param>
            /// <returns>
            /// True if the entry was stored successfully,
            /// False if not.
            /// </returns>
            public bool RemoveSavedEntries(IDictionary<string, IDictionary<string, string>> entries)
            {
                return controller.FILE_CONTENTS.FILE_MODIFIER.RemoveStoredEntries(toList(entries));
            }
            /// <summary>
            /// Write all of the saved entries to the ini file.
            /// </summary>
            /// <returns>
            /// True if the writes were successfull, False if not.</returns>
            public bool WriteSavedEntries()
            {
                return controller.FILE_CONTENTS.FILE_MODIFIER.WriteStoredEntries();
            }

            #endregion
        }
    }
}
