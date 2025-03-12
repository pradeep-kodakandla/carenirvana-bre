using carenirvana.bre.codegenerator;
using carenirvana.bre.codegenerator.Impl;
using carenirvana.bre.model;
using carenirvana.bre.model.Impl;
using carenirvana.bre.utility;
using static carenirvana.bre.codegenerator.EnumCollection;

namespace carenirvana.bre.codebuilder
{
    public class InputObjectBuilder(List<RuleModel> ruleModels, string inputTableName) : CodeModelBuilder
    {
        private readonly List<RuleModel> _ruleModels = ruleModels ?? throw new ArgumentNullException(nameof(ruleModels));
        private readonly string inputTableName = inputTableName ?? throw new ArgumentNullException(nameof(inputTableName));
        private const string PropertyReflector = "IPropertyReflector";
        private const string PropertyManager = "IPropertyManager";
        private const string IInputObject = "IInputObject";
        private const string GetReflector = "GetReflector";

        public override ICodeModel ToModel()
        {
            BuildInputObject();
            return CodeModel;
        }

        private void BuildInputObject()
        {
            AddClassWithName(inputTableName, false)
                .AddNamespace(ConstantsUtility.RunTimeInputDataTypeName)
                .AddNamespaceImports(["System", "System.Collections.Concurrent", "System.Reflection", "carenirvana.bre.utility.Extensions", "carenirvana.bre.model", "carenirvana.bre.model.Impl"])
                .AddBaseType(IInputObject)
                .AddBaseType(PropertyReflector)
                .AddField(new CodeMember("propertyManager", typeof(IPropertyManager).FullName, new[] { CodeModelAttribute.Private }, null))
                .AddMethod(new CodeMemberMethod(
                    GetReflector,
                    PropertyManager,
                    [CodeModelAttribute.Public, CodeModelAttribute.Final],
                    null,
                    "return propertyManager ??= new PropertyManager(this, properties);",
                    null))
                .AddProperty(NewCodeMemberProperty("id", "int"))
                .AddProperties(GetProperties())
                .AddField(new CodeMember(
                    "properties",
                    "ConcurrentDictionary<string, PropertyInfo>",
                    [CodeModelAttribute.Private, CodeModelAttribute.Static],
                    null,
                    $"typeof({inputTableName}).ToProperties()"));
        }

        private List<ICodeMemberProperty> GetProperties()
        {
            return _ruleModels
                .Select(NewCodeMemberProperty)
                .ToList();
        }

        private ICodeMemberProperty NewCodeMemberProperty(RuleModel ruleModel)
        {
            return new CodeMemberProperty(
                ruleModel.DataFieldName,
                HelperFunctions.GetTypeFromString(ruleModel.DataFieldType).FullName,
                [CodeModelAttribute.Public, CodeModelAttribute.Final],
                null,
                string.Empty,
                string.Empty);
        }

        private ICodeMemberProperty NewCodeMemberProperty(string propertyName, string dataType)
        {
            return new CodeMemberProperty(
                propertyName,
                HelperFunctions.GetTypeFromString(dataType).FullName,
                [CodeModelAttribute.Public, CodeModelAttribute.Final],
                null,
                string.Empty,
                string.Empty);
        }
    }
}