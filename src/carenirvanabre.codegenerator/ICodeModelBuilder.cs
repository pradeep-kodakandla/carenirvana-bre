using static carenirvanabre.codegenerator.EnumCollection;

namespace carenirvanabre.codegenerator
{
    public interface ICodeModelBuilder
    {
        bool IsClass { get; }

        bool IsInterface { get; }

        bool IsPartialClass { get; }

        ICodeModelBuilder AddClassWithName(string name, bool isPartialClass);

        ICodeModelBuilder AddInterfaceWithName(string name);

        ICodeModelBuilder AddNamespace(string name);

        ICodeModelBuilder AddNamespaceImports(string[] namespaceImports);

        ICodeModelBuilder AddNamespaceImport(string namespaceImport);

        ICodeModelBuilder AddBaseTypes(string[] baseTypes);

        ICodeModelBuilder AddBaseType(string baseType);

        ICodeModelBuilder AddAttribute(CodeModelAttribute codeModelAttribute);

        ICodeModelBuilder AddProperties(IList<ICodeMemberProperty> codeMemberProperties);

        ICodeModelBuilder AddProperty(ICodeMemberProperty codeMemberProperty);

        ICodeModelBuilder AddFields(IList<ICodeMember> codeMemberFields);

        ICodeModelBuilder AddField(ICodeMember codeMemberField);

        ICodeModelBuilder AddMethods(IList<ICodeMemberMethod> codeMemberMethods);

        ICodeModelBuilder AddMethod(ICodeMemberMethod codeMemberMethod);

        ICodeModelBuilder AddConstructors(IList<ICodeMemberConstructor> codeMemberConstructors);

        ICodeModelBuilder AddConstructor(ICodeMemberConstructor codeMemberConstructor);

        ICodeModel ToModel();
    }
}
