using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace IniIO
{
    /// <summary>
    /// Wrapper class for the FileIOManager class.
    /// </summary>
    public sealed class FileModifier
    {
        private FileSystemInfo fsi;

        /// <summary>
        /// Default class constructor.
        /// </summary>
        /// <param name="filename">
        /// Path to the INI file.
        /// </param>
        public FileModifier(string filename)
        {
            this.Filename = filename;
        }


        /// <summary>
        /// True if an instance of FileSystemInfo was created successfully 
        /// from filename param; otherwise, False
        /// </summary>
        private bool isCreated = false;
        /// <summary>
        /// True if an instance of FileSystemInfo was created successfully 
        /// from filename param; otherwise, False
        /// </summary>
        public bool IsCreated
        {
            get { return isCreated; }
            private set { isCreated = value; }
        }

        /// <summary>
        /// This is the path to the ini file.
        /// <br/><br/>
        /// It is initialized to this value to be able to check whether filename 
        /// gets initialized or not.  The format was chosen to be unique and
        /// most likely never equal to a real path.
        /// </summary>
        private string filename = "~NA~";
        /// <summary>
        /// This is the path to the ini file.
        /// <br/><br/>
        /// When set, the FileSystemInfo object is created using the filename.
        /// </summary>
        public string Filename
        {
            get { return filename; }
            set
            {
                if (File.Exists(value))
                {
                    if (null == fsi)
                    {
                        try
                        {
                            fsi = new FileInfo(value);
                            filename = value;
                            isCreated = true;
                        }
                        // This exception would be caught by ArgumentException-
                        // Keeping it in for testing, to know exactly where it fails (if/when).
                        catch (ArgumentNullException anex)
                        {
                            throw new ArgumentNullException(anex.Message);
                        }
                        catch (SecurityException secex)
                        {
                            throw new SecurityException(secex.Message);
                        }
                        catch (ArgumentException aex)
                        {
                            throw new ArgumentException(aex.Message);
                        }
                        catch (UnauthorizedAccessException uex)
                        {
                            throw new UnauthorizedAccessException(uex.Message);
                        }
                        catch (PathTooLongException pex)
                        {
                            throw new PathTooLongException(pex.Message);
                        }
                        catch (NotSupportedException nex)
                        {
                            throw new NotSupportedException(nex.Message);
                        }
                    }
                }
            }
        }      


        /// <summary>
        /// A delegate to handle calling the two versions of GetLastModifiedTime
        /// </summary>
        /// <param name="sourceFile">
        /// The INI file.
        /// <br/><br/>
        /// Type: FileSystemInfo<br/>
        /// Provides the base class for both FileInfo and DirectoryInfo objects.
        /// </param>
        /// <returns>Type: System.DateTime</returns>
        private delegate DateTime delegateGetLastModifiedTime(FileSystemInfo sourceFile);

        /// <summary>
        /// Get FileSystemInfo.LastWriteTime for the configuration file.
        /// </summary>
        /// <returns></returns>
        public DateTime GetLastWriteTime()
        {
            delegateGetLastModifiedTime delGetTime = 
                new delegateGetLastModifiedTime(FileIOManager.GetLastModifiedTime);
            return delGetTime(fsi);
        }

        /// <summary>
        /// Get FileSystemInfo.LastWriteTimeUtc for the configuration file.
        /// </summary>
        /// <returns></returns>
        public DateTime GetLastWriteTimeUtc()
        {
            delegateGetLastModifiedTime delGetTimeUtc =
                new delegateGetLastModifiedTime(FileIOManager.GetLastModifiedTimeUTC);
            return delGetTimeUtc(fsi);
        }


        /// <summary>
        /// A delegate to handle calling methods that handle single Entry IO.
        /// </summary>
        /// <param name="entry">
        /// Instance of an object from a class that implements
        /// the IEntry interface.
        /// </param>
        /// <param name="sourceFile">
        /// The INI file.
        /// Type: FileSystemInfo
        /// Provides the base class for both FileInfo and DirectoryInfo objects.
        /// </param>
        /// <returns>
        /// True if the action was successful, False if not.
        /// </returns>
        private delegate bool delegateEntryIO(IEntry entry, FileSystemInfo sourceFile);

        /// <summary>
        /// A delegate to handle calling methods that handle multiple Entry IO.
        /// </summary>
        /// <param name="sourceFile">
        /// The INI file.
        /// Type: FileSystemInfo
        /// Provides the base class for both FileInfo and DirectoryInfo objects.
        /// </param>
        /// <param name="entries">
        /// A list of Entry objects.
        /// </param>
        /// <returns>
        /// True if the action was successful, False if not.
        /// </returns>
        private delegate bool delegateMultiEntryIO(FileSystemInfo sourceFile, IList<IEntry> entries);

        /// <summary>
        /// Write a Name/Value to a section.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.  Entry to write.
        /// </param>
        /// <returns>False if the write was unsucessful</returns>
        public bool WriteEntry(IEntry e)
        {
            delegateEntryIO delWriteEntry = 
                new delegateEntryIO(FileIOManager.WriteEntry);
            return delWriteEntry(e, fsi);
        }

        /// <summary>
        /// Remove a Property/Value pair from the ini file.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.  Entry to write.
        /// </param>
        /// <returns>
        /// True if the Entry was removed, False if not.
        /// </returns>
        public bool RemoveEntry(IEntry e)
        {
            delegateEntryIO delRemoveEntry = 
                new delegateEntryIO(FileIOManager.RemoveEntry);
            return delRemoveEntry(e, fsi);
        }

        /// <summary>
        /// Add an ini entry to a collection that can then be written all at once.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.  Entry to write.
        /// </param>
        /// <returns>
        /// True if the Entry was stored successfully, False if there was a problem.
        /// </returns>
        public bool StoreEntryForWrite(IEntry e)
        {
            delegateEntryIO delStoreEntry = 
                new delegateEntryIO(FileIOManager.AddEntryToList);            
            return delStoreEntry(e, fsi);
        }

        /// <summary>
        /// Add a list of Entries the list of Entries to be written to the ini file.
        /// </summary>
        /// <param name="entries">
        /// List of Entry objects.
        /// </param>
        /// <returns>
        /// True if all the Entries were added successfully, False if there was a failure.
        /// </returns>
        public bool StoreEntriesForWrite(IList<IEntry> entries)
        {
            delegateMultiEntryIO delStoreMulti = 
                new delegateMultiEntryIO(FileIOManager.AddEntriesToList);
            return delStoreMulti(fsi, entries);
        }

        /// <summary>
        /// Remove an ini entry from the collection so it is not written to file.
        /// </summary>
        /// <returns>
        /// True if the Entry was removed from the stored list,
        /// False if there was a problem.
        /// </returns>
        public bool RemoveStoredEntry(IEntry e)
        {
            delegateEntryIO delRemoveStoredEntry = 
                new delegateEntryIO(FileIOManager.RemoveEntryFromList);
            return delRemoveStoredEntry(e, fsi);            
        }

        /// <summary>
        /// Remove a list of Entries from the list of entries waiting to
        /// be written.
        /// </summary>
        /// List of Entry objects.
        /// <returns>
        /// True if all of the Entries were successfully removed from the 
        /// list, False if there was a problem.
        /// </returns>
        public bool RemoveStoredEntries(IList<IEntry> entries)
        {
            delegateMultiEntryIO delRemoveMulti = 
                new delegateMultiEntryIO(FileIOManager.RemoveEntriesFromList);
            return delRemoveMulti(fsi, entries);            
        }

        /// <summary>
        /// Check to see if the stored list of Entries written en masse
        /// contains an Entry or not.  
        /// Can be used after a call to StoreEntryForWrite or RemoveStoredEntry to validate that
        /// the Entry was really added/removed.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool ContainsStoredEntry(IEntry e)
        {
            delegateEntryIO delContainsStoredEntry = 
                new delegateEntryIO(FileIOManager.ListContainsEntry);
            return delContainsStoredEntry(e, fsi);
        }


        /// <summary>
        /// A parameterless delegate that returns a Boolean.
        /// </summary>
        /// <returns>
        /// True if the action was successful,
        /// False if not.
        /// </returns>
        private delegate bool delegateGenericReturnBool();
        /// <summary>
        /// A parameterless delegate that returns a string.
        /// </summary>
        /// <returns>
        /// True if the action was successful,
        /// False if not.
        /// </returns>
        private delegate string delegateGenericReturnString();

        /// <summary>
        /// Write any stored Entry to its ini file.
        /// </summary>
        /// <returns>
        /// True if the writes were successful, False if not.
        /// </returns>
        public bool WriteStoredEntries() 
        {
            delegateGenericReturnBool delWriteAll =
                new delegateGenericReturnBool(FileIOManager.FlushEntries);
            return delWriteAll();
        }

        /// <summary>
        /// If one of the methods called from this class return false, you can call
        /// this method to try and get what the Exception Message was.
        /// </summary>
        /// <returns>
        /// string containing the Exception.Message.
        /// </returns>
        public string GetLastError() 
        {
            delegateGenericReturnString delGetLastError =
                new delegateGenericReturnString(FileIOManager.GetLastError);
            return delGetLastError();            
        }
    }
}
