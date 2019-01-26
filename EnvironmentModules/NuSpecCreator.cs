using System.Collections.Generic;
using System.IO;

namespace EnvironmentModules
{
    public class NuSpecCreator
    {
        public static void CreateNuspecFileForEnvironmentModule(EnvironmentModuleInfo environmentModule, string workingDirectory = null)
        {
            if (environmentModule == null)
                throw new EnvironmentModuleException("Now module given");

            if (workingDirectory == null)
                workingDirectory = Directory.GetCurrentDirectory();

            if (!new DirectoryInfo(workingDirectory).Exists)
                throw new EnvironmentModuleException($"The given working directory '{workingDirectory}' does not exist");

            var modelDefinition = new
            {
                Name = environmentModule.FullName,
                Version = environmentModule.PSModuleInfo.Version.ToString(),
                Authors = environmentModule.PSModuleInfo.Author,
                Description = $"{environmentModule.FullName} EnvironmentModule",
                Dependencies = environmentModule.RequiredEnvironmentModules
            };

            FileInfo templateNuspec = new FileInfo(Path.Combine(workingDirectory, "Templates\\EnvironmentModule.nuspec.template"));
            Dictionary<string, string> templateFiles = new Dictionary<string, string>
            {
                [templateNuspec.FullName] = Path.Combine(environmentModule.ModuleBase.FullName, environmentModule.FullName + ".nuspec")
            };

            DotLiquidTemplateRenderer.CreateConcreteFilesFromTemplates(modelDefinition, templateFiles);
        }
    }
}
