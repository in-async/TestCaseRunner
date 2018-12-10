using System;

namespace Inasync {

    /// <summary>
    /// <see cref="ITestActual{TResult}"/> の拡張クラス。
    /// </summary>
    public static class TestActualExtensions {

        /// <summary>
        /// テスト結果を検証します。
        /// </summary>
        /// <param name="actual">テスト結果。</param>
        /// <param name="resultVerifier">テストの戻り値の検証を行うデリゲート。</param>
        /// <param name="exceptionVerifier">テスト中に生じた例外の検証を行うデリゲート。</param>
        /// <exception cref="ArgumentNullException"><paramref name="actual"/> or <paramref name="resultVerifier"/> or <paramref name="exceptionVerifier"/> is <c>null</c>.</exception>
        public static void Verify(this ITestActual actual, TestActualVerifier resultVerifier, TestActualVerifier<Exception> exceptionVerifier) {
            if (actual == null) { throw new ArgumentNullException(nameof(actual)); }
            if (resultVerifier == null) { throw new ArgumentNullException(nameof(resultVerifier)); }
            if (exceptionVerifier == null) { throw new ArgumentNullException(nameof(exceptionVerifier)); }

            // 期待通りに例外が生じている or 生じていない事を検証。
            exceptionVerifier(actual.Exception, actual.Description);

            // 成功時検証は例外が生じていない時のみ。
            if (actual.Exception == null) {
                resultVerifier(actual.Description);
            }
        }

        /// <summary>
        /// テスト結果を検証します。
        /// </summary>
        /// <typeparam name="TResult">テストの戻り値の型。</typeparam>
        /// <param name="actual">テスト結果。</param>
        /// <param name="resultVerifier">テストの戻り値の検証を行うデリゲート。</param>
        /// <param name="exceptionVerifier">テスト中に生じた例外の検証を行うデリゲート。</param>
        /// <exception cref="ArgumentNullException"><paramref name="actual"/> or <paramref name="resultVerifier"/> or <paramref name="exceptionVerifier"/> is <c>null</c>.</exception>
        public static void Verify<TResult>(this ITestActual<TResult> actual, TestActualVerifier<TResult> resultVerifier, TestActualVerifier<Exception> exceptionVerifier) {
            if (actual == null) { throw new ArgumentNullException(nameof(actual)); }
            if (resultVerifier == null) { throw new ArgumentNullException(nameof(resultVerifier)); }
            if (exceptionVerifier == null) { throw new ArgumentNullException(nameof(exceptionVerifier)); }

            actual.Verify(description => resultVerifier(actual.Result, description), exceptionVerifier);
        }
    }
}