using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IniIO;

namespace Client
{
    class Program
    {        
        static void Main(string[] args)
        {
            // Create new instance of the ConfigurationFileIO class
            ConfigurationFileIO iniMgr = 
                new ConfigurationFileIO(Environment.CurrentDirectory +"\\MyECFMail.ini");
            ConfigurationFileIO iniMgr2 =
                new ConfigurationFileIO(Environment.CurrentDirectory + "\\empty.ini");

            // See if the ini file contains a certain Section name.
            bool success4 = iniMgr.ContainsSection("users");
            Console.WriteLine("TEST ContainsSection():\nContains Section 'users' = {0}\n", success4);
            
            // Check to see if file contains the "cloud" Property
            // This only tells you if the Property exists in this ini file.
            // If the Property name is used in multiple Sections then the first one that
            // is found is what is used to test the validity of this method.
            bool containsCloud = iniMgr.ContainsProperty("cloud");
            Console.WriteLine("TEST ContainsProperty():\nini file contains 'cloud' = '{0}'\n", containsCloud);

            // Now check to see if it contains the "cloud" Property, and get its Section and Value
            string section = "";
            string value = "";
            containsCloud = iniMgr.ContainsProperty("cloud", out section, out value);
            Console.WriteLine("TEST ContainsProperty(out,out):\nini file contains 'cloud' = '{0}' under Section '{1}' with a Value of '{2}'\n", containsCloud, section, value);

            // Try to read a Value for a Property
            string strValue = iniMgr.ReadValue("users", "modwyer");
            Console.WriteLine("TEST ReadValue():\nValue for 'modwyer' under Section 'users' = '{0}'\n", strValue);

            // Try and get the Value of the Property 'cloud' as a Boolean.
            bool valAsBool;
            bool success = iniMgr.ReadValueAsBoolean("environment", "cloud", out valAsBool);
            Console.WriteLine("TEST AsBoolean():\n'cloud' Value parsed as Boolean successfully = '{0}', and the Value is '{1}'\n", success, valAsBool);

            // Try and get the Value of the Property 'LID' as an Integer.
            int valAsInt;
            bool success2 = iniMgr.ReadValueAsInteger("license", "LID", out valAsInt);
            Console.WriteLine("TEST AsInteger():\n'LID' Value parsed as Integer successfully = '{0}', and the Value is '{1}'\n", success2, valAsInt);

            // Try and get the Value of the Property 'ID' as an long.
            long valAsLong;
            bool success3 = iniMgr.ReadValueAsLong("license", "ID", out valAsLong);
            Console.WriteLine("TEST AsLong():\n'ID' Value parsed as Long successfully = '{0}', and the Value is '{1}'\n", success3, valAsLong);

            // Try and write a Value to the ini file.
            // If the Property doesn't exist, it will be added.
            bool exists = iniMgr.ContainsProperty("mick");
            if (!exists)
            {
                bool wasWritten = iniMgr.WriteEntry("users", "mick", "newlyAddedValue");
                Console.WriteLine("TEST WriteEntry():\nUnder 'users', 'mick' didn't exist, was written = '{0}'", wasWritten);
            }

            // Try and remove an Entry from the ini file.
            exists = iniMgr.ContainsProperty("mick");
            if (exists)
            {
                bool removed = iniMgr.RemoveEntry("users", "mick");
                Console.WriteLine("TEST RemoveEntry():\nProperty: 'mick' exists, and was removed = '{0}'", removed);
            }


            // END
            Console.ReadKey();
        }
    }
}
