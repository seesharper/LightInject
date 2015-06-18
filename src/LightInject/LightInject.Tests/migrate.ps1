Param(
  [string ]$ProjectDir
)
get-childitem $ProjectDir -recurse -include *.cs |
 select -expand fullname |
  foreach {
            ( Get-Content $_ ) -replace '\[TestMethod\]','[Fact]' `
            -replace 'Assert.AreEqual' , 'Assert.Equal' `
            -replace 'Assert.AreNotEqual' , 'Assert.NotEqual' `
            -replace 'Assert.IsTrue' , 'Assert.True' `
            -replace 'Assert.IsFalse' , 'Assert.False' `
            -replace 'Assert.IsNotNull' , 'Assert.NotNull' `
            -replace 'Assert.IsNull' , 'Assert.Null' `
            -replace '\[TestClass\]' , ' ' `
            -replace 'Assert.AreNotSame' , 'Assert.NotSame' `
            -replace 'Assert.AreSame' , 'Assert.NotSame' `
            -replace 'Assert.IsInstanceOfType' , 'Assert.IsType' `
            -replace 'Assert.IsNotInstanceOfType' , 'Assert.IsNotType' `
            -replace 'using Microsoft.VisualStudio.TestTools.UnitTesting' , 'using Xunit' `
            | Set-Content -Encoding utf8 $_
            } 