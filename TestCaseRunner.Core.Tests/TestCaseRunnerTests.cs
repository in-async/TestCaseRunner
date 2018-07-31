using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InAsync.Tests {

    [TestClass]
    public class TestCaseRunnerTests {

        [DataTestMethod]
        [DataRow(0, null, null, typeof(ArgumentNullException))]
        [DataRow(1, "123", 123, null)]
        public void Usage(int testNumber, string input, int expected, Type expectedExceptionType) {
            new TestCaseRunner($"No.{testNumber}")
                .Run(() => int.Parse(input))
                .Verify(
                    (result, description) => Assert.AreEqual(expected, result, description),
                    (ex, description) => Assert.AreEqual(expectedExceptionType, ex?.GetType(), description)
                );
        }

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
            IEnumerable<(int testNumber, string description, Type expectedExceptionType)> TestCases() => new(int testNumber, string description, Type expectedExceptionType)[]{
                (10, null , null),
                (11, ""   , null),
                (12, "  " , null),
                (13, "foo", null),
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
            IEnumerable<(int testNumber, Func<int> targetCode, ITestActual<int> expected, Type expectedExceptionType)> TestCases() => new(int testNumber, Func<int> targetCode, ITestActual<int> expected, Type expectedExceptionType)[]{
                ( 0, null                               , null                                   , typeof(ArgumentNullException)),
                (10, () => 0                            , TestActual( 0, null)                   , null),
                (11, () => 1                            , TestActual( 1, null)                   , null),
                (12, () => -1                           , TestActual(-1, null)                   , null),
                (13, () => throw new Exception()        , TestActual( 0, new Exception())        , null),
                (14, () => throw new ArgumentException(), TestActual( 0, new ArgumentException()), null),
            };

            ITestActual<TResult> TestActual<TResult>(TResult result, Exception ex) => new StubITestActual<TResult>("desc") { Result = result, Exception = ex };
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
            IEnumerable<(int testNumber, Action targetCode, ITestActual expected, Type expectedExceptionType)> TestCases() => new(int testNumber, Action targetCode, ITestActual expected, Type expectedExceptionType)[]{
                ( 0, null                               , null                                  , typeof(ArgumentNullException)),
                (10, () => { }                          , TestActual(null)                   , null),
                (11, () => throw new Exception()        , TestActual(new Exception())        , null),
                (12, () => throw new ArgumentException(), TestActual(new ArgumentException()), null),
            };

            ITestActual TestActual(Exception ex) => new StubITestActual("desc") { Exception = ex };
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