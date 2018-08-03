# TestCaseRunner
`TestCaseRunner` は可読性の高いテストコードを記述するための .NET ライブラリです。


## Target Frameworks
- .NET Standard 1.0+
- .NET Core 1.0+
- .NET Framework 4.5+


## Description
基本的な使い方は、
```cs
new TestCaseRunner(テストケースの説明)
    .Run(テスト対象のコード)
    .Verify(テスト対象コードの戻り値の検証, 例外の検証);
```
となります。

もしテストケース毎に前処理が必要であれば、`new TestCaseRunner()` より前に記述します。
```cs
// 
// 前処理はこの辺に書く。
//

new TestCaseRunner(...)
    ...
```

`TestCaseRunner` のコンストラクター引数にはそのテストケースの説明を書きます。必要が無ければ引数を省略してください。

`Run()` の引数には、実際にテストしたいメソッドやコードを記述します。  
ただし、ここにテスト対象でないコードは書かないで下さい。ここに非テストコードを記述すると、そこから発生した例外がテスト対象コードから生じたものとして扱われてしまいます。
```cs
new TestCaseRunner(...)
    .Run(() => { /* ここでテスト対象のメソッドを呼ぶ */ })
```

`Verify()` で `Run()` の結果を検証します。  
第１引数で `Run()` に渡されたテストコードの戻り値を検証し、第２引数で `Run()` で生じた例外を検証（または例外が生じなかった事を検証）します。
```cs
    .Run(() => int.Parse("123"))
    .Verify(
        (result, description) => { /* ここで result の検証 */ },
        (exception, description) => { /* ここで exception の検証 */ }
    );
```

これらの機能は `TestCaseRunner.Core` に含まれています。

もしテストフレームワークに MSTest を使用しているのであれば、`TestCaseRunner.MSTest` を利用することで `Verify()` の記述を簡素化する事もできます。


## Usage
### Basic
```cs
public void IntParseTest() {
    new TestCaseRunner("Success")
        .Run(() => int.Parse("123"))
        .Verify(
            (result, description) => result.Is(123, description),
            (exception, description) => exception?.GetType().Is(null, description)
        );

    new TestCaseRunner("Exception")
        .Run(() => int.Parse("abc"))
        .Verify(
            (result, description) => { },
            (exception, description) => exception?.GetType().Is(typeof(FormatException), description)
        );
}
```

### for MSTest
```cs
[TestMethod]
public void IntParseTest() {
    new TestCaseRunner("Success")
        .Run(() => int.Parse("123"))
        .Verify(123, (Type)null);

    new TestCaseRunner("Failed")
        .Run(() => int.Parse("abc"))
        .Verify(0, typeof(FormatException));
}
```

より実践的には、下記のように利用します。
```cs
// for MSTest v2
[DataTestMethod]
[DataRow(0, null, null, typeof(ArgumentNullException))]
[DataRow(1, "123", 123, null)]
[DataRow(2, "abc", null, typeof(FormatException))]
public void IntParseTest(int testNumber, string input, int expected, Type expectedExceptionType) {
    new TestCaseRunner($"No.{testNumber}")
        .Run(() => int.Parse(input))
        .Verify(expected, expectedExceptionType);
}
```
```cs
// for MSTest v1
[TestMethod]
public void IntParseTest() {
    foreach (var item in TestCases()) {
        new TestCaseRunner($"No.{item.testNumber}")
            .Run(() => int.Parse(item.input))
            .Verify(item.expected, item.expectedExceptionType);
    }

    (int testNumber, string input, int expected, Type expectedExceptionType)[] TestCases() => new[] {
        (0, null , 0  , typeof(ArgumentNullException)),
        (1, "123", 123, null),
        (2, "abc", 0  , typeof(FormatException)),
    };
}
```
`Run()` に渡されたテストコードの戻りちと `Verify()` の第１引数の比較方法ですが、 MSTest の場合 `Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual()` によって行われます。


## Licence
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
