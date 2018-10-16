using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inasync.Tests {

    [TestClass]
    public class Usage {

        [DataTestMethod]
        [DataRow(0, null, null, typeof(ArgumentNullException))]
        [DataRow(1, "123", 123, null)]
        [DataRow(2, "abc", null, typeof(FormatException))]
        public void MSTest_v2(int testNumber, string input, int expected, Type expectedExceptionType) {
            new TestCaseRunner($"No.{testNumber}")
                .Run(() => int.Parse(input))
                .Verify(expected, expectedExceptionType);
        }

        [TestMethod]
        public void MSTest() {
            foreach (var item in TestCases()) {
                new TestCaseRunner($"No.{item.testNumber}")
                    .Run(() => int.Parse(item.input))
                    .Verify(item.expected, item.expectedExceptionType);
            }

            (int testNumber, string input, int expected, Type expectedExceptionType)[] TestCases() => new[] {
                (0, null , 0  , (Type)typeof(ArgumentNullException)),
                (1, "123", 123, (Type)null),
                (2, "abc", 0  , (Type)typeof(FormatException)),
            };
        }
    }
}