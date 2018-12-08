using System;
using System.Collections;
using System.Linq;
using Inasync;

namespace Microsoft.VisualStudio.TestTools.UnitTesting {

    /// <summary>
    /// <see cref="ITestActual{TResult}"/> の拡張クラス。
    /// </summary>
    public static class TestActualExtensions {

        /// <summary>
        /// テスト結果を検証します。
        /// </summary>
        /// <param name="actual">テスト結果。</param>
        /// <param name="resultVerifier">テストの戻り値の検証を行うデリゲート。</param>
        /// <param name="expectedExceptionType">テスト中に生じる事が期待される例外の型。</param>
        /// <exception cref="ArgumentNullException"><paramref name="actual"/> or <paramref name="resultVerifier"/> is <c>null</c>.</exception>
        public static void Verify(this ITestActual actual, TestActualVerifier resultVerifier, Type expectedExceptionType) {
            Inasync.TestActualExtensions.Verify(actual, resultVerifier, CreateDefaultExceptionVerifier(expectedExceptionType));
        }

        /// <summary>
        /// テスト結果を検証します。
        /// </summary>
        /// <typeparam name="TResult">テストの戻り値の型。</typeparam>
        /// <param name="actual">テスト結果。</param>
        /// <param name="resultVerifier">テストの戻り値の検証を行うデリゲート。</param>
        /// <param name="expectedExceptionType">テスト中に生じる事が期待される例外の型。</param>
        /// <exception cref="ArgumentNullException"><paramref name="actual"/> or <paramref name="resultVerifier"/> is <c>null</c>.</exception>
        public static void Verify<TResult>(this ITestActual<TResult> actual, TestActualVerifier<TResult> resultVerifier, Type expectedExceptionType) {
            Inasync.TestActualExtensions.Verify(actual, resultVerifier, CreateDefaultExceptionVerifier(expectedExceptionType));
        }

        /// <summary>
        /// テスト結果を検証します。
        /// </summary>
        /// <typeparam name="TResult">テストの戻り値の型。</typeparam>
        /// <param name="actual">テスト結果。</param>
        /// <param name="expectedResult">テストの戻り値として期待される値。</param>
        /// <param name="expectedExceptionType">テスト中に生じる事が期待される例外の型。</param>
        /// <exception cref="ArgumentNullException"><paramref name="actual"/> is <c>null</c>.</exception>
        public static void Verify<TResult>(this ITestActual<TResult> actual, TResult expectedResult, Type expectedExceptionType) {
            Inasync.TestActualExtensions.Verify(
                  actual
                , (result, description) => {
                    if (typeof(TResult) != typeof(string) && expectedResult is IEnumerable expectedResults) {
                        CollectionAssert.AreEqual(expectedResults?.Cast<object>().ToArray(), (result as IEnumerable)?.Cast<object>().ToArray(), description);
                    }
                    else {
                        Assert.AreEqual(expectedResult, result, description);
                    }
                }
                , CreateDefaultExceptionVerifier(expectedExceptionType)
            );
        }

        #region Helpers

        /// <summary>
        /// 既定の例外検証を行う <see cref="TestActualVerifier{TActual}"/> を作成します。
        /// </summary>
        /// <param name="expectedExceptionType">テスト時に期待する例外の型。</param>
        /// <returns>例外検証を行う <see cref="TestActualVerifier{TActual}"/> デリゲート。常に非 <c>null</c>。</returns>
        private static TestActualVerifier<Exception> CreateDefaultExceptionVerifier(Type expectedExceptionType) => (Exception exception, string description) => {
            try {
                Assert.AreEqual(expectedExceptionType, exception?.GetType(), description);
            }
            catch (AssertFailedException) {
#if NETSTANDARD1_0
#else
                if (exception != null) {
                    Console.WriteLine(exception.ToString());
                }
#endif
                throw;
            }
        };

        #endregion Helpers
    }
}