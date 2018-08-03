using System;

namespace InAsync {

    /// <summary>
    /// テストを実行するクラス。
    /// </summary>
    public struct TestCaseRunner {

        /// <summary>
        /// <see cref="TestCaseRunner"/> のコンストラクター。
        /// </summary>
        /// <param name="description">テストの説明。</param>
        public TestCaseRunner(string description) {
            Description = description;
        }

        /// <summary>
        /// テストの説明。
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 対象コードのテストを実施します。
        /// </summary>
        /// <typeparam name="TResult">対象コードの戻り値の型。</typeparam>
        /// <param name="targetCode">テストの対象コード。</param>
        /// <returns>対象コードの実行結果。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="targetCode"/> is <c>null</c>.</exception>
        public ITestActual<TResult> Run<TResult>(Func<TResult> targetCode) {
            if (targetCode == null) { throw new ArgumentNullException(nameof(targetCode)); }

            try {
                var result = targetCode();
                return new TestActual<TResult>(Description, null, result);
            }
            catch (Exception ex) {
                return new TestActual<TResult>(Description, ex, default(TResult));
            }
        }

        /// <summary>
        /// 対象コードのテストを実施します。
        /// </summary>
        /// <param name="targetCode">テストの対象コード。</param>
        /// <returns>対象コードの実行結果。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="targetCode"/> is <c>null</c>.</exception>
        public ITestActual Run(Action targetCode) {
            if (targetCode == null) { throw new ArgumentNullException(nameof(targetCode)); }

            try {
                targetCode();
                return new TestActual(Description, null);
            }
            catch (Exception ex) {
                return new TestActual(Description, ex);
            }
        }
    }

    /// <summary>
    /// テスト対象コードの実行結果を検証するデリゲート。
    /// </summary>
    /// <param name="description">テストの説明。</param>
    public delegate void TestActualVerifier(string description);

    /// <summary>
    /// テスト対象コードの実行結果を検証するデリゲート。
    /// </summary>
    /// <typeparam name="TActual"><paramref name="actual" /> の型。</typeparam>
    /// <param name="actual">検証対象のオブジェクト。</param>
    /// <param name="description">テストの説明。</param>
    public delegate void TestActualVerifier<TActual>(TActual actual, string description);

    public interface ITestActual {

        /// <summary>
        /// テストの説明。
        /// </summary>
        string Description { get; }

        /// <summary>
        /// テスト対象コードから生じた例外。
        /// </summary>
        Exception Exception { get; }
    }

    public interface ITestActual<TResult> : ITestActual {

        /// <summary>
        /// テスト対象コードの戻り値。
        /// </summary>
        TResult Result { get; }
    }

    internal class TestActual : ITestActual {

        public TestActual(string description, Exception exception) {
            Description = description;
            Exception = exception;
        }

        public string Description { get; }

        public Exception Exception { get; }
    }

    internal class TestActual<TResult> : TestActual, ITestActual<TResult> {

        internal TestActual(string description, Exception exception, TResult result) : base(description, exception) {
            Result = result;
        }

        public TResult Result { get; }
    }

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