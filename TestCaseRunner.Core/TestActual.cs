using System;

namespace Inasync {

    /// <summary>
    /// テスト結果を表す型。
    /// </summary>
    public interface ITestActual {

        /// <summary>
        /// テストの説明。
        /// </summary>
        string Description { get; }

        /// <summary>
        /// テスト中に生じた例外。
        /// </summary>
        Exception Exception { get; }
    }

    /// <summary>
    /// テスト結果を表す型。
    /// </summary>
    /// <typeparam name="TResult">テストの戻り値の型。</typeparam>
    public interface ITestActual<TResult> : ITestActual {

        /// <summary>
        /// テストの戻り値。
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
}