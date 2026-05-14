using EnvironmentModuleCore.Test.Dummy;
using System.Dynamic;

namespace EnvironmentModuleCore.Test
{
    [TestClass]
    public class TemplateRegressionTests
    {
        private string GetExampleContent(string name)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", $"{name}.txt");
            return File.ReadAllText(path);
        }

        private string RenderScriban(string name, object modelDefinition)
        {
            string templateContent = GetExampleContent(name);
            Scriban.Template template = Scriban.Template.Parse(templateContent);

            // Renamer: keep PascalCase, otherwise Scriban will convert variables to snake_case by default
            return template.Render(modelDefinition, memberRenamer: member => member.Name);
        }

        private string Render(string name, object modelDefinition)
        {
            string templateContent = GetExampleContent(name);
            Template.Template template = Template.Template.Parse(templateContent);
            return template.Render(modelDefinition);
        }

        [TestMethod]
        public void TestRegressionSimple()
        {
            string name = "ExampleSimple";
            var model = new Dictionary<string, object>();
            model["Variable"] = "test";

            Assert.AreEqual(RenderScriban(name, model), Render(name, model));
        }

        [TestMethod]
        public void TestRegressionPlainText()
        {
            string name = "ExamplePlainText";
            var model = new Dictionary<string, object>();
            model["Variable"] = "test";

            Assert.AreEqual(RenderScriban(name, model), Render(name, model));
        }

        [TestMethod]
        public void TestRegressionVariableProperty()
        {
            string name = "ExampleVariableProperty";
            var model = new Dictionary<string, object>();
            model["Element"] = new DummyClass("Value");

            Assert.AreEqual(RenderScriban(name, model), Render(name, model));
        }

        [TestMethod]
        public void TestRegressionSimpleIf()
        {
            string name = "ExampleSimpleIf";
            var model = new Dictionary<string, object>();
            model["Condition"] = "test";

            Assert.AreEqual(RenderScriban(name, model), Render(name, model));
        }

        [TestMethod]
        public void TestRegressionSimpleIfElse()
        {
            string name = "ExampleSimpleIfElse";
            var model = new Dictionary<string, object>();
            model["Condition"] = "test";

            Assert.AreEqual(RenderScriban(name, model), Render(name, model));

            model["Condition"] = null;
            Assert.AreEqual(RenderScriban(name, model), Render(name, model));
        }

        [TestMethod]
        public void TestRegressionSimpleFor()
        {
            string name = "ExampleSimpleFor";
            var model = new Dictionary<string, object>();
            model["Entries"] = new List<DummyClass> { new("VariableA"), new("VariableB") };

            Assert.AreEqual(RenderScriban(name, model), Render(name, model));
        }

        [TestMethod]
        public void TestRegressionComplexFor()
        {
            string name = "ExampleComplexFor";
            var model = new Dictionary<string, object>();
            model["Entries"] = new List<DummyDependencyClass> { new("My Module A", true), new("MyModuleB") };

            Assert.AreEqual(RenderScriban(name, model), Render(name, model));
        }
    }
}
