using System.Reflection;

namespace carenirvanabre.codegenerator.Impl
{
    public class CodeModel : ICodeModel
    {
        public CodeModel() 
        {
            BaseTypes = [];
            NamespaceImports = [];
            Properties = [];
            Fields = [];
            Methods = [];
            Constructors = [];
        }

        public string Name { get; set; }
        public string NameSpace { get; set; }
        public EnumCollection.CodeModelAttribute Attribute { get; set; }
        public IList<string> BaseTypes { get; set; }
        public IList<string> NamespaceImports { get; set; }
        public bool IsClass { get; set; }
        public bool IsPartialClass { get; set; }
        public bool IsInterface { get; set; }
        public IList<ICodeMemberProperty> Properties { get; set; }
        public IList<ICodeMember> Fields { get; set; }
        public IList<ICodeMemberMethod> Methods { get; set; }
        public IList<ICodeMemberConstructor> Constructors { get; set; }
        public TypeAttributes TypeAttributes { get; set; }
    }
}
