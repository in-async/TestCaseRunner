using System;
using InAsync;

namespace Microsoft.VisualStudio.TestTools.UnitTesting {

    public static class ITestActualExtensions {

        /// <summary>
        /// テスト対象コードの実行結果を検証します。
        /// </summary>
        /// <param name="resultVerifier">テスト対象コードの戻り値の検証を行うデリゲート。</param>
        /// <param name="expectedExceptionType">テスト対象コードによって生じる事が期待される例外の型。</param>
        /// <exception cref="ArgumentNullException"><paramref name="actual"/> or <paramref name="resultVerifier"/> is <c>null</c>.</exception>
        public static void Verify(this ITestActual actual, TestActualVerifier resultVerifier, Type expectedExceptionType) {
            if (actual == null) { throw new ArgumentNullException(nameof(actual)); }

            actual.Verify(resultVerifier, (exception, description) => Assert.AreEqual(expectedExceptionType, exception?.GetType(), description));
        }

        /// <summary>
        /// テスト対象コードの実行結果を検証します。
        /// </summary>
        /// <param name="resultVerifier">テスト対象コードの戻り値の検証を行うデリゲート。</param>
        /// <param name="expectedExceptionType">テスト対象コードによって生じる事が期待される例外の型。</param>
        /// <exception cref="ArgumentNullException"><paramref name="actual"/> or <paramref name="resultVerifier"/> is <c>null</c>.</exception>
        public static void Verify<TResult>(this ITestActual<TResult> actual, TestActualVerifier<TResult> resultVerifier, Type expectedExceptionType) {
            if (actual == null) { throw new ArgumentNullException(nameof(actual)); }

            actual.Verify(resultVerifier, (exception, description) => Assert.AreEqual(expectedExceptionType, exception?.GetType(), description));
        }
    }
}