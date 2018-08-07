using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InAsync;

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

            actual.Verify(resultVerifier, (exception, description) => Assert.AreEqual(expectedExceptionType, exception?.GetType(), description));
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

            actual.Verify(resultVerifier, (exception, description) => Assert.AreEqual(expectedExceptionType, exception?.GetType(), description));
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
                (exception, description) => Assert.AreEqual(expectedExceptionType, exception?.GetType(), description)
            );
        }

        /// <summary>
        /// テスト対象コードの実行結果を検証します。
        /// </summary>
        /// <typeparam name="TResult">テスト対象コードの戻り値の型。</typeparam>
        /// <param name="actual">テスト対象コードの実行結果。</param>
        /// <param name="expectedResult">テスト対象コードの戻り値として期待される値。</param>
        /// <param name="resultComparer"><typeparamref name="TResult"/> の比較子。<c>null</c> の場合は <see cref="Comparer{T}.Default"/> が使用される。</param>
        /// <param name="expectedExceptionType">テスト対象コードによって生じる事が期待される例外の型。</param>
        /// <exception cref="ArgumentNullException"><paramref name="actual"/> or <paramref name="resultVerifier"/> is <c>null</c>.</exception>
        public static void Verify<TResult>(this ITestActual<IEnumerable<TResult>> actual, IEnumerable<TResult> expectedResult, IComparer<TResult> resultComparer, Type expectedExceptionType) {
            if (actual == null) { throw new ArgumentNullException(nameof(actual)); }
            if (resultComparer == null) {
                resultComparer = Comparer<TResult>.Default;
            }

            actual.Verify(
                (result, description) => CollectionAssert.AreEqual(expectedResult.ToArray(), result.ToArray(), (IComparer)resultComparer, description),
                (exception, description) => Assert.AreEqual(expectedExceptionType, exception?.GetType(), description)
            );
        }
    }
}