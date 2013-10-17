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
        public void TestType_Insertion() //to test if it is an insertion (in the middle)
        {

            FastAParser Query = new FastAParser(@"Insertion.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());
            var first = Mismatches_query.First();
            Assert.AreEqual(MismatchType.Insertion, first.Type);
        }

        [TestMethod]
        public void TestType_Insertion_a() //to test if it is an insertion (in the beginning)
        {

            FastAParser Query = new FastAParser(@"Insertion_a.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());
            var first = Mismatches_query.First();
            Assert.AreEqual(MismatchType.Insertion, first.Type);
        }

        [TestMethod]
        public void TestType_Insertion_b() //to test if it is an insertion (in the end)
        {

            FastAParser Query = new FastAParser(@"Insertion_b.txt");
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

        [TestMethod] 
        public void TestType_Translocation_moreGap() //to test if it is a translocation (more gap)
        {
            FastAParser Query = new FastAParser(@"Translocation_gap.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());
            var first = Mismatches_query.First();
            Assert.AreEqual(MismatchType.Translocation, first.Type);
        }           

        [TestMethod]
        public void TestType_Deletion() //to test if it is a deletion(delete first 5)
        {
            FastAParser Query = new FastAParser(@"Deletion.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());
            var first = Mismatches_query.First();
            Assert.AreEqual(MismatchType.Deletion, first.Type);
        }

        [TestMethod]
        public void TestType_Deletion_a() //to test if it is a deletion(delete first line 5-10) 
                                          //Reason:the system cannot verify the sequence under 20 characters
        {
            FastAParser Query = new FastAParser(@"Deletion_a.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());
            var first = Mismatches_query.First();
            Assert.AreEqual(MismatchType.Deletion, first.Type);
        }

        [TestMethod]
        public void TestType_Deletion_b() //to test if it is a deletion(delete second line, first 5)
        {
            FastAParser Query = new FastAParser(@"Deletion_b.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());
            var first = Mismatches_query.First();
            Assert.AreEqual(MismatchType.Deletion, first.Type);
        }

        [TestMethod]
        public void TestType_Deletion_c() //to test if it is a deletion(delete second line, first 5)
        {
            FastAParser Query = new FastAParser(@"Deletion_c.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());
            var first = Mismatches_query.First();
            Assert.AreEqual(MismatchType.Deletion, first.Type);
        }

        [TestMethod]
        public void TestType_Inversion() //to test if it is an inversion (inversion for the first 5 sequences)
        {
            FastAParser Query = new FastAParser(@"Inversion.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());
            var first = Mismatches_query.First();
            Assert.AreEqual(MismatchType.Inversion, first.Type);
        }

        [TestMethod]
        public void TestType_two1() //to test two types together. deletion first line, last 5. insertion next,second line, last5)
        {
            FastAParser Query = new FastAParser(@"two_del_ins.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Deletion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Insertion, Mismatches_query[1].Type);
 
            /* 
               foreach (Mismatch mismatch in Mismatches_query)
                {
                   Assert.AreEqual(MismatchType.Deletion, mismatch.Type);
                }
            */
        }

        [TestMethod]
        public void TestType_two1a() //to test two types together. delete first line (first 5), insertion next(first line, last 5))
        {
            FastAParser Query = new FastAParser(@"two_del_ins_a.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Deletion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Insertion, Mismatches_query[1].Type);

        }

        [TestMethod]
        public void TestType_two1c() //to test two types together. delete first line (middle 5), insertion next(middle 5))
        {
            FastAParser Query = new FastAParser(@"two_del_ins_c.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Deletion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Insertion, Mismatches_query[1].Type);

        }

        [TestMethod]
        public void TestType_two1b() //to test two types together. deletion first line (position5-10), insertion next(second line, last 5))
                                     //Reason: system cannot verify sequence under 20 characters
        {
            FastAParser Query = new FastAParser(@"two_del_ins_b.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Deletion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Insertion, Mismatches_query[1].Type);

        }

        [TestMethod]
        public void TestType_two2() //to test two types together. deletion first, inversion next(second line))
        {
            FastAParser Query = new FastAParser(@"two_del_inv.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Deletion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Inversion, Mismatches_query[1].Type);

        }

        [TestMethod]
        public void TestType_two3() //to test two types together. insertion first (beginning), deletion next(end of first line))
        {
            FastAParser Query = new FastAParser(@"two_ins_del.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Insertion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Deletion, Mismatches_query[1].Type);

        }

        [TestMethod]
        public void TestType_two4() //to test two types together.inversion first(start of first line),deletion next (end of third line)
        {
            FastAParser Query = new FastAParser(@"two_inv_del.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Inversion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Deletion, Mismatches_query[1].Type);

        }

        [TestMethod]
        public void TestType_two5() //to test two types together.deletion(beginning 5) and translocation (third line, translocate first 5 with last 5)
        {
            FastAParser Query = new FastAParser(@"two_del_trans.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Deletion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Translocation, Mismatches_query[1].Type);

        }

        [TestMethod]
        public void TestType_two6() //to test two types together.2 deletion (beginning of first line and third line)
        {
            FastAParser Query = new FastAParser(@"two_del_del.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Deletion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Deletion, Mismatches_query[1].Type);

        }

        [TestMethod]
        public void TestType_two7() //to test two types together.insertion(beginning 5) and inverstion(end of last line, 5)
        {
            FastAParser Query = new FastAParser(@"two_ins_inv.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Insertion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Inversion, Mismatches_query[1].Type);

        }

        [TestMethod]
        public void TestType_two8() //to test two types together.insertion(beginning 5) and translocation (third line)
        {
            FastAParser Query = new FastAParser(@"two_ins_trans.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Insertion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Translocation, Mismatches_query[1].Type);

        }

        [TestMethod]
        public void TestType_two9() //to test two types together.insertion (first and third line)
        {
            FastAParser Query = new FastAParser(@"two_ins_ins.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Insertion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Insertion, Mismatches_query[1].Type);

        }

        [TestMethod]
        public void TestType_two10() //to test two types together.inversion (first 5) and insertion (first 5 of third line)
        {
            FastAParser Query = new FastAParser(@"two_inv_ins.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Inversion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Insertion, Mismatches_query[1].Type);

        }

        [TestMethod]
        public void TestType_two11() //to test two types together.inversion (first 5) and translocation (first and last 5 in third line)
        {
            FastAParser Query = new FastAParser(@"two_inv_trans.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Inversion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Translocation, Mismatches_query[1].Type);

        }

        [TestMethod]    //has problem!!
        public void TestType_two11a() //to test two types together.inversion (first 5) and translocation (first and last 5 in second line)
        {
            FastAParser Query = new FastAParser(@"two_inv_trans_a.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Inversion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Translocation, Mismatches_query[1].Type);

        }

        [TestMethod]
        public void TestType_two11c() //to test two types together.inversion (middle 5) and translocation (first and last 5 in second line)
        {
            FastAParser Query = new FastAParser(@"two_inv_trans_c.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Inversion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Translocation, Mismatches_query[1].Type);

        }


        [TestMethod]
        public void TestType_two11b() //to test two types together.inversion (first 5) and translocation (last 10(half half) in first line)
        {
            FastAParser Query = new FastAParser(@"two_inv_trans_b.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Inversion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Translocation, Mismatches_query[1].Type);

        }

        [TestMethod]
        public void TestType_two12() //to test two types together. 2 inversion (first 5 in first and third line) 
        {
            FastAParser Query = new FastAParser(@"two_inv_inv.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Inversion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Inversion, Mismatches_query[1].Type);

        }

        [TestMethod]
        public void TestType_two12a() //to test two types together. 2 inversion (first and last 5 in first line) 
        {
            FastAParser Query = new FastAParser(@"two_inv_inv_a.txt");
            var Mismatches_query = Mismatch_Test.GetMismatches(Query.Parse().First());

            Assert.AreEqual(MismatchType.Inversion, Mismatches_query[0].Type);

            Assert.AreEqual(MismatchType.Inversion, Mismatches_query[1].Type);

        }
    }
}
