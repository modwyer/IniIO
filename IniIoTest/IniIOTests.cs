using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IniIO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace IniIOTest
{
    [TestClass]
    public class IniIOTests
    {
        [TestMethod]
        public void InitializationSuccessTest()
        {
            // Test that it read something in from the file.
            ConfigurationFileIO iniMgr = 
                new ConfigurationFileIO(Environment.CurrentDirectory + "/MyECFMail.ini");

            Assert.IsNotNull(iniMgr);

            Debug.WriteLine(iniMgr.toString());
            Assert.IsNotNull( iniMgr.toString() );
        }

        [TestMethod]
        [ExpectedException(typeof(System.IO.IOException))]
        public void InitFileWithNoSectionTest()
        {
            // Test that it fails for an ini file with a name/value pair
            // that is not under a section.
            ConfigurationFileIO iniMgr =
                new ConfigurationFileIO(Environment.CurrentDirectory + "/empty.ini");

            Assert.IsNotNull(iniMgr);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void InitializationBadFileTest()
        {
            // Test that it fails with a bad file path.
            try
            {
                ConfigurationFileIO iniMgr =
                    new ConfigurationFileIO(Environment.CurrentDirectory + "/bogus.ini");

                Assert.IsNotNull(iniMgr);
            }
            catch (System.ArgumentException ex)
            {
                Debug.WriteLine(ex.Message);
                Assert.AreEqual("Bad file or path.", ex.Message);
                throw new ArgumentException();
            }
        }

        [TestMethod]
        public void GetSectionNamesTest()
        {
            ConfigurationFileIO iniMgr =
               new ConfigurationFileIO(Environment.CurrentDirectory + "/MyECFMail.ini");

            Assert.IsNotNull(iniMgr);

            List<string> list = iniMgr.GetSectionNames().ToList();

            // The Section names are pulled from the ini file with the brackets as part
            // of the string.  They are removed when retrieved and present to the user.
            // Check that the first char of each Section name is NOT a open bracket.
            foreach (string s in list)
            {
                Debug.WriteLine(s);
                Assert.AreNotEqual('[', s[0]);
            }

            // The MyECFMail.ini file used in this test has eight sections.  
            // The count of the collection should be 8.  
            Assert.AreEqual(8, list.Count);
        }

        [TestMethod]
        public void ContainsSectionNameTest()
        {
            ConfigurationFileIO iniMgr =
              new ConfigurationFileIO(Environment.CurrentDirectory + "/MyECFMail.ini");

            Assert.IsNotNull(iniMgr);

            Assert.IsTrue(iniMgr.ContainsSection("debug"));
            Assert.IsTrue(iniMgr.ContainsSection("license"));
            Assert.IsTrue(iniMgr.ContainsSection("users"));
            Assert.IsTrue(iniMgr.ContainsSection("licErr"));
            Assert.IsTrue(iniMgr.ContainsSection("environment"));
            Assert.IsTrue(iniMgr.ContainsSection("SessionInfo"));
            Assert.IsTrue(iniMgr.ContainsSection("properties"));
            Assert.IsTrue(iniMgr.ContainsSection("CalendarTransfer"));

            //// Check if fails when it should
            Assert.IsFalse(iniMgr.ContainsSection("bogus"));
        }

        [TestMethod]
        public void ContainsPropertyTest()
        {
            ConfigurationFileIO iniMgr =
              new ConfigurationFileIO(Environment.CurrentDirectory + "/MyECFMail.ini");

            Assert.IsNotNull(iniMgr);

             //Check that some that should be true, are true.
            Assert.IsTrue(iniMgr.ContainsProperty("cloud"));
            Assert.IsTrue(iniMgr.ContainsProperty("LastSessionAborted"));

            // Check to see that bogus ones are false.
            Assert.IsFalse(iniMgr.ContainsProperty("bogus"));

            // Test the optional method that has out params for the sectionName and Value
            string sectionName = "";
            string value = "";
            iniMgr.ContainsProperty("cloud", out sectionName, out value);
            // [environment]
            // cloud=false
            Assert.AreEqual("environment", sectionName);
            Assert.AreEqual("false", value);

        }

        //[TestMethod]
        //[ExpectedException(typeof(System.ArgumentNullException))]
        //public void FileModifierCtorArgumentNullExceptionTest()
        //{
        //    try
        //    {
        //        string badpath = null;
        //        FileModifier fm = new FileModifier(badpath);
        //    }
        //    catch (ArgumentNullException)
        //    {
        //        throw new ArgumentNullException();
        //    }
        //}

        //[TestMethod]
        //[ExpectedException(typeof(System.ArgumentException))]
        //public void FileModifierCtorArgumentExceptionTest()
        //{
        //    try
        //    {
        //        string badpath = null;
        //        FileModifier fm = new FileModifier(badpath);
        //    }
        //    catch (ArgumentException)
        //    {
        //        throw new ArgumentException();
        //    }
        //}

        //[TestMethod]
        //[ExpectedException(typeof(System.IO.PathTooLongException))]
        //public void FileModifierCtorPathTooLongExceptionTest()
        //{
        //    try
        //    {
        //        string goodpath = Environment.CurrentDirectory + "/MyECFMail.ini";
        //        //Debug.WriteLine("goodpath.Length: " + goodpath.Length);
        //        char[] path = goodpath.ToCharArray();
        //        //Debug.WriteLine("char count: " + path.Count().ToString());
        //        FileModifier fm = new FileModifier(goodpath);
        //    }
        //    catch(System.IO.PathTooLongException)
        //    {
        //        // Nothing here which would cause this test to fail it 
        //        // this exception should have been thrown.
        //    }

        //    try
        //    {
        //        string badpath260 = "ABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJ";
        //        //Debug.WriteLine("badpath.Length: " + badpath260.Length);
        //        char[] path = badpath260.ToCharArray();
        //        //Debug.WriteLine("char count: " + path.Count().ToString());
        //        FileModifier fm = new FileModifier(badpath260);
        //    }
        //    catch (System.IO.PathTooLongException)
        //    {
        //        throw new System.IO.PathTooLongException();
        //    }
        //}

        [TestMethod]
        public void FileModifierTest()
        {
            // Test that the filename is "~NA~" if a bad path is passed.
            FileModifier fmbad = new FileModifier("bogusfile.ini");
            Assert.AreEqual("~NA~", fmbad.Filename);

            string path = Environment.CurrentDirectory + "/MyECFMail.ini";
            FileModifier fm = new FileModifier(path);            

            Assert.IsNotNull(fm);

            // Check that the path was set correctly
            Assert.AreEqual(path, fm.Filename);

            // Test that bool property IsCreated is true after successfully creation.
            Assert.IsTrue(fm.IsCreated);

            // Assert that you can just reset the path.  It must be set in the ctor privately.
            fm.Filename = "bogus.ini";
            Assert.AreNotEqual("bogus.ini", fm.Filename);
            // Filename should still be as it was in ctor.
            Assert.AreEqual(path, fm.Filename);

            // Get the LastAccessTime and LastAccessTimeUtc from the private FileIOManager object.
            DateTime lat = fm.GetLastWriteTime();
            Assert.IsNotNull(lat);

            DateTime latUtc = fm.GetLastWriteTimeUtc();
            Assert.IsNotNull(latUtc);

            //Debug.WriteLine("lat: " + lat + "\nlatUtc: " + latUtc.ToLocalTime());
        }

        [TestMethod]
        public void FileIOManagerQueueTest()
        {
            IniCreator ic = 
                new IniCreator(Environment.CurrentDirectory + "/MyECFMail.ini");

            string filepath = Environment.CurrentDirectory + "/MyECFMail.ini";
            string filepath2 = Environment.CurrentDirectory + "/Memmy.ini";
            var e = new Entry("users", "tester1", "tester1value");
            var ee = new Entry("users", "tester2", "tester2value");
            var eee = new Entry("section1", "property2", "value2");
            var eeee = new Entry("section2", "propertyB", "valueB_2");

            bool success = ic.SaveEntry("users", "tester1");
            Assert.IsTrue(success);

            success = ic.SaveEntry("users", "tester1", "tester1value");
            Assert.IsTrue(success);

            //******you ended here testing the multi entry stuff.
            // need a good test dictionary of values.  maybe make
            // separate method that creates it.

            //var entries = new Dictionary<string,Dictionary<string,string>>();
            //Dictionary<string,string> vals = new Dictionary<string,string>();
            //vals.Add("e1", "e1val");
            //vals.Add("e2", "e2val");
            //vals.Add("e3", "e3val");
            //entries.Add("users", vals);
            //success = ic.SaveEntries(entries as IDictionary<string,IDictionary<string,string>>);

            //int loc = "modwyer=XtOnLPcds4G2B9FQx11l+g==".IndexOf('=');
            //var list = FileContents.splitString(loc, "modwyer=XtOnLPcds4G2B9FQx11l+g==");
            //Assert.AreEqual("modwyer", list[0]);
            //Assert.AreEqual("XtOnLPcds4G2B9FQx11l+g==", list[1]);

            
            //success = ic.SaveEntry();

            //bool success;
            //success = FileIOManager.AddEntryToList(e);
            //Assert.IsTrue(success);
            //success = FileIOManager.AddEntryToList(ee);
            //Assert.IsTrue(success);
            //success = FileIOManager.AddEntryToList(eee);
            //Assert.IsTrue(success);
            //success = FileIOManager.AddEntryToList(eeee);
            //Assert.IsTrue(success);

            FileIOManager.FlushEntries();

        }

        [TestMethod]
        public void WriteEntryToListTest()
        {
            ConfigurationFileIO iniMgr =
              new ConfigurationFileIO(Environment.CurrentDirectory + "/Memmy.ini");
            
            Assert.IsNotNull(iniMgr);

            Assert.IsTrue(iniMgr.WriteEntry("section1", "unitTest", "testValue"));
        }

        [TestMethod]
        public void RemoveEntryFromListTest()
        {
            FileModifier fm = new FileModifier(Environment.CurrentDirectory + "/Memmy.ini");
            Entry e = new Entry("sectionName", "property", "value");
            
            Assert.IsNotNull(e);

            bool success = fm.StoreEntryForWrite(e);
            Assert.IsTrue(success);

            bool contains = fm.ContainsStoredEntry(e);
            Assert.IsTrue(contains);

            bool removed = fm.RemoveStoredEntry(e);
            Assert.IsTrue(removed);

            bool contains2 = fm.ContainsStoredEntry(e);
            Assert.IsFalse(contains2);

        }

        [TestMethod]
        public void EntryClassTest()
        {
            // Test the default ctor
            Entry e = new Entry();
            Assert.IsNotNull(e);

            // Test the two param ctor
            Entry ee = new Entry("sectionName", "property");
            Assert.IsNotNull(ee);
            Assert.AreEqual("sectionName", ee.Section);
            Assert.AreEqual("property", ee.Property);
            Assert.IsNull(ee.Value);

            // Test the three param ctor
            Entry eee = new Entry("sectionName", "property", "value");
            Assert.IsNotNull(eee);
            Assert.AreEqual("sectionName", eee.Section);
            Assert.AreEqual("property", eee.Property);
            Assert.AreEqual("value", eee.Value);

            // Test that a Value can be returned as an Integer
            Entry intVal = new Entry("section", "property", "1");
            Assert.IsNotNull(intVal);
            int value = 0;
            bool success = intVal.AsInteger(out value);            
            Assert.AreEqual(1, value);
            Assert.IsTrue(success);

            // Test that a string value will fail to return as an Integer
            int badValue = 999;
            bool failure = eee.AsInteger(out badValue);
            Assert.IsFalse(failure);
            // The out param gets set to 0 on failing.
            Assert.AreEqual(0, badValue);

            // Test that a Value can be returned as a Boolean.
            Entry eBool = new Entry("section", "property", "true");
            Assert.IsNotNull(eBool);
            bool val;
            bool bSuccess = eBool.AsBoolean(out val);
            Assert.IsTrue(bSuccess);
            Assert.AreEqual(true, val);

            Entry fBool = new Entry("section", "property", "False");
            Assert.IsNotNull(fBool);
            bool val2;
            bool fSuccess = fBool.AsBoolean(out val2);
            Assert.IsTrue(fSuccess);
            Assert.AreEqual(false, val2);

            // Test that a string will fail to return as a Boolean
            bool badVal;
            bool boolFailure = eee.AsBoolean(out badVal);
            Assert.IsFalse(boolFailure);
            // The out param gets set to false on failing.
            Assert.AreEqual(false, badVal);
        }

        [TestMethod]
        public void AsBooleanTest()
        {
            ConfigurationFileIO iniMgr =
              new ConfigurationFileIO(Environment.CurrentDirectory + "/MyECFMail.ini");

            Assert.IsNotNull(iniMgr);

            // Check that cloud Property is parsed successfully and returns a Value of false.
            bool value;
            bool success = iniMgr.ReadValueAsBoolean("environment", "cloud", out value);
            Assert.IsTrue(success);
            Assert.IsFalse(value);

            // Check that Exp Property, which is a Date, is false.
            bool value2;
            bool success2 = iniMgr.ReadValueAsBoolean("license", "Exp", out value2);
            Assert.IsFalse(success2);
            Assert.IsFalse(value2);
        }

        [TestMethod]
        public void AsIntegerTest()
        {
            ConfigurationFileIO iniMgr =
                new ConfigurationFileIO(Environment.CurrentDirectory + "/MyECFMail.ini");

            Assert.IsNotNull(iniMgr);

            int value;
            bool success = iniMgr.ReadValueAsInteger("license", "LID", out value);
            Assert.IsTrue(success);
            Assert.AreEqual(3000615, value);

            // Check that the ID Property is NOT parsed successfully.  It is too big.
            int value2;
            bool success2 = iniMgr.ReadValueAsInteger("license", "ID", out value2);
            Assert.IsFalse(success2);
            Assert.AreNotEqual(129526429308092496, value2);
        }

        [TestMethod]
        public void AsLongTest()
        {
            ConfigurationFileIO iniMgr =
                new ConfigurationFileIO(Environment.CurrentDirectory + "/MyECFMail.ini");

            Assert.IsNotNull(iniMgr);

            long value;
            bool success = iniMgr.ReadValueAsLong("license", "ID", out value);
            Assert.IsTrue(success);
            Assert.AreEqual(129526429308092496, value);

            // Fails on a string
            long value2;
            bool success2 = iniMgr.ReadValueAsLong("environment", "cloud", out value2);
            Assert.IsFalse(success2);
            Assert.AreNotEqual(129526429308092496, value2);
        }

        [TestMethod]
        public void RemoveEntryFromIniTest()
        {
            ConfigurationFileIO iniMgr =
                new ConfigurationFileIO(Environment.CurrentDirectory + "/MyECFMail.ini");

            Assert.IsNotNull(iniMgr);

            bool success = iniMgr.RemoveEntry("users", "tester2");
            Assert.IsTrue(success);

            // Check that the Property no longer exists.
            //Assert.IsFalse(iniMgr.ContainsProperty("tester2"));
        }

        [TestMethod]
        public void GetContentsAsDictionaryTest()
        {
            ConfigurationFileIO iniMgr = 
                new ConfigurationFileIO(Environment.CurrentDirectory + "/MyECFMail.ini");

            Assert.IsNotNull(iniMgr);


            var d = iniMgr.toDictionary();
            // Count of Sections.
            Assert.AreEqual(8, d.Count);
            // Count of the number of Properties under licErr Section.
            Assert.AreEqual(4, d["licErr"].Values.Count);
            

            // Debug
            //foreach (KeyValuePair<string, Dictionary<string, string>> kvp in d)
            //{
            //    Debug.WriteLine("\n[" + kvp.Key + "]");
            //    foreach (KeyValuePair<string, string> kvp2 in kvp.Value)
            //    {
            //        Debug.WriteLine(kvp2.Key + "=" + kvp2.Value);
            //    }
            //}
            // Debug

        }
    }
}
