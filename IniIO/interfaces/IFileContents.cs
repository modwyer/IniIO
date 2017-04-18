using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IniIO
{
    /// <summary>
    /// Interface for a class that handles the data read in from an INI file.
    /// <br/><br/>
    /// A FileContents class reads in the INI file and holds it in memory.  This
    /// interface exposes methods that allow other classes to get at the contents
    /// of the file in various ways.
    /// </summary>
    public interface IFileContents
    {
        /// <summary>
        /// Get the contents of the file.<br/>
        /// This method assumes some things:<br/>
        /// <list type="bullet">
        /// <item>You are only using open square brackets at the beginning
        ///         of a Section name and never for a name of a Property.</item>
        /// <item>You don't duplicate Section names.</item>
        /// <item>You don't duplicate Property names within the same Section.</item>
        /// </list>
        /// <remarks>
        /// Not following these basic rules in your ini file will yield
        /// unexpected results.
        /// </remarks> 
        /// </summary>
        bool ReadFile();
        /// <summary>
        /// Return the contents, broken out by Section name with each Section
        /// containing all of its Properties (name/value pairs).
        /// <br/><br/>
        /// If there are any duplicate properties, the second one is ignored - assuming 
        /// that you have made a mistake by having two properties named the same thing.
        /// </summary>
        void GetAllContent();
        /// <summary>
        /// Check to see if a property exists.
        /// </summary>
        /// <param name="property">Name of the Property to search for</param>
        /// <returns>
        /// True if the Property was found in the ini file.
        /// False if the Property was not found in the ini file.
        /// </returns>
        bool ContainsProperty(string property);
        /// <summary>
        /// Check to see if a property exists.
        /// </summary>
        /// <param name="property">
        /// Name of the Property to search for
        /// </param>
        /// <returns>
        /// An Entry object containing the info about the Property.
        /// </returns>
        IEntry ContainsPropertyReturnInfo(string property);
        /// <summary>
        /// Get the collection of Section names pulled from ini file.
        /// </summary>
        /// <returns>
        /// IEnumerable collection of strings
        /// </returns>
        IEnumerable<string> GetSectionNames();
        /// <summary>
        /// Retrieve a Value for a Property.
        /// </summary>
        /// <param name="e">Entry object</param>
        /// <returns>Value as a string</returns>
        string ReadEntryValue(IEntry e);
        /// <summary>
        /// Retrieve the Value for a Property and save it back to the IEntry object
        /// that is passed as the parameter.
        /// </summary>
        /// <param name="e"></param>
        /// <returns>
        /// The IEntry object with the Value set if it is found.
        /// </returns>
        IEntry ReadEntry(IEntry e);
        /// <summary>
        /// Write a Name/Value to a Section.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <returns>False if the write was unsucessful</returns>
        bool WriteEntry(IEntry e);
        /// <summary>
        /// Remove a Property/Value pair from a Section.
        /// </summary>
        /// <param name="e">Entry object</param>
        /// <returns>
        /// True if the Entry was removed successfully,
        /// False if not.
        /// </returns>
        bool RemoveEntry(IEntry e);
        /// <summary>
        /// Return the contents of the file as a string.
        /// </summary>
        string toString();
    }
}
