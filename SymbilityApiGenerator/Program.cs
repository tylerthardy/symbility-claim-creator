using System;
using System.IO;
using System.Threading.Tasks;
using NSwag;
using NSwag.CodeGeneration.CSharp;

namespace SymbilityApiGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var generateApiTask = GenerateApi();
            generateApiTask.Wait();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static async Task GenerateApi()
        {
            Console.WriteLine("Reading Symbility API OpenAPI spec...");
            var document = await OpenApiDocument.FromFileAsync("SymbilityRestApi.json");

            var settings = new CSharpClientGeneratorSettings
            {
                ClassName = "MyClass",
                CSharpGeneratorSettings =
                {
                    Namespace = "MyNamespace"
                }
            };

            Console.WriteLine("Generating Symbility API code...");
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();
            Console.WriteLine("Symbility API code generated.");

            var fileName = "test.txt";
            await File.WriteAllTextAsync(fileName, code);
            Console.WriteLine($"Code written to {fileName}.");
        }
    }
}
