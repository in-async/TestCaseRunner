namespace Inasync {

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
}