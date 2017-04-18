using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IniIO
{
    /// <summary>
    /// Interface to be implemented when you want to have the ability to write
    /// multiple entries to an ini file at once.
    /// <br/><br/>
    /// The FileModifier class which is owned by the FileContents class which is
    /// owned by the Controller class, contains the methods that communicate with
    /// the static FileIOManager class, which is the class that implements these methods.
    /// <br/><br/>
    /// For an example see the Controller class and its nested class MultiplePropertyController, which
    /// implements this interface, and the IniCreator class which accesses the nested class.
    /// </summary>
    public interface IMultiplePropertyIO
    {
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
        bool SaveEntry(string sectionName, string property);
        /// <summary>
        /// Store an entry to be written at once with other entries.
        /// <br/><br/>
        /// Accepts a Value for the Property.
        /// </summary>
        /// <param name="sectionName">
        /// Name of the Section to save the Property in.
        /// </param>
        /// <param name="property">
        /// Name of the Property to store for write.
        /// </param>
        /// <param name="value"></param>
        /// <returns>
        /// True if the entry was stored successfully,
        /// False if not.
        /// </returns>
        bool SaveEntry(string sectionName, string property, string value);
        /// <summary>
        /// Store multiple entries at once.
        /// </summary>
        /// <param name="entries">
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
        /// </param>
        /// <returns></returns>
        bool SaveEntries(IDictionary<string, IDictionary<string, string>> entries);
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
        bool RemoveSavedEntry(string sectionName, string property);
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
        /// <param name="value"></param>
        /// <returns>
        /// True if the entry was stored successfully,
        /// False if not.
        /// </returns>
        bool RemoveSavedEntry(string sectionName, string property, string value);
        /// <summary>
        /// Removed multiple stored entries from the list of entries waiting to be
        /// written.
        /// </summary>
        /// <param name="entries">
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
        /// </param>
        /// <returns>
        /// True if the entry was stored successfully,
        /// False if not.
        /// </returns>
        bool RemoveSavedEntries(IDictionary<string, IDictionary<string, string>> entries);
        /// <summary>
        /// Write all of the saved entries to the ini file.
        /// </summary>
        /// <returns>
        /// True if the writes were successfull, False if not.</returns>
        bool WriteSavedEntries();
    }
}
