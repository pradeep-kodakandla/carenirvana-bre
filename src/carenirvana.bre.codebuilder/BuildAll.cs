using System.Runtime.Loader;
using carenirvana.bre.codegenerator.Impl;
using carenirvana.bre.model.Impl;
using carenirvana.bre.utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace carenirvana.bre.codebuilder
{
    public class BuildAll()
    {
        public void Build(RuleSetting ruleSetting, string inputTableName)
        {
            LoadIntoCurrentAssembly(SerializeRuleFunctionsAndClasses(ruleSetting, inputTableName));
        }

        private string SerializeRuleFunctionsAndClasses(RuleSetting ruleSetting, string inputTableName)
        {
            var ruleFunctionSerializer = new CodeModelSerializerFactory()
                .CreateCSharpSerializer(new RuleFunctionClassBuilder(ruleSetting.RuleFunction).ToModel())
            .Serialize();

            var ruleClassSerializer = new CodeModelSerializerFactory()
                .CreateCSharpSerializer(new RuleClassBuilder(ruleSetting, "realtime").ToModel())
            .Serialize();

            var inputObjectSerializer = string.Empty;

            inputObjectSerializer += new CodeModelSerializerFactory()
                .CreateCSharpSerializer(new InputObjectBuilder(ruleSetting.RuleModel, inputTableName).ToModel())
                .Serialize();

            return string.Join(Environment.NewLine, ruleFunctionSerializer, ruleClassSerializer, inputObjectSerializer);
        }

        private static void LoadIntoCurrentAssembly(string code)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .ToList();

            var compilation = CSharpCompilation.Create(ConstantsUtility.RunTimeNameSpace)
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
