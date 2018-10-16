using System;
using Inasync;

namespace Microsoft.VisualStudio.TestTools.UnitTesting {

    public static class TestActualExtensions {

        /// <summary>
        /// テスト対象コードの実行結果を検証します。
        /// </summary>
        /// <param name="actual">テスト対象コードの実行結果。</param>
        /// <param name="resultVerifier">テスト対象コードの戻り値の検証を行うデリゲート。</param>
        /// <param name="expectedExceptionType">テスト対象コードによって生じる事が期待される例外の型。</param>
        /// <exception cref="ArgumentNullException"><paramref name="actual"/> or <paramref name="resultVerifier"/> is <c>null</c>.</exception>
        public static void Verify(this ITestActual actual, TestActualVerifier resultVerifier, Type expectedExceptionType) {
            if (actual == null) { throw new ArgumentNullException(nameof(actual)); }

            actual.Verify(resultVerifier, CreateDefaultExceptionVerifier(expectedExceptionType));
        }

        /// <summary>
        /// テスト対象コードの実行結果を検証します。
        /// </summary>
        /// <typeparam name="TResult">テスト対象コードの戻り値の型。</typeparam>
        /// <param name="actual">テスト対象コードの実行結果。</param>
        /// <param name="resultVerifier">テスト対象コードの戻り値の検証を行うデリゲート。</param>
        /// <param name="expectedExceptionType">テスト対象コードによって生じる事が期待される例外の型。</param>
        /// <exception cref="ArgumentNullException"><paramref name="actual"/> or <paramref name="resultVerifier"/> is <c>null</c>.</exception>
        public static void Verify<TResult>(this ITestActual<TResult> actual, TestActualVerifier<TResult> resultVerifier, Type expectedExceptionType) {
            if (actual == null) { throw new ArgumentNullException(nameof(actual)); }

            actual.Verify(resultVerifier, CreateDefaultExceptionVerifier(expectedExceptionType));
        }

        /// <summary>
        /// テスト対象コードの実行結果を検証します。
        /// </summary>
        /// <typeparam name="TResult">テスト対象コードの戻り値の型。</typeparam>
        /// <param name="actual">テスト対象コードの実行結果。</param>
        /// <param name="expectedResult">テスト対象コードの戻り値として期待される値。</param>
        /// <param name="expectedExceptionType">テスト対象コードによって生じる事が期待される例外の型。</param>
        /// <exception cref="ArgumentNullException"><paramref name="actual"/> or <paramref name="resultVerifier"/> is <c>null</c>.</exception>
        public static void Verify<TResult>(this ITestActual<TResult> actual, TResult expectedResult, Type expectedExceptionType) {
            if (actual == null) { throw new ArgumentNullException(nameof(actual)); }

            actual.Verify(
                (result, description) => Assert.AreEqual(expectedResult, result, description),
                CreateDefaultExceptionVerifier(expectedExceptionType)
            );
        }

        #region Helpers

        /// <summary>
        /// 既定の例外検証を行う <see cref="TestActualVerifier{TActual}"/> を作成します。
        /// </summary>
        /// <param name="expectedExceptionType">テスト時に期待する例外の型。</param>
        /// <returns>例外検証を行う <see cref="TestActualVerifier{TActual}"/> デリゲート。</returns>
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