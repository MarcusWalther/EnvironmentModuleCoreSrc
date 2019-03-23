// <copyright file="TemplateRenderer.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

namespace EnvironmentModuleCore
{
    using System.Collections.Generic;
    using System.IO;

    using Scriban;

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
                throw new ModuleException($"The template file '{templateFileInfo.FullName}' does not exist");
            }

            string templateContent = File.ReadAllText(templateFile);
            Template template = Template.Parse(templateContent);
            string concreteContent = template.Render(modelDefinition, memberRenamer: member => member.Name);
            File.WriteAllText(targetFile, concreteContent);
        }

        public static void CreateConcreteFileFromTemplate(IDictionary<string, object> modelDefinition, string templateFile, string targetFile)
        {
            string templateContent = File.ReadAllText(templateFile);
            Template template = Template.Parse(templateContent);
            string concreteContent = template.Render(modelDefinition, memberRenamer: member => member.Name);
            File.WriteAllText(targetFile, concreteContent);
        }
    }
}
