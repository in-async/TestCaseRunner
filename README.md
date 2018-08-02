# TestCaseRunner
`TestCaseRunner` は可読性の高いテストコードを記述するための .NET ライブラリです。


## Description
基本機能は `TestCaseRunner.Core` にあります。 

`TestCaseRunner.MSTest` には、テストコードの実行による戻り値 (actual) の検証処理を簡素化した MSTest 用の拡張メソッドが含まれています。


## Target Frameworks
- .NET Standard 1.0+
- .NET Core 1.0+
- .NET Framework 4.5+


## Usage
### Basic
```cs
public void IntParseTest() {
    new TestCaseRunner($"Success")
        .Run(() => int.Parse("123"))
        .Verify(
            (result, description) => Assert.Equals(123, result, description),
            (exception, description) => Assert.Equals(null, exception?.GetType(), description)
        );

    new TestCaseRunner($"Exception")
        .Run(() => int.Parse("abc"))
        .Verify(
            (result, description) => {},
            (exception, description) => Assert.Equals(typeof(FormatException), exception?.GetType(), description)
        );
}
```

### for MSTest
```cs
[TestMethod]
public void IntParseTest() {
    new TestCaseRunner($"Success")
        .Run(() => int.Parse("123"))
        .Verify(123, null);

    new TestCaseRunner($"Failed")
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


## Licence
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details