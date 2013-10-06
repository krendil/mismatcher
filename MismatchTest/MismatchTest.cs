using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.Mismatcher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.IO.FastA;

namespace MismatchTest
{
    /// <summary>
    /// Summary description for MismatchTest
    /// </summary>
    [TestClass]
    public class MismatchTest
    {
        public MismatchTest()
        {
            //
            // TODO: Add constructor logic here
            //
            FastAParser Reference = new FastAParser(@"Reference.txt");
            Mismatch_Test = new Mismatcher(Reference.Parse().First());
            
        }

        Mismatcher Mismatch_Test { get; set; }
        
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1() //head and tail mismatch
        {
            
            
            FastAParser Query = new FastAParser(@"Query.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());
            Assert.AreEqual(2,Mismatches_query.Count());

            var first = Mismatches_query.First();
            Assert.AreEqual(0, first.QuerySequenceOffset);

            var last = Mismatches_query.Last();
            Assert.AreEqual(179, last.QuerySequenceOffset);
            
    
        }
        [TestMethod]
        public void TestMethod2() //test full match
        {

            FastAParser Query = new FastAParser(@"Query2.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());
            Assert.AreEqual(0,Mismatches_query.Count());

       }

        [TestMethod] 
        public void TestMethod3() //test mismatch in the middle, the 59th is different
        {

            FastAParser Query = new FastAParser(@"Query3.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());
            var first = Mismatches_query.First();
            Assert.AreEqual(59, first.QuerySequenceOffset);
        }

        [TestMethod]
        public void TestMethod4() //test completely mismatch
        {

            FastAParser Query = new FastAParser(@"Query4.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());
            Assert.AreEqual(1, Mismatches_query.Count());

        }

        [TestMethod]
        public void TestType_Insertion() //to test if it is an insertion
        {

            FastAParser Query = new FastAParser(@"Insertion.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());
            var first = Mismatches_query.First();
            Assert.AreEqual(MismatchType.Insertion, first.Type);
        }

        [TestMethod]
        public void TestType_Translocation() //to test if it is a translocation (translocate fisrt 5 sequence with second 5 sequence)
        {
            FastAParser Query = new FastAParser(@"Translocation.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());
            var first = Mismatches_query.First();
            Assert.AreEqual(MismatchType.Translocation, first.Type);
        }

    }
}
