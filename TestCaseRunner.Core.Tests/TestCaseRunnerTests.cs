using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inasync.Tests {

    [TestClass]
    public class TestCaseRunnerTests {

        [TestMethod]
        public void TestCaseRunner() {
            foreach (var item in TestCases()) {
                var message = $"No.{item.testNumber}";

                TestCaseRunner actual;
                try {
                    actual = new TestCaseRunner(item.description);
                }
                catch (Exception ex) {
                    Assert.AreEqual(item.expectedExceptionType, ex.GetType(), message);
                    continue;
                }

                Assert.AreEqual(item.description, actual.Description);
                Assert.AreEqual(item.expectedExceptionType, null, message);
            }

            // テストケース定義。
            (int testNumber, string description, Type expectedExceptionType)[] TestCases() => new[]{
                (10, null , (Type)null),
                (11, ""   , (Type)null),
                (12, "  " , (Type)null),
                (13, "foo", (Type)null),
            };
        }

        [TestMethod]
        public void Run_TResult() {
            foreach (var item in TestCases()) {
                var message = $"No.{item.testNumber}";

                var runner = new TestCaseRunner("desc");
                ITestActual<int> actual;
                try {
                    actual = runner.Run(item.targetCode);
                }
                catch (Exception ex) {
                    Assert.AreEqual(item.expectedExceptionType, ex.GetType(), message);
                    continue;
                }

                Assert.AreEqual(item.expected.Description, actual.Description, message);
                Assert.AreEqual(item.expected.Exception?.GetType(), actual.Exception?.GetType(), message);
                Assert.AreEqual(item.expected.Result, actual.Result, message);
            }

            // テストケース定義。
            (int testNumber, Func<int> targetCode, ITestActual<int> expected, Type expectedExceptionType)[] TestCases() => new[]{
                ( 0, null                                     , null                                   , typeof(ArgumentNullException)),
                (10, Code(() =>  0)                           , TestActual( 0, null)                   , (Type)null),
                (11, Code(() =>  1)                           , TestActual( 1, null)                   , (Type)null),
                (12, Code(() => -1)                           , TestActual(-1, null)                   , (Type)null),
                (13, Code(() => throw new Exception())        , TestActual( 0, new Exception())        , (Type)null),
                (14, Code(() => throw new ArgumentException()), TestActual( 0, new ArgumentException()), (Type)null),
            };

            ITestActual<TResult> TestActual<TResult>(TResult result, Exception ex) => new StubITestActual<TResult>("desc") { Result = result, Exception = ex };
            Func<int> Code(Func<int> targetCode) => targetCode;
        }

        [TestMethod]
        public void Run() {
            foreach (var item in TestCases()) {
                var message = $"No.{item.testNumber}";

                var runner = new TestCaseRunner("desc");
                ITestActual actual;
                try {
                    actual = runner.Run(item.targetCode);
                }
                catch (Exception ex) {
                    Assert.AreEqual(item.expectedExceptionType, ex.GetType(), message);
                    continue;
                }

                Assert.AreEqual(item.expected.Description, actual.Description, message);
                Assert.AreEqual(item.expected.Exception?.GetType(), actual.Exception?.GetType(), message);
            }

            // テストケース定義。
            (int testNumber, Action targetCode, ITestActual expected, Type expectedExceptionType)[] TestCases() => new[]{
                ( 0, null                                     , null                               , typeof(ArgumentNullException)),
                (10, Code(() => { })                          , TestActual(null)                   , (Type)null),
                (11, Code(() => throw new Exception())        , TestActual(new Exception())        , (Type)null),
                (12, Code(() => throw new ArgumentException()), TestActual(new ArgumentException()), (Type)null),
            };

            ITestActual TestActual(Exception ex) => new StubITestActual("desc") { Exception = ex };
            Action Code(Action targetCode) => targetCode;
        }

        #region Helpers

        private class StubITestActual<TResult> : ITestActual<TResult> {

            public StubITestActual(string description) {
                Description = description;
            }

            public string Description { get; }

            public Exception Exception { get; set; }

            public TResult Result { get; set; }

            public void Verify(TestActualVerifier<TResult> resultVerifier, TestActualVerifier<Exception> exceptionVerifier) => throw new NotImplementedException();
        }

        private class StubITestActual : ITestActual {

            public StubITestActual(string description) {
                Description = description;
            }

            public string Description { get; }

            public Exception Exception { get; set; }

            public void Verify(TestActualVerifier resultVerifier, TestActualVerifier<Exception> exceptionVerifier) => throw new NotImplementedException();
        }

        #endregion Helpers
    }
}