using System.Runtime.Loader;
using carenirvana.bre.codegenerator.Impl;
using carenirvana.bre.model.Impl;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace carenirvana.bre.codebuilder
{
    public class BuildAll()
    {
        public void Build(RuleSetting ruleSetting)
        {
            LoadIntoCurrentAssembly(SerializeRuleFunctionsAndClasses(ruleSetting));
        }

        private string SerializeRuleFunctionsAndClasses(RuleSetting ruleSetting)
        {
            var ruleFunctionSerializer = new CodeModelSerializerFactory()
                .CreateCSharpSerializer(new RuleFunctionClassBuilder(ruleSetting.RuleFunction).ToModel())
            .Serialize();

            var ruleClassSerializer = new CodeModelSerializerFactory()
                .CreateCSharpSerializer(new RuleClassBuilder(ruleSetting, "ruleset1").ToModel())
            .Serialize();

            var inputObjectSerializer = string.Empty;

            //create input object for all categories...
            foreach (var category in ruleSetting.RuleDataCategory.Select(x => x.CategoryName))
            {
                if (ruleSetting.RuleModel.Where(x => x.CategoryName.Equals(category)).ToList().Count > 0)
                {
                    inputObjectSerializer += new CodeModelSerializerFactory()
                        .CreateCSharpSerializer(new InputObjectBuilder(ruleSetting.RuleModel, category).ToModel())
                        .Serialize();
                }
            }

            return string.Join(Environment.NewLine, ruleFunctionSerializer, ruleClassSerializer, inputObjectSerializer);
        }

        private static void LoadIntoCurrentAssembly(string code)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var assemblyName = "carenirvana.bre.engine.runtime";
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .ToList();

            var compilation = CSharpCompilation.Create(assemblyName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTree);

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                foreach (var diagnostic in result.Diagnostics)
                {
                    Console.WriteLine(diagnostic.ToString());
                }
                return;
            }

            ms.Seek(0, SeekOrigin.Begin);
            var context = new AssemblyLoadContext(null, true);
            context.LoadFromStream(ms);
        }
    }
}
