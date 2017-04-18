using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IniIO
{
    /// <summary>
    /// Extended class to the ConfigurationFileIO class.  This class
    /// allows you to queue up changes to the INI file to write multple 
    /// properties all at once.
    /// </summary>
    public class MultiplePropertyIO : ConfigurationFileIO, IMultiplePropertyIO
    {
        public MultiplePropertyIO(string iniFilePath)
            : base(iniFilePath)
        {
            bool fileFound = false;
            this.controller = new Controller(iniFilePath, out fileFound);

            if (!fileFound)
            {
                throw new ArgumentException("Bad file or path.");
            }

            this.mpc = base.Helper.CreateMultiPropertyController();
            this.FILE_PATH = iniFilePath;
        }

        #region IMultiplePropertyIO Members

        /// <summary>
        /// Store an entry to be written at once with other entries.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="property"></param>
        /// <returns>
        /// True if the entry was stored successfully,
        /// False if not.
        /// </returns>
        public bool SaveEntry(string sectionName, string property)
        {
            return mpc.SaveEntry(sectionName, property);
        }
        /// <summary>
        /// Store an entry to be written at once with other entries.
        /// Accepts a Value for the Property.
        /// </summary>
        /// <param name="sectionName">Name of the Section to save entry in.</param>
        /// <param name="property">Name of the Property to save.</param>
        /// <param name="value">Value of the Property to save.</param>
        /// <returns>
        /// True if the entry was stored successfully,
        /// False if not.
        /// </returns>
        public bool SaveEntry(string sectionName, string property, string value)
        {
            return mpc.SaveEntry(sectionName, property, value);
        }
        /// <summary>
        /// Store multiple entries at once.
        /// </summary>
        /// <param name="contents">
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
        /// <returns></returns>
        public bool SaveEntries(IDictionary<string, IDictionary<string, string>> contents)
        {
            return mpc.SaveEntries(contents);
        }
        /// <summary>
        /// Remove a stored entry from the list of entries waiting to be written.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="property"></param>
        /// <returns>
        /// True if the entry was stored successfully,
        /// False if not.
        /// </returns>
        public bool RemoveSavedEntry(string sectionName, string property)
        {
            return mpc.RemoveSavedEntry(sectionName, property);
        }
        /// <summary>
        /// Remove a stored entry from the list of entries waiting to be written.
        /// Accepts a Value for the Property.
        /// </summary>
        /// <param name="sectionName">Name of the Section that Property is in.</param>
        /// <param name="property">Name of Property to remove.</param>
        /// <param name="value">Value of Property to remove.</param>
        /// <returns>
        /// True if the entry was stored successfully,
        /// False if not.
        /// </returns>
        public bool RemoveSavedEntry(string sectionName, string property, string value)
        {
            return mpc.RemoveSavedEntry(sectionName, property, value);
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
            return mpc.RemoveSavedEntries(entries);
        }
        /// <summary>
        /// Write all of the saved entries to the ini file.
        /// </summary>
        /// <returns>
        /// True if the writes were successfull, False if not.
        /// </returns>
        public bool WriteSavedEntries()
        {
            return mpc.WriteSavedEntries();
        }

        #endregion


        private IController controller;

        private Controller.MultiplePropertyController mpc;

        private string FILE_PATH;
    }
}
