using System.Collections.Generic;

namespace EnvironmentModuleCore.Template
{
    public class Template
    {
        public List<Token> Tokens { get; }

        public Template(List<Token> tokens)
        {
            Tokens = tokens;
        }

        public string Render(object model = null)
        {
            var renderer = new TemplateRenderer();
            return renderer.Render(this, model);
        }

        public static Template Parse(string content)
        {
            var parser = new Parser();
            return parser.Parse(content);
        }
    }
}
