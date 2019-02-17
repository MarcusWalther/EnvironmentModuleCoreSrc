using System.Collections.Generic;
using System.IO;

using DotLiquid;

namespace EnvironmentModules
{
    public class TemplateRenderer
    {
        internal static void CreateConcreteFilesFromTemplates(object modelDefinition, Dictionary<string, string> templateFiles)
        {
            foreach (KeyValuePair<string, string> templateFile in templateFiles)
            {
                CreateConcreteFileFromTemplate(modelDefinition, templateFile.Key, templateFile.Value);
            }
        }

        public static void CreateConcreteFileFromTemplate(object modelDefinition, string templateFile, string targetFile)
        {
            FileInfo templateFileInfo = new FileInfo(templateFile);

            if (!templateFileInfo.Exists)
            {
                throw new EnvironmentModuleException($"The template file '{templateFileInfo.FullName}' does not exist");
            }

            string templateContent = File.ReadAllText(templateFile);
            Template template = Template.Parse(templateContent);
            string concreteContent = template.Render(Hash.FromAnonymousObject(modelDefinition));
            File.WriteAllText(targetFile, concreteContent);
        }

        public static void CreateConcreteFileFromTemplate(IDictionary<string, object> modelDefinition, string templateFile, string targetFile)
        {
            FileInfo templateFileInfo = new FileInfo(templateFile);

            if (!templateFileInfo.Exists)
            {
                throw new EnvironmentModuleException($"The template file '{templateFileInfo.FullName}' does not exist");
            }

            string templateContent = File.ReadAllText(templateFile);
            Template template = Template.Parse(templateContent);
            string concreteContent = template.Render(Hash.FromDictionary(modelDefinition));
            File.WriteAllText(targetFile, concreteContent);
        }
    }
}
