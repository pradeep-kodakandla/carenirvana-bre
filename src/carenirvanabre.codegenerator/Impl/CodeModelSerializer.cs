using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection.Metadata;
using static carenirvanabre.codegenerator.EnumCollection;

namespace carenirvanabre.codegenerator.Impl
{
    public class CodeModelSerializer : ICodeModelSerializer
    {
        private const string PropertyGetSet = " { get; set; }//";
        private readonly ICodeModel codeModel;
        private readonly CodeModelLanguage language;

        public CodeModelSerializer(ICodeModel codeModel, CodeModelLanguage language)
        {
            this.codeModel = codeModel;
            this.language = language;
        }

        public string Serialize()
        {
            return GenerateCode();
        }

        public static string ColumnAttribName { get; } = "Column";
        public static string ColumnAttribArgumentName { get; } = "Name";

        public static string ColumnCodeCommentPrefix { get; } = "Maps to column";

        private string GenerateCode()
        {
            StringWriter writer = GenerateCodeAndGetWriter();
            return GetCodeAndCloseWriter(writer);
        }

        private StringWriter GenerateCodeAndGetWriter()
        {
            StringWriter writer = new();
            GetProvider().GenerateCodeFromCompileUnit(CreateCodeCompilationUnit(), writer, new CodeGeneratorOptions() { BracingStyle = BracingStyle.C.ToString() });
            return writer;
        }

        private string GetCodeAndCloseWriter(StringWriter writer)
        {
            string code = writer.GetStringBuilder().ToString();
            writer.Close();
            return code;
        }

        private CodeDomProvider GetProvider()
        {
            return CodeDomProvider.CreateProvider(language.ToString());
        }

        private CodeCompileUnit CreateCodeCompilationUnit()
        {
            CodeCompileUnit compileUnit = new();
            compileUnit.Namespaces.Add(CreateCodeNamespace(codeModel.NameSpace));
            return compileUnit;
        }

        private CodeNamespace CreateCodeNamespace(string nameSpace)
        {
            CodeNamespace codeNamespace = new(nameSpace);
            CreateNameSpaceImports(codeNamespace);
            AddTypeToCodeNameSpace(codeNamespace);
            return codeNamespace;
        }

        private void CreateNameSpaceImports(CodeNamespace codeNamespace)
        {
            foreach (string import in codeModel.NamespaceImports)
            {
                AddNameSpaceImportToCodeNameSpace(codeNamespace, import);
            }
        }

        private void AddNameSpaceImportToCodeNameSpace(CodeNamespace codeNamespace, string import)
        {
            codeNamespace.Imports.Add(CreateNameSpaceImport(import));
        }

        private CodeNamespaceImport CreateNameSpaceImport(string import)
        {
            return new CodeNamespaceImport(import);
        }

        private void AddTypeToCodeNameSpace(CodeNamespace codeNamespace)
        {
            codeNamespace.Types.Add(CreateType(codeModel));
        }

        private CodeTypeDeclaration CreateType(ICodeModel codeModel)
        {
            CodeTypeDeclaration codeType = CreateTypeInternal(codeModel);
            AddBaseTypes(codeType, codeModel);
            AddFields(codeType, codeModel);
            AddProperties(codeType, codeModel);
            AddMethods(codeType, codeModel);
            AddConstructors(codeType, codeModel);

            return codeType;
        }

        private CodeTypeDeclaration CreateTypeInternal(ICodeModel codeModel)
        {
            return new CodeTypeDeclaration
            {
                Name = codeModel.Name,
                IsClass = codeModel.IsClass,
                IsInterface = codeModel.IsInterface,
                IsPartial = codeModel.IsPartialClass,
                Attributes = GetCodeDomAttributes(new[] { codeModel.Attribute }),
                TypeAttributes = codeModel.TypeAttributes
            };
        }

        private MemberAttributes GetCodeDomAttributes(CodeModelAttribute[] attributes)
        {
            return attributes.Select(
                modifier => (MemberAttributes)Enum.Parse(typeof(MemberAttributes), modifier.ToString()))
                .Aggregate((firstModifier, secondModifier) => firstModifier | secondModifier);
        }

        private void AddBaseTypes(CodeTypeDeclaration codeType, ICodeModel codeModel)
        {
            codeType.BaseTypes.AddRange(CreateBaseTypes(codeModel));
        }

        private void AddFields(CodeTypeDeclaration codeType, ICodeModel codeModel)
        {
            codeType.Members.AddRange(CreateFields(codeModel));
        }

        private void AddProperties(CodeTypeDeclaration codeType, ICodeModel codeModel)
        {
            codeType.Members.AddRange(CreateProperties(codeModel));
        }

        private void AddMethods(CodeTypeDeclaration codeType, ICodeModel codeModel)
        {
            codeType.Members.AddRange(CreateMethods(codeModel));
        }

        private void AddConstructors(CodeTypeDeclaration codeType, ICodeModel codeModel)
        {
            foreach (var constructor in codeModel.Constructors)
            {
                codeType.Members.Add(AddConstructor(codeType, constructor));
            }
        }

        private CodeConstructor AddConstructor(CodeTypeDeclaration codeType, ICodeMemberConstructor constructor)
        {
            CodeConstructor codeConstructor = new()
            {
                Attributes = GetCodeDomAttributes(constructor.Attributes)
            };
            AddConstructorParameters(codeConstructor, constructor);
            return codeConstructor;
        }

        private void AddConstructorParameters(CodeConstructor codeConstructor, ICodeMemberConstructor constructor)
        {
            if (constructor.Parameters.Count > 0)
            {
                codeConstructor.Parameters.AddRange(CreateParameters(constructor.Parameters));

                foreach (var parameter in constructor.Parameters)
                {
                    if (constructor.MethodBody != string.Empty)
                    {
                        CodeFieldReferenceExpression constructorParameter = new(new CodeThisReferenceExpression(), parameter.Key);
                        codeConstructor.Statements.Add(new CodeAssignStatement(constructorParameter, new CodeVariableReferenceExpression(parameter.Key)));
                    }

                    if (constructor.ShouldAddBaseConstructor)
                    {
                        codeConstructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression(parameter.Key));
                    }
                }

            }
        }

        private CodeTypeReferenceCollection CreateBaseTypes(ICodeModel codeModel)
        {
            CodeTypeReferenceCollection items = new();
            foreach (string baseType in codeModel.BaseTypes)
            {
                items.Add(GetBaseType(baseType));
            }
            return items;
        }

        private CodeTypeReference GetBaseType(string baseType)
        {
            return new CodeTypeReference(baseType);
        }

        private CodeTypeMemberCollection CreateFields(ICodeModel codeModel)
        {
            CodeTypeMemberCollection items = new();
            foreach (ICodeMember field in codeModel.Fields)
            {
                items.Add(CreateField(field));
            }
            return items;
        }

        private CodeMemberField CreateField(ICodeMember codeMember)
        {
            CodeMemberField codeMemberField = new(codeMember.Type, codeMember.Name)
            {
                Attributes = GetCodeDomAttributes(codeMember.Attributes)
            };

            if (!string.IsNullOrWhiteSpace(codeMember.InlineIntializeExpression))
            {
                AddInitializeExpressionToCodeMemberField(codeMemberField, codeMember.InlineIntializeExpression);
            }
            return codeMemberField;
        }

        private void AddInitializeExpressionToCodeMemberField(CodeMemberField codeMemberField, string inlineIntializeExpression)
        {
            codeMemberField.Name += $" = {inlineIntializeExpression}";
        }

        private CodeTypeMemberCollection CreateProperties(ICodeModel codeModel)
        {
            CodeTypeMemberCollection items = new();
            foreach (ICodeMemberProperty property in codeModel.Properties)
            {
                items.Add(CreateProperty(property, codeModel.IsInterface));
            }
            return items;
        }

        private CodeTypeMember CreateProperty(ICodeMemberProperty codeMemberProperty, bool isInterface)
        {
            CodeTypeMember property = new();
            if (GetSetStatementsExists(codeMemberProperty) || isInterface)
            {
                property = CreatePropertyWithGetSetStatements(codeMemberProperty);
                AddHasGetAndHasSetIfInterface(isInterface, (System.CodeDom.CodeMemberProperty)property);
            }
            else
            {
                property = CreatePropertyWithoutGetSetStatements(codeMemberProperty);
            }
            return property;
        }

        private void AddHasGetAndHasSetIfInterface(bool isInterface, System.CodeDom.CodeMemberProperty property)
        {
            if (isInterface)
            {
                property.HasGet = true;
                property.HasSet = true;
            }
        }

        private CodeMemberField CreatePropertyWithoutGetSetStatements(ICodeMemberProperty codeMemberProperty)
        {
            var property = new CodeMemberField(codeMemberProperty.Type, codeMemberProperty.Name)
            {
                Attributes = GetCodeDomAttributes(codeMemberProperty.Attributes)
            };
            if (codeMemberProperty.ColumnName != null)
            {
                property.CustomAttributes.Add(GetColumnAttributeDeclaration(codeMemberProperty));
                property.Comments.AddRange(GetCodeCommentsForMappedColumn(codeMemberProperty));
            }

            AppendGetSetToPropertyName(property);
            return property;
        }

        private CodeAttributeDeclaration GetColumnAttributeDeclaration(ICodeMemberProperty codeMemberProperty)
        {
            CodeAttributeArgument codeAttr = new CodeAttributeArgument(ColumnAttribArgumentName, new CodePrimitiveExpression(codeMemberProperty.ColumnName));
            CodeAttributeDeclaration codeAttrDecl = new CodeAttributeDeclaration(ColumnAttribName, codeAttr);

            return codeAttrDecl;
        }

        private CodeCommentStatementCollection GetCodeCommentsForMappedColumn(ICodeMemberProperty codeMemberProperty)
        {
            CodeCommentStatementCollection collection = new CodeCommentStatementCollection
            {
                new CodeCommentStatement("<summary>", true),
                new CodeCommentStatement($"{ColumnCodeCommentPrefix} {codeMemberProperty.ColumnName}", true),
                new CodeCommentStatement("</summary>", true),
            };
            return collection;
        }

        private System.CodeDom.CodeMemberProperty CreatePropertyWithGetSetStatements(ICodeMemberProperty codeMemberProperty)
        {
            System.CodeDom.CodeMemberProperty property = new()
            {
                Name = codeMemberProperty.Name,
                Type = GetCodeDomType(codeMemberProperty.Type),
                Attributes = GetCodeDomAttributes(codeMemberProperty.Attributes)
            };

            AddGetStatement(property, codeMemberProperty.GetStatementBody);
            AddSetStatement(property, codeMemberProperty.SetStatementBody);

            return property;
        }

        private void AddGetStatement(System.CodeDom.CodeMemberProperty property, string getStatementBody)
        {
            if (!string.IsNullOrWhiteSpace(getStatementBody))
            {
                property.GetStatements.Add(CreateCodeSnippetStatement(getStatementBody));
            }
        }
        private void AddSetStatement(System.CodeDom.CodeMemberProperty property, string setStatementBody)
        {
            if (!string.IsNullOrWhiteSpace(setStatementBody))
            {
                property.SetStatements.Add(CreateCodeSnippetStatement(setStatementBody));
            }
        }

        private void AppendGetSetToPropertyName(CodeMemberField property)
        {
            property.Name += PropertyGetSet;
        }

        private bool GetSetStatementsExists(ICodeMemberProperty codeMemberProperty)
        {
            return !(string.IsNullOrWhiteSpace(codeMemberProperty.GetStatementBody) && !string.IsNullOrWhiteSpace(codeMemberProperty.SetStatementBody));
        }

        private CodeSnippetStatement CreateCodeSnippetStatement(string codeSnippet)
        {
            return new CodeSnippetStatement(codeSnippet);
        }

        private CodeTypeMemberCollection CreateMethods(ICodeModel codeModel)
        {
            CodeTypeMemberCollection items = new();
            foreach (ICodeMemberMethod method in codeModel.Methods)
            {
                items.Add(CreateMethod(method));
            }
            return items;
        }

        private System.CodeDom.CodeMemberMethod CreateMethod(ICodeMemberMethod method)
        {
            System.CodeDom.CodeMemberMethod codeMethod = GetMethodInternal(method);
            AddMethodBody(codeMethod, method.MethodBody);
            AddMethodParameters(codeMethod, method.Parameters);

            return codeMethod;
        }

        private System.CodeDom.CodeMemberMethod GetMethodInternal(ICodeMemberMethod codeMemberMethod)
        {
            return new System.CodeDom.CodeMemberMethod
            {
                Name = codeMemberMethod.Name,
                Attributes = GetCodeDomAttributes(codeMemberMethod.Attributes),
                ReturnType = GetCodeDomType(codeMemberMethod.Type)
            };
        }

        private void AddMethodBody(System.CodeDom.CodeMemberMethod codeMemberMethod, string methodBody)
        {
            if (!string.IsNullOrWhiteSpace(methodBody))
            {
                codeMemberMethod.Statements.Add(CreateCodeSnippetStatement(methodBody));
            }
        }

        private void AddMethodParameters(System.CodeDom.CodeMemberMethod codeMemberMethod, IDictionary<string, Type> parameters)
        {
            if (parameters != null)
            {
                codeMemberMethod.Parameters.AddRange(CreateParameters(parameters));
            }
        }

        private CodeParameterDeclarationExpressionCollection CreateParameters(IDictionary<string, Type> parameters)
        {
            CodeParameterDeclarationExpressionCollection items = new();
            foreach(KeyValuePair<string, Type> parameter in parameters)
            {
                items.Add(CreateParameter(parameter.Key, parameter.Value.FullName));
            }

            return items;
        }

        private CodeParameterDeclarationExpression CreateParameter(string name, string type)
        {
            return new CodeParameterDeclarationExpression(GetCodeDomType(type), name);
        }

        private CodeTypeReference GetCodeDomType(string type)
        {
            return new CodeTypeReference(type);
        }
    }
}
