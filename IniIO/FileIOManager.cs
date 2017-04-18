using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IniIO
{
    /// <summary>
    /// Class that handles the bulk of the reading and writing to the INI file.
    /// </summary>
    public sealed class FileIOManager
    {
        private static object padlock = new object();

        // The following is used to write a single entry to an ini file.
        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString")]
        private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        /// <summary>
        /// Holds all the entries to write to the ini file(s).
        /// KEY: File path to write to.
        /// VALUE: List holding all the entries.
        /// </summary>
        private static Dictionary<string, List<Entry>> Entries = new Dictionary<string, List<Entry>>();

        /// <summary>
        /// The RemoveEntryFromList tries to remove an Entry that is stored for writing. If
        /// that fails, then the Message text of the Exception is stored in this string.
        /// 
        /// If the AddEntryToList fails to add an Entry to the list, this string will hold
        /// the Message thrown by the Exception.
        /// 
        /// The method GetLastError will simply return this string.
        /// </summary>
        private static string LastError = "No error message set.";

        /// <summary>
        /// Gets or sets the time the current file or directory was last accessed.
        /// </summary>
        /// <param name="fsi">
        /// Type: FileSystemInfo<br/>
        /// Provides the base class for both FileInfo and DirectoryInfo objects.
        /// </param>
        /// <returns>
        /// Type: System.DateTime<br/>
        /// The time that the current file or directory was last accessed.
        /// </returns>
        public static DateTime GetLastModifiedTime(FileSystemInfo fsi)
        {        
            return fsi.LastAccessTime;
        }

        /// <summary>
        /// Gets or sets the time, in coordinated universal time (UTC), that the current 
        /// file or directory was last accessed.
        /// </summary>
        /// <param name="fsi">
        /// Type: FileSystemInfo<br/>
        /// Provides the base class for both FileInfo and DirectoryInfo objects.
        /// </param>
        /// <returns>
        /// Type: System.DateTime<br/>
        /// The UTC time that the current file or directory was last accessed.
        /// </returns>
        public static DateTime GetLastModifiedTimeUTC(FileSystemInfo fsi)
        {            
            return fsi.LastAccessTimeUtc;
        }

        /// <summary>
        /// Modify the properties of the file when it has been changed.
        /// </summary>
        /// <param name="fsi">
        /// Type: FileSystemInfo<br/>
        /// Provides the base class for both FileInfo and DirectoryInfo objects.
        /// </param>
        /// <returns></returns>
        private static bool UpdateModifiedTime(FileSystemInfo fsi)
        {            
            try
            {
                fsi.CreationTime = fsi.LastWriteTime = fsi.LastAccessTime =
                    DateTime.Now;
            }
            catch (IOException ioex)
            {
                throw new IOException("Unable to modify last write. " + ioex.Message);
            }
            catch (PlatformNotSupportedException pex)
            {
                throw new PlatformNotSupportedException("Unable to modify last write. " + pex.Message);
            }
            catch (ArgumentOutOfRangeException aex)
            {
                throw new ArgumentOutOfRangeException("Unable to modify last write. " + aex.Message);
            }
            return true;
        }

        /// <summary>
        /// Add an entry (Section, Property, Value) to the queue.
        /// <br/><br/>
        /// A separate method will write all of the entries in the queue to
        /// the ini file.<br/>
        /// This can be used if you have a handful of changes you would like
        /// to execute all at once.  You queue them up and then when you are 
        /// ready to write all of your changes, you call FlushEntries();
        /// </summary>
        /// <param name="e">
        /// <list type="bullet">
        ///     <item>Item1: String: File path to write to</item>
        ///     <item>Item2: String: Name of the Section</item>
        ///     <item>Item3: String: Name of the Property</item>
        ///     <item>Item4: String: Value for the Property</item>
        /// </list>
        /// </param>
        /// <param name="fsi">
        /// Type: FileSystemInfo<br/>
        /// Provides the base class for both FileInfo and DirectoryInfo objects.
        /// </param>
        /// <returns>True if removing the Entry occurred without error.</returns>
        public static bool AddEntryToList(IEntry e, FileSystemInfo fsi)
        {
            bool retval = true;
            string filepath = fsi.FullName;
            try
            {
                if (Entries.ContainsKey(filepath))
                {
                    var entryCol = Entries[filepath] as List<Entry>;
                    var entry = new Entry(e.Section, e.Property, e.Value);
                    entryCol.Add(entry);
                    Entries[filepath] = entryCol;
                }
                else
                {
                    Entries.Add(filepath, new List<Entry>() { 
                        new Entry(e.Section, e.Property, e.Value) });
                }
            }
            catch (Exception ex)
            {
                // Not the greatest exception handling.
                // Just want to catch whether adding the Entry failed.
                retval = false;
                LastError = "AddEntryToList: " + ex.Message;
            }
            return retval;
        }

        /// <summary>
        /// Add a list of Entries the list of Entries to be written to the ini file.
        /// </summary>
        /// <param name="fsi"></param>
        /// <param name="entries"></param>
        /// <returns>
        /// True if all the Entries were added successfully, False if there was a failure.
        /// </returns>
        public static bool AddEntriesToList(FileSystemInfo fsi, IList<IEntry> entries)
        {
            bool retval = true;
            foreach (IList<Entry> elist in entries)
            {
                foreach (Entry e in elist)
                {
                    retval = AddEntryToList(e, fsi);
                    if (!retval) { retval = false; break; }
                }
            }
            return retval;
        }

        /// <summary>
        /// Remove an Entry from the collection so it is not written to the ini file.
        /// </summary>
        /// <param name="iniEntry">
        /// <list type="bullet">
        ///     <item>Item1: String: File path to write to</item>
        ///     <item>Item2: String: Name of the Section</item>
        ///     <item>Item3: String: Name of the Property</item>
        ///     <item>Item4: String: Value for the Property</item>
        /// </list>
        /// </param>
        /// <param name="fsi">
        /// Type: FileSystemInfo<br/>
        /// Provides the base class for both FileInfo and DirectoryInfo objects.
        /// </param>
        /// <returns>
        /// True if removing the Entry occurred without error.
        /// </returns>
        public static bool RemoveEntryFromList(IEntry iniEntry, FileSystemInfo fsi)
        {
            bool retval = true;
            var tmp = new List<Entry>();

            try
            {
                foreach (KeyValuePair<string, List<Entry>> kvp in Entries)
                {
                    if (fsi.FullName == kvp.Key)
                    {
                        foreach (var e in kvp.Value)
                        {
                            if (!iniEntry.Equals(e))
                            {
                                // Store any entry that isn't one being removed
                                tmp.Add(e);
                            }}}}
            }
            catch (Exception ex)
            {
                retval = false;
                LastError = "RemoveEntryFromList: " + ex.Message;
            }
            finally
            {
                // Write the entries back without the one removed.
                Entries[fsi.FullName] = tmp;
            }
            return retval;
        }

        /// <summary>
        /// Remove multiple Entries from the main list.
        /// <br/><br/>
        /// If an error occurs when an Entry is trying to be removed
        /// then the error is stored in RemoveEntryFromListLastError.
        /// </summary>
        /// <param name="fsi">
        /// Type: FileSystemInfo<br/>
        /// Provides the base class for both FileInfo and DirectoryInfo objects.
        /// </param>
        /// <param name="entries">
        /// List that holds all of the entries, as Entry objects.
        /// </param>
        /// <returns></returns>
        public static bool RemoveEntriesFromList(FileSystemInfo fsi, IList<IEntry> entries)
        {
            bool retval = true;
            foreach (IList<Entry> elist in entries)
            {
                foreach (Entry e in elist)
                {
                    retval = RemoveEntryFromList(e, fsi);
                    if (!retval) { retval = false; break; }
                }
            }
            return retval;
        }

        /// <summary>
        /// Check to see if the Entry is already stored in the master list
        /// that will be written.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <param name="fsi">
        /// Type: FileSystemInfo<br/>
        /// Provides the base class for both FileInfo and DirectoryInfo objects.
        /// </param>
        /// <returns>
        /// True if the list contains the entry, False if not.
        /// </returns>
        public static bool ListContainsEntry(IEntry e, FileSystemInfo fsi)
        {
            bool retval = false;
            try
            {
                foreach (KeyValuePair<string, List<Entry>> kvp in Entries)
                {
                    if (fsi.FullName == kvp.Key)
                    {
                        if (kvp.Value.Contains(e)) { retval = true; }
                    }
                }
            }
            catch (Exception ex)
            {
                LastError = "ListContainsEntry: " + ex.Message;
            }
            return retval;
        }

        /// <summary>
        /// Write all of the entries in the queue to the ini file on disk.
        /// <br/><br/>
        /// New entries/Properties are added directly underneath the Section.
        /// </summary>
        public static bool FlushEntries()
        {
            bool retval = true;
            try
            {
                foreach (KeyValuePair<string, List<Entry>> kvp in Entries)
                {
                    ThreadPool.QueueUserWorkItem((c) =>
                    {
                        try
                        {
                            // Write the queued items to each file.
                            foreach (Entry e in kvp.Value)
                            {
                                WritePrivateProfileString(e.Section, e.Property, e.Value, kvp.Key);
                            }
                        }
                        catch (InvalidOperationException iex)
                        {
                            throw new InvalidOperationException(iex.Message);
                        }
                    });
                }
                // Clear the list.
                Entries.Clear();
            }
            catch (Exception ex)
            {
                retval = false;
                LastError = "FlushEntries: " + ex.Message;
            }
            return retval;
        }

        /// <summary>
        /// Uses kernel32.dll method to write and entry to the config file.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <param name="fsi">File to write to.</param>
        /// <returns></returns>
        public static bool WriteEntry(IEntry e, FileSystemInfo fsi)
        {
            bool retval = false;
            try
            {
                lock (padlock)
                {
                    retval = WritePrivateProfileString(e.Section, e.Property, e.Value, fsi.FullName);
                    if (retval) { UpdateModifiedTime(fsi); }
                }
            }
            catch (Exception ex)
            {
                retval = false;
                LastError = "WriteEntry: " + ex.Message;
            }
            return retval;
        }

        /// <summary>
        /// Remove a Property/Value pair from a Section in the ini file.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <param name="fsi"></param>
        /// <returns>
        /// True is the Property/Value is removed, False if not.
        /// </returns>
        public static bool RemoveEntry(IEntry e, FileSystemInfo fsi)
        {
            bool retval = true;
            try
            {
                retval = WritePrivateProfileString(e.Section, e.Property, null, fsi.FullName);
            }
            catch (Exception ex)
            {
                retval = false;
                LastError = "RemoveEntry: " + ex.Message;
            }
            return retval;           
        }

        /// <summary>
        /// Get the error thrown by any method that writes the Exception text to the
        /// LastError string.
        /// </summary>
        /// <returns>
        /// String object that contains the error text.
        /// </returns>
        public static string GetLastError() { return LastError; }
    }
}
