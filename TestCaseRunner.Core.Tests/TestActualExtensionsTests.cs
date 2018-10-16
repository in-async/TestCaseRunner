using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inasync.Tests {

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
            (int testNumber, ITestActual<int> testActual, TestActualVerifier<int> resultVerifier, TestActualVerifier<Exception> exceptionVerifier, bool expectedResultVerifierCalled, bool expectedExceptionVerifierCalled, Type expectedExceptionType)[] TestCases() => new[]{
                ( 0, TestActual( 0, null)                   , null            , ExceptionVerifier(), false, false, typeof(ArgumentNullException)),
                ( 1, TestActual( 0, null)                   , ResultVerifier(), null               , false, false, typeof(ArgumentNullException)),
                (10, TestActual( 0, null)                   , ResultVerifier(), ExceptionVerifier(), true , true , (Type)null),
                (11, TestActual( 1, null)                   , ResultVerifier(), ExceptionVerifier(), true , true , (Type)null),
                (12, TestActual(-1, null)                   , ResultVerifier(), ExceptionVerifier(), true , true , (Type)null),
                (13, TestActual( 0, new Exception())        , ResultVerifier(), ExceptionVerifier(), false, true , (Type)null),
                (14, TestActual( 0, new ArgumentException()), ResultVerifier(), ExceptionVerifier(), false, true , (Type)null),
            };

            ITestActual<TResult> TestActual<TResult>(TResult result, Exception ex) => new TestCaseRunner("desc").Run(() => (ex == null) ? result : throw ex);
            TestActualVerifier<int> ResultVerifier() => (result, description) => { };
            TestActualVerifier<Exception> ExceptionVerifier() => (exception, description) => { };
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
            (int testNumber, ITestActual testActual, TestActualVerifier resultVerifier, TestActualVerifier<Exception> exceptionVerifier, bool expectedResultVerifierCalled, bool expectedExceptionVerifierCalled, Type expectedExceptionType)[] TestCases() => new[]{
                ( 0, TestActual(null)                   , null            , ExceptionVerifier(), false, false, typeof(ArgumentNullException)),
                ( 1, TestActual(null)                   , ResultVerifier(), null               , false, false, typeof(ArgumentNullException)),
                (10, TestActual(null)                   , ResultVerifier(), ExceptionVerifier(), true , true , (Type)null),
                (11, TestActual(new Exception())        , ResultVerifier(), ExceptionVerifier(), false, true , (Type)null),
                (12, TestActual(new ArgumentException()), ResultVerifier(), ExceptionVerifier(), false, true , (Type)null),
            };

            ITestActual TestActual(Exception ex) => new TestCaseRunner("desc").Run(() => { if (ex != null) throw ex; });
            TestActualVerifier ResultVerifier() => (description) => { };
            TestActualVerifier<Exception> ExceptionVerifier() => (exception, description) => { };
        }
    }
}