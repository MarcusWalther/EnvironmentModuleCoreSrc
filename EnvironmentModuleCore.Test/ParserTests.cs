using EnvironmentModuleCore.Template;
using EnvironmentModuleCore.Test.Dummy;
using System.Dynamic;

namespace EnvironmentModuleCore.Test
{
    [TestClass]
    public class ParserTests
    {
        private string GetExampleContent(string name)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", $"{name}.txt");
            return File.ReadAllText(path);
        }

        private string GetExpectedContent(string name)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Expected", $"{name}.txt");
            return File.ReadAllText(path);
        }

        /// <summary>
        /// Test that an empty string results in an empty list of tokens.
        /// </summary>
        [TestMethod]
        public void TestEmptyString()
        {
            var template = Template.Template.Parse("");
            Assert.HasCount(0, template.Tokens);
        }

        /// <summary>
        /// Verifies that the parser correctly tokenizes a simple example string containing text and a variable into the expected sequence of tokens.
        /// </summary>
        [TestMethod]
        public void TestSimpleString()
        {
            string content = GetExampleContent("ExampleSimple");
            var template = Template.Template.Parse(content);
            Assert.HasCount(5, template.Tokens);
            Assert.AreEqual(TokenType.Text, template.Tokens[0].TokenType);
            Assert.AreEqual(TokenType.CommandBegin, template.Tokens[1].TokenType);
            Assert.AreEqual(TokenType.Parameter, template.Tokens[2].TokenType);
            Assert.AreEqual(TokenType.CommandEnd, template.Tokens[3].TokenType);
            Assert.AreEqual(TokenType.Text, template.Tokens[4].TokenType);
            
            Assert.AreEqual("A simple line of ", template.Tokens[0].Value);
            Assert.AreEqual("Variable", template.Tokens[2].Value);
            Assert.AreEqual(".", template.Tokens[4].Value);

            var data = new Dictionary<string, object>();
            data["Variable"] = "test";
            Assert.AreEqual("A simple line of test.", template.Render(data));
        }

        /// <summary>
        /// Check if a text only template is rendered correctly.
        /// </summary>
        [TestMethod]
        public void TestPlainText()
        {
            string content = GetExampleContent("ExamplePlainText");
            var template = Template.Template.Parse(content);
            Assert.HasCount(1, template.Tokens);

            Assert.AreEqual(TokenType.Text, template.Tokens[0].TokenType);
            Assert.AreEqual("  A simple line of text.  ", template.Tokens[0].Value);
        }

        /// <summary>
        /// Check if an escaped text is detected correctly.
        /// </summary>
        [TestMethod]
        public void TestEscapeText()
        {
            string content = GetExampleContent("ExampleEscape");
            var template = Template.Template.Parse(content);
            Assert.HasCount(1, template.Tokens);

            Assert.AreEqual(TokenType.Text, template.Tokens[0].TokenType);
            Assert.AreEqual("Hello this is {{ name }}", template.Tokens[0].Value);
        }

        /// <summary>
        /// Check if a template containing only a variable is rendered correctly.
        /// </summary>
        [TestMethod]
        public void TestPlainVariable()
        {
            string content = GetExampleContent("ExamplePlainVariable");
            var template = Template.Template.Parse(content);
            Assert.HasCount(3, template.Tokens);

            Assert.AreEqual(TokenType.CommandBegin, template.Tokens[0].TokenType);
            Assert.AreEqual(TokenType.Parameter, template.Tokens[1].TokenType);
            Assert.AreEqual(TokenType.CommandEnd, template.Tokens[2].TokenType);
            Assert.AreEqual("Variable", template.Tokens[1].Value);

            // Check if a dynamic object variable is rendered correctly
            dynamic data = new ExpandoObject();
            data.Variable = "test";
            
            Assert.AreEqual(data.Variable, template.Render(data));
        }

        /// <summary>
        /// Check if a template containing only a variable is rendered correctly.
        /// </summary>
        [TestMethod]
        public void TestTrimmedVariable()
        {
            string content = GetExampleContent("ExampleTrimmedVariable");
            var template = Template.Template.Parse(content);

            Assert.AreEqual(TokenType.CommandBegin, template.Tokens[1].TokenType);
            Assert.AreEqual("-", template.Tokens[1].Value);
            Assert.AreEqual(TokenType.Parameter, template.Tokens[2].TokenType);
            Assert.AreEqual(TokenType.CommandEnd, template.Tokens[3].TokenType);
            Assert.AreEqual("Variable", template.Tokens[2].Value);

            // Check if a dynamic object variable is rendered correctly
            dynamic data = new ExpandoObject();
            data.Variable = "test";

            Assert.AreEqual($"A text with trimmed{data.Variable}  and with both sides{data.Variable}.", template.Render(data));
        }

        /// <summary>
        /// Check if a template containing only a variable is rendered correctly when using a real class object.
        /// </summary>
        [TestMethod]
        public void TestPlainVariableObject()
        {
            string content = GetExampleContent("ExamplePlainVariable");
            var template = Template.Template.Parse(content);

            // Check if a real class object variable is rendered correctly
            var data = new DummyClass
            {
                Variable = "test"
            };

            Assert.AreEqual(data.Variable, template.Render(data));
        }

        /// <summary>
        /// Check if a template containing a variable that is referencing a property is handled correctly.
        /// </summary>
        [TestMethod]
        public void TestVariableProperty()
        {
            string content = GetExampleContent("ExampleVariableProperty");
            var template = Template.Template.Parse(content);

            // Check if a real class object variable is rendered correctly
            var data = new DummyClass
            {
                Variable = "test"
            };

            Assert.AreEqual(data.Variable, template.Render(new Dictionary<string, object>{{"Element", data}}));
        }

        /// <summary>
        /// Check if a simple "if" condition is parsed and rendered correctly.
        /// </summary>
        [TestMethod]
        public void TestSimpleIf()
        {
            string content = GetExampleContent("ExampleSimpleIf");
            var template = Template.Template.Parse(content);

            Assert.HasCount(10, template.Tokens);

            var data = new Dictionary<string, object>();
            data["Condition"] = "something";
            Assert.AreEqual("A simple line of  conditional text .", template.Render(data));

            data = new Dictionary<string, object>();
            data["Condition"] = false;
            Assert.AreEqual("A simple line of .", template.Render(data));

            data = new Dictionary<string, object>();
            data["Condition"] = null;
            Assert.AreEqual("A simple line of .", template.Render(data));
        }

        /// <summary>
        /// Check if a complex "if" condition is parsed and rendered correctly.
        /// </summary>
        [TestMethod]
        public void TestIfCondition()
        {
            string content = GetExampleContent("ExampleIfCondition");
            var template = Template.Template.Parse(content);

            Assert.HasCount(16, template.Tokens);
            Assert.AreEqual(TokenType.Comparator, template.Tokens[4].TokenType);

            var data = new Dictionary<string, object>();
            data["Value"] = "something";
            Assert.AreEqual("A simple line of  conditional text .", template.Render(data));

            data = new Dictionary<string, object>();
            data["Value"] = "";
            Assert.AreEqual("A simple line of .", template.Render(data));

            data = new Dictionary<string, object>();
            data["Value"] = null;
            Assert.AreEqual("A simple line of .", template.Render(data));
        }

        /// <summary>
        /// Check if a simple "if-else" condition is parsed and rendered correctly.
        /// </summary>
        [TestMethod]
        public void TestSimpleIfElse()
        {
            string content = GetExampleContent("ExampleSimpleIfElse");
            var template = Template.Template.Parse(content);

            Assert.HasCount(14, template.Tokens);

            var data = new Dictionary<string, object>();
            data["Condition"] = "something";
            Assert.AreEqual("A simple line of  conditional text .", template.Render(data));

            data = new Dictionary<string, object>();
            data["Condition"] = false;
            Assert.AreEqual("A simple line of  nothing .", template.Render(data));
        }

        /// <summary>
        /// Check if a simple "for" condition is parsed and rendered correctly.
        /// </summary>
        [TestMethod]
        public void TestSimpleFor()
        {
            string content = GetExampleContent("ExampleSimpleFor");
            var template = Template.Template.Parse(content);

            Assert.HasCount(16, template.Tokens);

            dynamic data = new ExpandoObject();
            data.Entries = new List<DummyClass> {new ("VariableA"), new ("VariableB")};
            Assert.AreEqual("A simple line of  Entry:VariableA  Entry:VariableB .", template.Render(data));
        }

        /// <summary>
        /// Check if a simple "for" condition is parsed and rendered correctly if the value is null.
        /// </summary>
        [TestMethod]
        public void TestSimpleForNullValue()
        {
            string content = GetExampleContent("ExampleSimpleFor");
            var template = Template.Template.Parse(content);

            Assert.HasCount(16, template.Tokens);

            dynamic data = new ExpandoObject();
            // data.Entries = new List<DummyClass> { new("VariableA"), new("VariableB") };
            Assert.AreEqual("A simple line of .", template.Render(data));
        }

        /// <summary>
        /// Check if a complex "for" condition is parsed and rendered correctly.
        /// </summary>
        [TestMethod]
        public void TestComplexFor()
        {
            string content = GetExampleContent("ExampleComplexFor");
            string expected = GetExpectedContent("ExampleComplexFor");
            var template = Template.Template.Parse(content);

            dynamic data = new ExpandoObject();
            data.Entries = new List<DummyDependencyClass> { new("My Module A", true), new("MyModuleB") };
            Assert.AreEqual(expected, template.Render(data));
        }

        /// <summary>
        /// Check if a dictionary based "for" condition is parsed and rendered correctly.
        /// </summary>
        [TestMethod]
        public void TestDictFor()
        {
            string content = GetExampleContent("ExampleDictFor");
            string expected = GetExpectedContent("ExampleDictFor");
            var template = Template.Template.Parse(content);

            dynamic data = new ExpandoObject();
            data.Entries = new Dictionary<string, DummyClass> { {"A", new("VariableA")}, {"B", new("VariableB") } };
            Assert.AreEqual(expected, template.Render(data));
        }
    }
}
