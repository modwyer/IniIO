using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IniIO
{
    /// <summary>
    /// This class retrieves and holds the contents of the ini file.
    /// Instead of the IConfigurationFileIO interface returning
    /// a set data structure like a Dictionary, it will return
    /// an instance of this class.  
    /// <br/><br/>
    /// The internal data structure that holds the contents of the 
    /// ini file are encapsulated within this class. 
    /// <br/><br/>
    /// This class keeps track of the last time the ini file was read into
    /// memory and will read it in again if it sees that the data it has
    /// stored is not the most recent.
    /// <br/><br/>
    /// This class implements the IFileContents interface.
    /// </summary>
    public sealed class FileContents : IniIO.IFileContents
    {
        private DateTime LAST_MODIFIED_TIME;
        private Dictionary<string, List<string>> FILE_TEXT;
        private List<string> PROPERTY_LIST;
        private string FILE_NAME;
        internal FileModifier FILE_MODIFIER;

        /// <summary>
        /// Default class constructor.
        /// </summary>
        /// <param name="filename">
        /// Path to the INI file.
        /// </param>
        public FileContents(string filename) 
        {
            FILE_NAME = filename;
            FILE_TEXT = new Dictionary<string, List<string>>();
            PROPERTY_LIST = new List<string>();
            contents = new Dictionary<string, Dictionary<string, string>>();
            
            // Create new FileModifier.  It will throw an exception if can't be created.
            FILE_MODIFIER = new FileModifier(filename);
        }

        private Dictionary<string, Dictionary<string, string>> contents;
        /// <summary>
        /// The contents of the INI file represented as a Dictionary data structure.
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
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Contents
        {
            get { return contents; }
        }


        /// <summary>
        /// Get the value out of a Section name taken directly from the ini file.
        /// </summary>
        /// <param name="sectionNameRaw">
        /// String value of the name pulled from the file. ie - [debug]
        /// </param>
        /// <returns>
        /// Cleaned-up Section name. ie - above param becomes debug
        /// </returns>
        private string CleanSectionName(string sectionNameRaw)
        {
            string retval = "";
            #region Explanation of the below RegEx
            /*
                 * From: http://codeasp.net/blogs/raghav_khunger/microsoft-net/1888/c-regex-extract-the-text-between-square-brackets-without-returning-the-brackets-themselves
                 * (?<=\[)(.*?)(?=\])
                 * Assert that the regex below can be matched, with the match ending at this position (positive lookbehind) «(?<=\[)»
                 * Match the character “[” literally «\[»
                 * Match the regular expression below and capture its match into backreference number 1 «(.*?)»
                 * Match any single character that is not a line break character «.*?»
                 * Between zero and unlimited times, as few times as possible, expanding as needed (lazy) «*?»
                 * Assert that the regex below can be matched, starting at this position (positive lookahead) «(?=\])»
                 * Match the character “]” literally «\]»
                 */
            #endregion Explanation of the below RegEx
            string pattern = @"(?<=\[)(.*?)(?=\])";
            Match output = Regex.Match(sectionNameRaw, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (output.Success)
            {
                retval = output.Groups[0].Value;
            }
            return retval;
        }

        /// <summary>
        /// Get rid of the square brackets from the all of the section names.
        /// </summary>
        /// <returns>
        /// IEnumerable list of strings that are the Section names from the 
        /// INI file, without their enclosing brackets.
        /// </returns>
        private IEnumerable<string> CleanSectionNames()
        {
            List<string> temp = new List<string>();

            foreach (string s in FILE_TEXT.Keys)
            {
                string pattern = @"(?<=\[)(.*?)(?=\])";
                Match output = Regex.Match(s, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (output.Success)
                {
                    temp.Add(output.Groups[0].Value);
                }
            }
            return temp;
        }

        /// <summary>
        /// String split operation that splits based on <para>loc</para> value.
        /// <br/><br/>
        /// The Split method in the .NET library splits on any
        /// instance of the split character.  If the string contains
        /// the split character more than once then the string is split 
        /// multiple times.  
        /// <br/><br/>
        /// <remarks>
        /// For these ini entries, the format is:<br/>
        /// <list type="bullet">
        ///     <item>stringProperty=stringValue</item>
        /// </list>     
        /// The stringProperty may not contain a equals sign.<br/>
        /// The stringValue is allowed to contain any number of equals signs.
        /// </remarks>
        /// </summary>
        /// <param name="loc">
        /// Location, index of the character in the string to split on.
        /// </param>
        /// <param name="s">
        /// String to split in two.
        /// </param>
        /// <returns>
        /// String array containing the string split.
        /// Example:<br/>
        /// <list type="bullet">
        ///     <item>String: modwyer=XtOnLPcds4G2B9FQx11l+g==</item>
        ///     <item>Returned string array:
        ///         <list type="bullet">
        ///             <item>string[0] = modwyer</item>
        ///             <item>string[1] = XtOnLPcds4G2B9FQx11l+g==</item>
        ///         </list>
        ///     </item>
        /// </list>
        /// </returns>
        private string[] splitString(int loc, string s)
        {
            var str1 = s.Substring(0, loc);
            var str2 = s.Substring(loc+1, s.Length - loc-1);
            return new [] { str1, str2 };
        }

        /// <summary>
        /// Loops the raw content read in from the ini file and fills the Contents 
        /// Dictionary with all the Properties broken out by Section.
        /// </summary>
        private void ParseAllContents()
        {
            char c = '=';
            Dictionary<string, string> value = null;

            foreach (string s in FILE_TEXT.Keys)
            {
                // Reset the Dictionary<> that is the value for each Section name.
                value = new Dictionary<string, string>();
                foreach (string val in FILE_TEXT[s])
                {
                    int loc = val.IndexOf(c);
                    if (0 < loc)
                    {
                        var split = splitString(loc, val);
                        if (split.Length > 1)
                        {
                            if (!value.ContainsKey(split[0]))
                            {
                                value.Add(split[0], split[1]);
                            }
                        }
                    }
                }
                // Add in the Key/Value for the main dictionary
                contents.Add(CleanSectionName(s), value);
            }
        }

        /// <summary>
        /// Take all the Properties and put them into their own enumerable list.
        /// </summary>
        /// <returns>
        /// IEnumerable collection of strings
        /// </returns>
        private void GetPropertyList()
        {
            foreach (List<string> ls in FILE_TEXT.Values)
            {
                foreach (string s in ls)
                {
                    var ss = s.Split('=');
                    // Do not add empty strings or comments
                    if (0 < ss[0].Length && ';' != ss[0][0])
                    {
                        PROPERTY_LIST.Add(ss[0]);
                    }
                }
            }
        }


        #region IFileContents Members

        /// <summary>
        /// Get the contents of the file.
        /// <br/><br/>
        /// <remarks>
        /// This method assumes some things.<br/><br/>
        /// You are only using open square brackets at the beginning
        /// of a Section name and never for a name of a Property.<br/>
        /// You don't duplicate Section names.<br/>
        /// You don't duplicate Property names within the same Section.<br/><br/>
        /// Not following these basic rules in your ini file will yeild
        /// unexpected results.
        /// </remarks>
        /// <returns>True if the reading of the file was successful, False if not.</returns>
        /// </summary>
        public bool ReadFile()
        {
            bool retval = false;

            if (!File.Exists(FILE_NAME)) { return retval; }

            using (StreamReader sr = File.OpenText(FILE_NAME))
            {
                string input;
                string currentSection = "";
                while ((input = sr.ReadLine()) != null)
                {
                    if (0 < input.Length && input[0] == '[')
                    {
                        FILE_TEXT.Add(input, new List<string>());
                        currentSection = input;
                    }
                    else
                    {
                        if ("".Equals(currentSection)) 
                        { throw new IOException("Property or Value found outside of a valid Section. Check that your INI file is formatted correctly."); }

                        List<string> items = FILE_TEXT[currentSection];
                        items.Add(input);
                    }
                }
                retval = true;
                LAST_MODIFIED_TIME = FILE_MODIFIER.GetLastWriteTime();
            }
            return retval;
        }

        /// <summary>
        /// Return the contents, broken out by Section name with each Section
        /// containing all of its Properties (name/value pairs).
        /// <br/><br/>
        /// If there are any duplicate properties, the second one is ignored - assuming 
        /// that you have made a mistake by having two properties named the same thing.
        /// </summary>
        public void GetAllContent()
        {
            // Check that the initial read of the ini file was done or not.
            bool check1 = (null == FILE_TEXT) 
                ? true : false;
            // Check if the last modified time of the config file was set or not.
            bool check2 = (null == LAST_MODIFIED_TIME) 
                ? true : false;
            // Check if the config file has been modified since last time it was read.
            bool check3 = (LAST_MODIFIED_TIME < FILE_MODIFIER.GetLastWriteTime()) 
                ? true : false;

            // If any of the above checks are true, we need to read in the file again.
            if (check1 || check2 || check3) { ReadFile(); }
                        
            ParseAllContents();
        }

        /// <summary>
        /// Check to see if a property exists.
        /// </summary>
        /// <param name="property">Name of the Property to search for</param>
        /// <returns>
        /// True if the Property was found in the ini file.<br/>
        /// False if the Property was not found in the ini file.
        /// </returns>
        public bool ContainsProperty(string property)
        {
            // Double check that the file has been read in.
            // If not, read it in.  Otherwise, no data to loop over.
            if (contents.Count < 1) { GetAllContent(); }

            // Check to see if the PROPERTY_LIST has been made yet.
            // If not, then fill it for this and future searches.
            if (PROPERTY_LIST.Count < 1) { GetPropertyList(); }

            return (PROPERTY_LIST.Contains(property)) ? true : false;
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
            // Double check that the file has been read in.
            // If not, read it in.  Otherwise, no data to loop over.
            if (contents.Count < 1) { GetAllContent(); }

            Entry e = null;

            foreach (KeyValuePair<string, Dictionary<string, string>> kvp in contents)
            {
                var val = kvp.Value;
                foreach (KeyValuePair<string, string> kvp2 in val)
                {
                    if (kvp2.Key == property)
                    {
                        Debug.WriteLine(kvp.Key + " | " + property + " | " + kvp2.Value);
                        e = new Entry(kvp.Key, property, kvp2.Value);
                    }
                }
            }
            return e as Entry;
        }

        /// <summary>
        /// Get the collection of Section names pulled from ini file.
        /// </summary>
        /// <returns>
        /// IEnumerable collection of strings
        /// </returns>
        public IEnumerable<string> GetSectionNames()
        {
            return CleanSectionNames() as IEnumerable<string>;
        }

        /// <summary>
        /// Retrieve a Value for a Property.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.  Entry to read Value for.
        /// </param>
        /// <returns>Value as a string</returns>
        public string ReadEntryValue(IEntry e)
        {
            string retval = "";
            foreach (KeyValuePair<string, Dictionary<string, string>> kvp in contents)
            {
                if (kvp.Key == e.Section)
                {
                    var val = kvp.Value;
                    foreach (KeyValuePair<string, string> kvp2 in val)
                    {
                        if (kvp2.Key == e.Property)
                        {
                            retval = kvp2.Value;
                        }
                    }
                }
            }
            return retval;
        }

        /// <summary>
        /// Retrieve the Value for a Property and save it back to the IEntry object
        /// that is passed as the parameter.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.  Entry to read.
        /// </param>
        /// <returns>
        /// The IEntry object with the Value set if it is found.
        /// </returns>
        public IEntry ReadEntry(IEntry e)
        {            
            foreach (KeyValuePair<string, Dictionary<string, string>> kvp in contents)
            {
                if (kvp.Key == e.Section)
                {
                    var val = kvp.Value;
                    foreach (KeyValuePair<string, string> kvp2 in val)
                    {
                        if (kvp2.Key == e.Property)
                        {
                            e.Value = kvp2.Value;
                        }
                    }
                }
            }
            return e;
        }

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
            return FILE_MODIFIER.WriteEntry(e);
        }

        /// <summary>
        /// Remove a Property/Value pair from a Section.
        /// </summary>
        /// <param name="e">
        /// Instance of an object from a class that implements
        /// the IEntry interface.  Entry to remove.
        /// </param>
        /// <returns>
        /// True if the Entry was removed successfully,
        /// False if not.
        /// </returns>
        public bool RemoveEntry(IEntry e)
        {
            return FILE_MODIFIER.RemoveEntry(e);
        }

        /// <summary>
        /// Return the contents of the file as a string.
        /// </summary>
        /// <returns></returns>
        public string toString() { return FILE_TEXT.ToString(); }
        
        #endregion IFileContents Members
    }
}
