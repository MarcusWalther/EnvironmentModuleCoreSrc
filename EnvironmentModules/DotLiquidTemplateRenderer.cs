using System.Collections.Generic;
using System.IO;

using DotLiquid;

namespace EnvironmentModules
{
    internal class DotLiquidTemplateRenderer
    {
        internal static void CreateConcreteFilesFromTemplates(object modelDefinition, Dictionary<string, string> templateFiles)
        {
            foreach (KeyValuePair<string, string> templateFile in templateFiles)
            {
                FileInfo templateFileInfo = new FileInfo(templateFile.Key);

                if (!templateFileInfo.Exists)
                {
                    throw new EnvironmentModuleException($"The template file '{templateFileInfo.FullName}' does not exist");
                }

                string templateContent = File.ReadAllText(templateFile.Key);
                Template template = Template.Parse(templateContent);
                string concreteContent = template.Render(Hash.FromAnonymousObject(modelDefinition));
                File.WriteAllText(templateFile.Value, concreteContent);
            }
        }
    }
}
