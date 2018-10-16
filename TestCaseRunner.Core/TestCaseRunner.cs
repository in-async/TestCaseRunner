using System;

namespace Inasync {

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
}