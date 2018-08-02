using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InAsync.Tests {

    [TestClass]
    public class TestActualExtensionsTests {

        [TestMethod]
        public void Verify_TestActualVerifier_Type() {
            foreach (var item in TestCases()) {
                var message = $"No.{item.testNumber}";

                var resultVerifierCalled = false;
                var resultVerifierWrapper = (item.resultVerifier == null) ? (TestActualVerifier)null : (description) => {
                    resultVerifierCalled = true;
                    Assert.AreEqual(description, item.testActual.Description);
                    item.resultVerifier(description);
                };

                try {
                    item.testActual.Verify(resultVerifierWrapper, item.exceptionType);
                }
                catch (Exception ex) {
                    Assert.AreEqual(item.expectedExceptionType, ex.GetType(), message);
                    continue;
                }

                Assert.AreEqual(item.expectedResultVerifierCalled, resultVerifierCalled, message);
            }

            // テストケース定義。
            IEnumerable<(int testNumber, ITestActual testActual, TestActualVerifier resultVerifier, Type exceptionType, bool expectedResultVerifierCalled, Type expectedExceptionType)> TestCases() => new(int testNumber, ITestActual testActual, TestActualVerifier resultVerifier, Type exceptionType, bool expectedResultVerifierCalled, Type expectedExceptionType)[]{
                ( 0, TestActual(null)                   , null              , null                     , false, typeof(ArgumentNullException)),
                (10, TestActual(null)                   , (description)=>{ }, null                     , true , null),
                (11, TestActual(null)                   , (description)=>{ }, typeof(Exception)        , false, typeof(AssertFailedException)),
                (12, TestActual(new Exception())        , (description)=>{ }, null                     , false, typeof(AssertFailedException)),
                (13, TestActual(new Exception())        , (description)=>{ }, typeof(Exception)        , false, null),
                (14, TestActual(new Exception())        , (description)=>{ }, typeof(ArgumentException), false, typeof(AssertFailedException)),
                (15, TestActual(new ArgumentException()), (description)=>{ }, typeof(ArgumentException), false, null),
            };

            ITestActual TestActual(Exception ex) => new TestCaseRunner("desc").Run(() => { if (ex != null) throw ex; });
        }

        [TestMethod]
        public void Verify_TResult_TestActualVerifier_Type() {
            foreach (var item in TestCases()) {
                var message = $"No.{item.testNumber}";

                var resultVerifierCalled = false;
                var resultVerifierWrapper = (item.resultVerifier == null) ? (TestActualVerifier<int>)null : (result, description) => {
                    resultVerifierCalled = true;
                    Assert.AreEqual(result, item.testActual.Result);
                    Assert.AreEqual(description, item.testActual.Description);
                    item.resultVerifier(result, description);
                };

                try {
                    item.testActual.Verify(resultVerifierWrapper, item.exceptionType);
                }
                catch (Exception ex) {
                    Assert.AreEqual(item.expectedExceptionType, ex.GetType(), message);
                    continue;
                }

                Assert.AreEqual(item.expectedResultVerifierCalled, resultVerifierCalled, message);
            }

            // テストケース定義。
            IEnumerable<(int testNumber, ITestActual<int> testActual, TestActualVerifier<int> resultVerifier, Type exceptionType, bool expectedResultVerifierCalled, Type expectedExceptionType)> TestCases() => new(int testNumber, ITestActual<int> testActual, TestActualVerifier<int> resultVerifier, Type exceptionType, bool expectedResultVerifierCalled, Type expectedExceptionType)[]{
                ( 0, TestActual( 0, null)                   , null                      , null                     , false, typeof(ArgumentNullException)),
                (10, TestActual( 0, null)                   , (result, description)=>{ }, null                     , true , null),
                (11, TestActual( 1, null)                   , (result, description)=>{ }, null                     , true , null),
                (12, TestActual(-1, null)                   , (result, description)=>{ }, null                     , true , null),
                (13, TestActual( 0, null)                   , (result, description)=>{ }, typeof(Exception)        , false, typeof(AssertFailedException)),
                (14, TestActual( 0, new Exception())        , (result, description)=>{ }, null                     , false, typeof(AssertFailedException)),
                (15, TestActual( 0, new Exception())        , (result, description)=>{ }, typeof(Exception)        , false, null),
                (16, TestActual( 0, new Exception())        , (result, description)=>{ }, typeof(ArgumentException), false, typeof(AssertFailedException)),
                (17, TestActual( 0, new ArgumentException()), (result, description)=>{ }, typeof(ArgumentException), false, null),
            };

            ITestActual<TResult> TestActual<TResult>(TResult result, Exception ex) => new TestCaseRunner("desc").Run(() => (ex == null) ? result : throw ex);
        }

        [TestMethod]
        public void Verify_TResult_TResult_Type() {
            foreach (var item in TestCases()) {
                var message = $"No.{item.testNumber}";

                try {
                    item.testActual.Verify(item.result, item.exceptionType);
                }
                catch (Exception ex) {
                    Assert.AreEqual(item.expectedExceptionType, ex.GetType(), message);
                    continue;
                }
            }

            // テストケース定義。
            IEnumerable<(int testNumber, ITestActual<int> testActual, int result, Type exceptionType, Type expectedExceptionType)> TestCases() => new(int testNumber, ITestActual<int> testActual, int result, Type exceptionType, Type expectedExceptionType)[]{
                (10, TestActual( 0, null)                   , 0, null                     , null),
                (11, TestActual( 1, null)                   , 0, null                     , typeof(AssertFailedException)),
                (12, TestActual( 0, null)                   , 1, null                     , typeof(AssertFailedException)),
                (13, TestActual( 1, null)                   , 1, null                     , null),
                (14, TestActual( 0, null)                   , 0, typeof(Exception)        , typeof(AssertFailedException)),
                (15, TestActual( 0, new Exception())        , 0, null                     , typeof(AssertFailedException)),
                (16, TestActual( 0, new Exception())        , 0, typeof(Exception)        , null),
                (17, TestActual( 0, new Exception())        , 0, typeof(ArgumentException), typeof(AssertFailedException)),
                (18, TestActual( 0, new ArgumentException()), 0, typeof(ArgumentException), null),
            };

            ITestActual<TResult> TestActual<TResult>(TResult result, Exception ex) => new TestCaseRunner("desc").Run(() => (ex == null) ? result : throw ex);
        }
    }
}