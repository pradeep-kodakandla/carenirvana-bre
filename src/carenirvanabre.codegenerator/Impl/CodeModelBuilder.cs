
namespace carenirvanabre.codegenerator.Impl
{
    public abstract class CodeModelBuilder : ICodeModelBuilder
    {
        private bool isClass = true;

        protected CodeModelBuilder()
        {
            CodeModel = new CodeModel();
        }

        protected ICodeModel CodeModel { get; }

        public bool IsClass
        {
            get { return isClass; }
            set { CodeModel.IsClass = isClass = value; }
        }

        public bool IsInterface { get; private set; }

        public bool IsPartialClass { get; private set; }

        public ICodeModelBuilder AddAttribute(EnumCollection.CodeModelAttribute codeModelAttribute)
        {
            CodeModel.Attribute = codeModelAttribute;
            return this;
        }

        public ICodeModelBuilder AddBaseType(string baseType)
        {
            CodeModel.BaseTypes.Add(baseType);
            return this;
        }

        public ICodeModelBuilder AddBaseTypes(string[] baseTypes)
        {
            foreach(var baseType in baseTypes)
            {
                AddBaseType(baseType);
            }
            return this;
        }

        public ICodeModelBuilder AddClassWithName(string name, bool isPartialClass)
        {
            AddName(name);
            SetIsClass(true);
            SetIsPartialClass(isPartialClass);
            return this;
        }

        public ICodeModelBuilder AddConstructor(ICodeMemberConstructor constructor)
        {
            CodeModel.Constructors.Add(new CodeMemberConstructor(constructor.Name, constructor.Type, constructor.Attributes, constructor.ColumnName, constructor.MethodBody, constructor.Parameters, constructor.ShouldAddBaseConstructor));
            return this;
        }

        public ICodeModelBuilder AddConstructors(IList<ICodeMemberConstructor> codeMemberConstructors)
        {
            foreach (var constructor in codeMemberConstructors)
            {
                AddConstructor(constructor);
            }
            return this;
        }

        public ICodeModelBuilder AddField(ICodeMember codeMemberField)
        {
            CodeModel.Fields.Add(AddMember(codeMemberField));
            return this;
        }

        public ICodeModelBuilder AddFields(IList<ICodeMember> codeMemberFields)
        {
            foreach(var field in codeMemberFields)
            {
                AddField(field);
            }
            return this;
        }

        public ICodeModelBuilder AddInterfaceWithName(string name)
        {
            AddName(name);
            SetIsClass(true);
            SetIsInterface(true);
            return this;
        }

        public ICodeModelBuilder AddMethod(ICodeMemberMethod codeMemberMethod)
        {
            CodeModel.Methods.Add(new CodeMemberMethod(codeMemberMethod.Name, codeMemberMethod.Type, codeMemberMethod.Attributes, codeMemberMethod.ColumnName, codeMemberMethod.MethodBody, codeMemberMethod.Parameters));
            return this;
        }

        public ICodeModelBuilder AddMethods(IList<ICodeMemberMethod> codeMemberMethods)
        {
            foreach(var method in codeMemberMethods)
            {
                AddMethod(method);
            }
            return this;
        }

        public ICodeModelBuilder AddNamespace(string name)
        {
            CodeModel.NameSpace = name;
            return this;
        }

        public ICodeModelBuilder AddNamespaceImport(string namespaceImport)
        {
            CodeModel.NamespaceImports.Add(namespaceImport);
            return this;
        }

        public ICodeModelBuilder AddNamespaceImports(string[] namespaceImports)
        {
            foreach(var namespaceImport in namespaceImports)
            {
                AddNamespaceImport(namespaceImport);
            }
            return this;
        }

        public ICodeModelBuilder AddProperties(IList<ICodeMemberProperty> codeMemberProperties)
        {
            foreach(var property in codeMemberProperties)
            {
                AddProperty(property);
            }
            return this;
        }

        public ICodeModelBuilder AddProperty(ICodeMemberProperty codeMemberProperty)
        {
            CodeModel.Properties.Add(AddMemberProperty(codeMemberProperty));
            return this;
        }

        public abstract ICodeModel ToModel();

        private void AddName(string name)
        {
            CodeModel.Name = name;
        }

        private void SetIsClass(bool value)
        {
            CodeModel.IsClass = value;
        }

        private void SetIsInterface(bool value)
        {
            CodeModel.IsInterface = value;
        }

        private void SetIsPartialClass(bool value)
        {
            CodeModel.IsPartialClass = value;
        }

        private ICodeMember AddMember(ICodeMember codeMember)
        {
            return new CodeMember(codeMember.Name, codeMember.Type, codeMember.Attributes, codeMember.InlineIntializeExpression, codeMember.ColumnName);
        }

        private ICodeMemberProperty AddMemberProperty(ICodeMemberProperty codeMemberProperty)
        {
            return new CodeMemberProperty(codeMemberProperty.Name, codeMemberProperty.Type, codeMemberProperty.Attributes, codeMemberProperty.ColumnName, codeMemberProperty.GetStatementBody, codeMemberProperty.SetStatementBody);
        }
    }
}
