using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inasync.Tests {

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
            (int testNumber, ITestActual testActual, TestActualVerifier resultVerifier, Type exceptionType, bool expectedResultVerifierCalled, Type expectedExceptionType)[] TestCases() => new[]{
                ( 0, TestActual(null)                   , null            , null                     , false, (Type)typeof(ArgumentNullException)),
                (10, TestActual(null)                   , ResultVerifier(), null                     , true , (Type)null),
                (11, TestActual(null)                   , ResultVerifier(), typeof(Exception)        , false, (Type)typeof(AssertFailedException)),
                (12, TestActual(new Exception())        , ResultVerifier(), null                     , false, (Type)typeof(AssertFailedException)),
                (13, TestActual(new Exception())        , ResultVerifier(), typeof(Exception)        , false, (Type)null),
                (14, TestActual(new Exception())        , ResultVerifier(), typeof(ArgumentException), false, (Type)typeof(AssertFailedException)),
                (15, TestActual(new ArgumentException()), ResultVerifier(), typeof(ArgumentException), false, (Type)null),
            };

            ITestActual TestActual(Exception ex) => new TestCaseRunner("desc").Run(() => { if (ex != null) throw ex; });
            TestActualVerifier ResultVerifier() => (description) => { };
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
            (int testNumber, ITestActual<int> testActual, TestActualVerifier<int> resultVerifier, Type exceptionType, bool expectedResultVerifierCalled, Type expectedExceptionType)[] TestCases() => new[]{
                ( 0, TestActual( 0, null)                   , null            , null                     , false, (Type)typeof(ArgumentNullException)),
                (10, TestActual( 0, null)                   , ResultVerifier(), null                     , true , (Type)null),
                (11, TestActual( 1, null)                   , ResultVerifier(), null                     , true , (Type)null),
                (12, TestActual(-1, null)                   , ResultVerifier(), null                     , true , (Type)null),
                (13, TestActual( 0, null)                   , ResultVerifier(), typeof(Exception)        , false, (Type)typeof(AssertFailedException)),
                (14, TestActual( 0, new Exception())        , ResultVerifier(), null                     , false, (Type)typeof(AssertFailedException)),
                (15, TestActual( 0, new Exception())        , ResultVerifier(), typeof(Exception)        , false, (Type)null),
                (16, TestActual( 0, new Exception())        , ResultVerifier(), typeof(ArgumentException), false, (Type)typeof(AssertFailedException)),
                (17, TestActual( 0, new ArgumentException()), ResultVerifier(), typeof(ArgumentException), false, (Type)null),
            };

            ITestActual<TResult> TestActual<TResult>(TResult result, Exception ex) => new TestCaseRunner("desc").Run(() => (ex == null) ? result : throw ex);
            TestActualVerifier<int> ResultVerifier() => (result, description) => { };
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
            IEnumerable<(int testNumber, ITestActual<int> testActual, int result, Type exceptionType, Type expectedExceptionType)> TestCases() => new[]{
                (10, TestActual( 0, null)                   , 0, null                     , (Type)null),
                (11, TestActual( 1, null)                   , 0, null                     , (Type)typeof(AssertFailedException)),
                (12, TestActual( 0, null)                   , 1, null                     , (Type)typeof(AssertFailedException)),
                (13, TestActual( 1, null)                   , 1, null                     , (Type)null),
                (14, TestActual( 0, null)                   , 0, typeof(Exception)        , (Type)typeof(AssertFailedException)),
                (15, TestActual( 0, new Exception())        , 0, null                     , (Type)typeof(AssertFailedException)),
                (16, TestActual( 0, new Exception())        , 0, typeof(Exception)        , (Type)null),
                (17, TestActual( 0, new Exception())        , 0, typeof(ArgumentException), (Type)typeof(AssertFailedException)),
                (18, TestActual( 0, new ArgumentException()), 0, typeof(ArgumentException), (Type)null),
            };

            ITestActual<TResult> TestActual<TResult>(TResult result, Exception ex) => new TestCaseRunner("desc").Run(() => (ex == null) ? result : throw ex);
        }
    }
}