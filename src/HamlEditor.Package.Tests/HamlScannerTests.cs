using HamlEditor;
using Microsoft.VisualStudio.Package;
using NUnit.Framework;

namespace HamlEditor.Package.Tests
{
    [TestFixture]
    public class HamlScannerTests
    {
        private static void TestScanner(string line, TestTokenInfo[] tokens)
        {
            var scanner = new HamlScanner();

            string testLine = "";
            foreach (TestTokenInfo token in tokens)
            {
                token.StartIndex = testLine.Length;
                token.EndIndex = token.StartIndex + token.Token.Length -1;
                testLine += token.Token;
            }
            Assert.AreEqual(line, testLine, "Test line doesn't match tokens");

            scanner.SetSource(line, 0);

            var tokenInfo = new TokenInfo();
            int state = 0;

            int tokenNum = 0;
            while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state))
            {
                TestTokenInfo token = tokens[tokenNum];
                AssertAreEqual(token, tokenInfo, string.Format("Token {0}({1})", tokenNum, token.Token));
                tokenNum++;
            }

            Assert.AreEqual(tokens.Length, tokenNum, "Should have matched {0} tokens but actually matched {1}", tokens.Length, tokenNum);
        }

        private static void AssertAreEqual(TestTokenInfo infoExpected, TokenInfo infoActual, string message)
        {
            Assert.AreEqual(infoExpected.StartIndex, infoActual.StartIndex, message + " StartIndex");
            Assert.AreEqual(infoExpected.EndIndex, infoActual.EndIndex, message + " EndIndex");
            Assert.AreEqual((int)infoExpected.HamlTokenColor, (int)infoActual.Color, message + " Color");
        }

        [Test]
        public void CanScanLineWithSimpleTag()
        {
            const string line = @"%table";
            var tokens = new[]{ new TestTokenInfo("%table", HamlTokenColor.HamlTag) };

            TestScanner(line, tokens);
        }

        [Test]
        public void CanScanSimpleLine()
        {
            const string line = @"  %td Unit Price";

            var tokens = new[] {
                                   new TestTokenInfo("  ", HamlTokenColor.HamlWhitespace),
                                   new TestTokenInfo("%td", HamlTokenColor.HamlTag),
                                   new TestTokenInfo(" Unit Price", HamlTokenColor.HamlText)
                               };

            TestScanner(line, tokens);
        }

        [Test]
        public void CanScanLineWithCode()
        {
            const string line = @"      %td= Html.TextBox(ViewData.Product.ProductName)";

            var tokens = new[] {
                                   new TestTokenInfo("      ", HamlTokenColor.HamlWhitespace),
                                   new TestTokenInfo("%td", HamlTokenColor.HamlTag),
                                   new TestTokenInfo(@"= Html.TextBox(ViewData.Product.ProductName)", 
                                       HamlTokenColor.HamlCode)
                             };

            TestScanner(line, tokens);
        }

        [Test]
        public void CanScanLineWithCode2()
        {
            const string line = @"%form{action=Url.Action(new { ...";

            var tokens = new[] {
                                   new TestTokenInfo("%form", HamlTokenColor.HamlTag),
                                   new TestTokenInfo(@"{action=Url.Action(new { ...", 
                                       HamlTokenColor.HamlCode)
                             };

            TestScanner(line, tokens);
        }

        [Test]
        public void CanScanLineWithCodeAndNoTag()
        {
            const string line = @" = Html.ActionLink()";

            var tokens = new[] {
                                   new TestTokenInfo(@" ", HamlTokenColor.HamlWhitespace),
                                   new TestTokenInfo(@"= Html.ActionLink()", HamlTokenColor.HamlCode)
                             };

            TestScanner(line, tokens);
        }

        [Test]
        public void CanScanLineWithPartial()
        {
            const string line = @"_ Shared\Form";

            var tokens = new[] {
                                   new TestTokenInfo(@"_ Shared\Form", HamlTokenColor.HamlPartial)
                             };

            TestScanner(line, tokens);
        }

        [Test]
        public void CanScanLineWithMinusCodeNoTag()
        {
            const string line = @"  - foreach (var product in ViewData.Products)";

            var tokens = new[] {
                                   new TestTokenInfo("  ", HamlTokenColor.HamlWhitespace),
                                   new TestTokenInfo(@"- foreach (var product in ViewData.Products)", HamlTokenColor.HamlCode)
                             };

            TestScanner(line, tokens);
        }

        [Test]
        public void CanScanLineWithEqualsCodeNoTag()
        {
            const string line = @"  = foreach (var product in ViewData.Products)";

            var tokens = new[] {
                                   new TestTokenInfo("  ", HamlTokenColor.HamlWhitespace),
                                   new TestTokenInfo(@"= foreach (var product in ViewData.Products)", HamlTokenColor.HamlCode)
                             };

            TestScanner(line, tokens);
        }

        [Test]
        public void CanScanLineWithIdAndClass()
        {
            const string line = @"%tagname#id.class Hi";

            var tokens = new[] {
                                   new TestTokenInfo("%tagname", HamlTokenColor.HamlTag),
                                   new TestTokenInfo("#id", HamlTokenColor.HamlTagId),
                                   new TestTokenInfo(".class", HamlTokenColor.HamlTagClass),
                                   new TestTokenInfo(@" Hi", HamlTokenColor.HamlText)
                             };

            TestScanner(line, tokens);
        }

        [Test]
        public void CanScanLineWithIdAndCode()
        {
            const string line = @"%input#email{type=text, name=email}";

            var tokens = new[] {
                                   new TestTokenInfo("%input", HamlTokenColor.HamlTag),
                                   new TestTokenInfo("#email", HamlTokenColor.HamlTagId),
                                   new TestTokenInfo(@"{type=text, name=email}", HamlTokenColor.HamlCode),
                             };

            TestScanner(line, tokens);
        }

        [Test]
        public void CanScanLineWithIdAndClassAndCode()
        {
            const string line = @"%tagname#id.class= Method()";

            var tokens = new[] {
                                   new TestTokenInfo("%tagname", HamlTokenColor.HamlTag),
                                   new TestTokenInfo("#id", HamlTokenColor.HamlTagId),
                                   new TestTokenInfo(".class", HamlTokenColor.HamlTagClass),
                                   new TestTokenInfo(@"= Method()", HamlTokenColor.HamlCode)
                             };

            TestScanner(line, tokens);
        }

        [Test]
        public void CanScanCodeLineWithComment()
        {
            const string line = @"%p - Code // This is a comment";

            var tokens = new[] {
                                   new TestTokenInfo("%p", HamlTokenColor.HamlTag),
                                   new TestTokenInfo(" - Code ", HamlTokenColor.HamlCode),
                                   new TestTokenInfo("// This is a comment", HamlTokenColor.CsComment),
                             };

            TestScanner(line, tokens);
        }

        [Test]
        public void CanScanCodeLineWithString()
        {
            const string line = @"%p - Method(""hi"")";

            var tokens = new[] {
                                   new TestTokenInfo("%p", HamlTokenColor.HamlTag),
                                   new TestTokenInfo(" - Method(", HamlTokenColor.HamlCode),
                                   new TestTokenInfo("\"hi\"", HamlTokenColor.CsString),
                                   new TestTokenInfo(")", HamlTokenColor.HamlCode),
                             };

            TestScanner(line, tokens);
        }

        [Test]
        public void CanScanNoTagCodeLineWithString()
        {
            const string line = @"- ""ReturnUrl""";

            var tokens = new[] {
                                   new TestTokenInfo("- ", HamlTokenColor.HamlCode),
                                   new TestTokenInfo("\"ReturnUrl\"", HamlTokenColor.CsString),
                             };

            TestScanner(line, tokens);
        }

        [Test]
        public void CodeLineWithUnendedStringShouldMatchRestOfLine()
        {
            const string line = @"%p - Method(""hi ";

            var tokens = new[] {
                                   new TestTokenInfo("%p", HamlTokenColor.HamlTag),
                                   new TestTokenInfo(" - Method(", HamlTokenColor.HamlCode),
                                   new TestTokenInfo("\"hi ", HamlTokenColor.CsString),
                             };

            TestScanner(line, tokens);
        }

        [Test]
        public void CanScanLineWithBangs()
        {
            const string line = @"!!! XML";

            var tokens = new[] {
                                   new TestTokenInfo("!!! XML", HamlTokenColor.HamlTag),
                             };

            TestScanner(line, tokens);
        }
    }

    public class TestTokenInfo
    {
        public HamlTokenColor HamlTokenColor;
        public string Token;
        public int StartIndex;
        public int EndIndex;

        public TestTokenInfo(string token, HamlTokenColor hamlToken)
        {
            Token = token;
            HamlTokenColor = hamlToken;
        }
    }
}