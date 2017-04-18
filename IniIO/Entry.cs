using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IniIO
{
    /// <summary>
    /// Class that models an entry in the INI file.
    /// <br/><br/>
    /// An Entry consists of a Section, Property, and Value.  
    /// <br/><br/>
    /// This class also implements IEquatable to allow for testing the 
    /// equality of two Entry objects.
    /// <remarks>IEquatable: "http://msdn.microsoft.com/en-us/library/ms131187(v=vs.100).aspx"</remarks>
    /// </summary> 
    public sealed class Entry : IniIO.IEntry, IEquatable<Entry>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Entry() { }

        /// <summary>
        /// Optional constructor that only takes the Section and Property values.
        /// </summary>
        /// <param name="section">Name of the Section.</param>
        /// <param name="property">Name of the Property.</param>
        public Entry(string section, string property)
        {
            Section = section;
            Property = property;
        }

        /// <summary>
        /// Optional constructor that takes all three values as parameters.
        /// </summary>
        /// <param name="section">Name of the Section.</param>
        /// <param name="property">Name of the Property.</param>
        /// <param name="value">Value of the Property.</param>
        public Entry(string section, string property, string value)
        {
            Section = section;
            Property = property;
            Value = value;
        }

        /// <summary>
        /// Override this to get HashCode used in equals comparison.
        /// </summary>
        /// <returns>
        /// Integer value of the hashcode.
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 37 + Section.GetHashCode();
            hash = hash * 37 + Property.GetHashCode();
            hash = hash * 37 + Value.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Checks to see if the object passed in is equal to this
        /// Entry object.
        /// </summary>
        /// <param name="obj">
        /// object to compare for equality.
        /// </param>
        /// <returns>
        /// True if the object is equal to this object,
        /// False if they are not equal.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Entry);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">
        /// Entry object to compare for equality.
        /// </param>
        /// <returns>
        /// True if the object is equal to this object,
        /// False if they are not equal.
        /// </returns>
        public bool Equals(Entry other)
        {
            return (other != null &&
                    other.Section  == this.Section  &&
                    other.Property == this.Property &&
                    other.Value    == this.Value);
        }
        

        #region IEntry Members

        /// <summary>
        /// The bracketed name of the group that the 
        /// Property belongs to.  ie: [debug]
        /// </summary>
        public string Section { get; set; }
        /// <summary>
        /// The name of an item within a Section.  (aka Field, Key)
        /// </summary>
        public string Property { get; set; }
        /// <summary>
        /// The content stored for a specific Property.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Tries to return the Value of the Property as an Integer.
        /// <br/><br/>
        /// The Value as an Integer is returned as an out parameter. 
        /// </summary>
        /// <param name="value">
        /// Out parameter that holds the Value as an Integer.
        /// </param>
        /// <returns>
        /// True if the Value can be converted to an Integer.<br/>
        /// False if it can not be converted to an Integer.
        /// </returns>
        public bool AsInteger(out int value)
        {
            bool success = Int32.TryParse(this.Value, out value);
            return success;
        }

        /// <summary>
        /// Tries to return the Value of the Property as a long.
        /// <br/><br/>
        /// The Value as a long is returned as an out parameter.
        /// </summary>
        /// <param name="value">
        /// Out parameter that holds the Value as a Long.
        /// </param>
        /// <returns>
        /// True if the Value can be converted to a Long,
        /// False if not.
        /// </returns>
        public bool AsLong(out long value)
        {
            bool success = long.TryParse(this.Value, out value);
            return success;
        }

        /// <summary>
        /// Tries to return the Value of the Property as a Boolean.
        /// </summary>
        /// <param name="value">
        /// Out parameter that holds the Value as a Boolean.
        /// </param>
        /// <returns>
        /// True is the Value can be parsed as Boolean=True.<br/>
        /// False if the Value can not be parsed as a Boolean.
        /// </returns>
        public bool AsBoolean(out bool value)
        {
            bool retval = Boolean.TryParse(this.Value, out value);
            return retval;
        }

        #endregion IEntry Members
    }
}
