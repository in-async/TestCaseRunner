using System;

namespace Inasync {

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
}