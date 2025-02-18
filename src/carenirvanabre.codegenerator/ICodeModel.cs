using System.Reflection;
using static carenirvanabre.codegenerator.EnumCollection;

namespace carenirvanabre.codegenerator
{
    public interface ICodeModel
    {
        string Name { get; set; }

        string NameSpace { get; set; }

        CodeModelAttribute Attribute { get; set; }

        IList<string> BaseTypes { get; set; }

        IList<string> NamespaceImports { get; set; }

        bool IsClass { get; set; }

        bool IsPartialClass { get; set; }

        bool IsInterface { get; set; }

        IList<ICodeMemberProperty> Properties { get; set; }

        IList<ICodeMember> Fields { get; set; }

        IList<ICodeMemberMethod> Methods { get; set; }

        IList<ICodeMemberConstructor> Constructors { get; set; }

        TypeAttributes TypeAttributes { get; set; }
    }
}
