using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InAsync.Tests {

    [TestClass]
    public class TestActualTests {

        [TestMethod]
        public void Verify_TResult() {
            foreach (var item in TestCases()) {
                var message = $"No.{item.testNumber}";

                var resultVerifierCalled = false;
                var resultVerifierWrapper = (item.resultVerifier == null) ? (TestActualVerifier<int>)null : (result, description) => {
                    resultVerifierCalled = true;
                    Assert.AreEqual(result, item.testActual.Result);
                    Assert.AreEqual(description, item.testActual.Description);
                    item.resultVerifier(result, description);
                };
                var exceptionVerifierCalled = false;
                var exceptionVerifierWrapper = (item.exceptionVerifier == null) ? (TestActualVerifier<Exception>)null : (exception, description) => {
                    exceptionVerifierCalled = true;
                    Assert.AreEqual(exception, item.testActual.Exception);
                    Assert.AreEqual(description, item.testActual.Description);
                    item.exceptionVerifier(exception, description);
                };

                try {
                    item.testActual.Verify(resultVerifierWrapper, exceptionVerifierWrapper);
                }
                catch (Exception ex) {
                    Assert.AreEqual(item.expectedExceptionType, ex.GetType(), message);
                    continue;
                }

                Assert.AreEqual(item.expectedResultVerifierCalled, resultVerifierCalled, message);
                Assert.AreEqual(item.expectedExceptionVerifierCalled, exceptionVerifierCalled, message);
            }

            // テストケース定義。
            IEnumerable<(int testNumber, ITestActual<int> testActual, TestActualVerifier<int> resultVerifier, TestActualVerifier<Exception> exceptionVerifier, bool expectedResultVerifierCalled, bool expectedExceptionVerifierCalled, Type expectedExceptionType)> TestCases() => new(int testNumber, ITestActual<int> testActual, TestActualVerifier<int> resultVerifier, TestActualVerifier<Exception> exceptionVerifier, bool expectedResultVerifierCalled, bool expectedExceptionVerifierCalled, Type expectedExceptionType)[]{
                ( 0, TestActual( 0, null)                   , null                      , (ex, description)=>{ }, false, false, typeof(ArgumentNullException)),
                ( 1, TestActual( 0, null)                   , (result, description)=>{ }, null                  , false, false, typeof(ArgumentNullException)),
                (10, TestActual( 0, null)                   , (result, description)=>{ }, (ex, description)=>{ }, true , true , null),
                (11, TestActual( 1, null)                   , (result, description)=>{ }, (ex, description)=>{ }, true , true , null),
                (12, TestActual(-1, null)                   , (result, description)=>{ }, (ex, description)=>{ }, true , true , null),
                (13, TestActual( 0, new Exception())        , (result, description)=>{ }, (ex, description)=>{ }, false, true , null),
                (14, TestActual( 0, new ArgumentException()), (result, description)=>{ }, (ex, description)=>{ }, false, true , null),
            };

            ITestActual<TResult> TestActual<TResult>(TResult result, Exception ex) => new TestCaseRunner("desc").Run(() => (ex == null) ? result : throw ex);
        }

        [TestMethod]
        public void Verify() {
            foreach (var item in TestCases()) {
                var message = $"No.{item.testNumber}";

                var resultVerifierCalled = false;
                var resultVerifierWrapper = (item.resultVerifier == null) ? (TestActualVerifier)null : (description) => {
                    resultVerifierCalled = true;
                    Assert.AreEqual(description, item.testActual.Description);
                    item.resultVerifier(description);
                };
                var exceptionVerifierCalled = false;
                var exceptionVerifierWrapper = (item.exceptionVerifier == null) ? (TestActualVerifier<Exception>)null : (exception, description) => {
                    exceptionVerifierCalled = true;
                    Assert.AreEqual(exception, item.testActual.Exception);
                    Assert.AreEqual(description, item.testActual.Description);
                    item.exceptionVerifier(exception, description);
                };

                try {
                    item.testActual.Verify(resultVerifierWrapper, exceptionVerifierWrapper);
                }
                catch (Exception ex) {
                    Assert.AreEqual(item.expectedExceptionType, ex.GetType(), message);
                    continue;
                }

                Assert.AreEqual(item.expectedResultVerifierCalled, resultVerifierCalled, message);
                Assert.AreEqual(item.expectedExceptionVerifierCalled, exceptionVerifierCalled, message);
            }

            // テストケース定義。
            IEnumerable<(int testNumber, ITestActual testActual, TestActualVerifier resultVerifier, TestActualVerifier<Exception> exceptionVerifier, bool expectedResultVerifierCalled, bool expectedExceptionVerifierCalled, Type expectedExceptionType)> TestCases() => new(int testNumber, ITestActual testActual, TestActualVerifier resultVerifier, TestActualVerifier<Exception> exceptionVerifier, bool expectedResultVerifierCalled, bool expectedExceptionVerifierCalled, Type expectedExceptionType)[]{
                ( 0, TestActual(null)                   , null              , (ex, description)=>{ }, false, false, typeof(ArgumentNullException)),
                ( 1, TestActual(null)                   , (description)=>{ }, null                  , false, false, typeof(ArgumentNullException)),
                (10, TestActual(null)                   , (description)=>{ }, (ex, description)=>{ }, true , true , null),
                (11, TestActual(new Exception())        , (description)=>{ }, (ex, description)=>{ }, false, true , null),
                (12, TestActual(new ArgumentException()), (description)=>{ }, (ex, description)=>{ }, false, true , null),
            };

            ITestActual TestActual(Exception ex) => new TestCaseRunner("desc").Run(() => { if (ex != null) throw ex; });
        }
    }
}