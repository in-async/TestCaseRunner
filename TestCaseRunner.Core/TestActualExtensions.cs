using System;

namespace Inasync {

    public static class TestActualExtensions {

        /// <summary>
        /// テスト対象コードの実行結果を検証します。
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="resultVerifier">テスト対象コードの戻り値の検証を行うデリゲート。</param>
        /// <param name="exceptionVerifier">テスト対象コードの例外の検証を行うデリゲート。</param>
        /// <exception cref="ArgumentNullException"><paramref name="resultVerifier"/> or <paramref name="exceptionVerifier"/> is <c>null</c>.</exception>
        public static void Verify(this ITestActual actual, TestActualVerifier resultVerifier, TestActualVerifier<Exception> exceptionVerifier) {
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
        /// テスト対象コードの実行結果を検証します。
        /// </summary>
        /// <typeparam name="TResult">テスト対象コードの戻り値の型。</typeparam>
        /// <param name="actual"></param>
        /// <param name="resultVerifier">テスト対象コードの戻り値の検証を行うデリゲート。</param>
        /// <param name="exceptionVerifier">テスト対象コードの例外の検証を行うデリゲート。</param>
        /// <exception cref="ArgumentNullException"><paramref name="resultVerifier"/> or <paramref name="exceptionVerifier"/> is <c>null</c>.</exception>
        public static void Verify<TResult>(this ITestActual<TResult> actual, TestActualVerifier<TResult> resultVerifier, TestActualVerifier<Exception> exceptionVerifier) {
            if (resultVerifier == null) { throw new ArgumentNullException(nameof(resultVerifier)); }
            if (exceptionVerifier == null) { throw new ArgumentNullException(nameof(exceptionVerifier)); }

            actual.Verify(description => resultVerifier(actual.Result, description), exceptionVerifier);
        }
    }
}