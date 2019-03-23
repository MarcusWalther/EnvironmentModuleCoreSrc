// <copyright file="TemplateRenderer.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

namespace EnvironmentModuleCore
{
    using System.Collections.Generic;
    using System.IO;

    using Scriban;

    /// <summary>
    /// This rendered class can be used to convert liquid template files to concrete files.
    /// </summary>
    public class TemplateRenderer
    {
        /// <summary>
        /// Create a concrete file from a template file using the given model definition.
        /// </summary>
        /// <param name="modelDefinition">The data source that is forwarded to the renderer.</param>
        /// <param name="templateFile">The template file to render.</param>
        /// <param name="targetFile">The target file to produce.</param>
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

        /// <summary>
        /// Create a concrete file from a template file using the given model definition.
        /// </summary>
        /// <param name="modelDefinition">The data source as key-value-pairs that is forwarded to the renderer.</param>
        /// <param name="templateFile">The template file to render.</param>
        /// <param name="targetFile">The target file to produce.</param>
        // ReSharper disable once UnusedMember.Global
        public static void CreateConcreteFileFromTemplate(IDictionary<string, object> modelDefinition, string templateFile, string targetFile)
        {
            string templateContent = File.ReadAllText(templateFile);
            Template template = Template.Parse(templateContent);
            string concreteContent = template.Render(modelDefinition, memberRenamer: member => member.Name);
            File.WriteAllText(targetFile, concreteContent);
        }

        /// <summary>
        /// Convert the given template files using the model definition as data source.
        /// </summary>
        /// <param name="modelDefinition">The data source that is forwarded to the renderer.</param>
        /// <param name="templateFiles">The template files as key-value-pairs (source file name, target file name).</param>
        internal static void CreateConcreteFilesFromTemplates(object modelDefinition, Dictionary<string, string> templateFiles)
        {
            foreach (KeyValuePair<string, string> templateFile in templateFiles)
            {
                CreateConcreteFileFromTemplate(modelDefinition, templateFile.Key, templateFile.Value);
            }
        }
    }
}
