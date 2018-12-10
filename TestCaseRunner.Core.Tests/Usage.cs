using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inasync.Tests {

    [TestClass]
    public class Usage {

        [TestMethod]
        public void Basic() {
            foreach (var item in TestCases()) {
                new TestCaseRunner($"No.{item.testNumber}")
                    .Run(() => int.Parse(item.input))
                    .Verify(
                        (actual, description) => Assert.AreEqual(item.expected, actual, description),
                        (exception, description) => Assert.AreEqual(item.expectedExceptionType, exception?.GetType(), description)
                    );
            }

            (int testNumber, string input, int expected, Type expectedExceptionType)[] TestCases() => new[] {
                (0, null , 0  , (Type)typeof(ArgumentNullException)),
                (1, "123", 123, (Type)null),
                (2, "abc", 0  , (Type)typeof(FormatException)),
            };
        }
    }
}