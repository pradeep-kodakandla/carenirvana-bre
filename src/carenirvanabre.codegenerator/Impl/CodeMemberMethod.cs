
namespace carenirvanabre.codegenerator.Impl
{
    public class CodeMemberMethod(string name, string type, EnumCollection.CodeModelAttribute[] codeModelAttribute, string columnName, string methodBody) : CodeMember(name, type, codeModelAttribute, columnName), ICodeMemberMethod
    {
        public CodeMemberMethod(string name, string type, EnumCollection.CodeModelAttribute[] codeModelAttribute, string columnName, string methodBody, IDictionary<string, Type> parameters) :
                this(name, type, codeModelAttribute, columnName, methodBody)
        {
            Parameters = parameters;
        }

        public string MethodBody { get; set; } = methodBody;

        public IDictionary<string, Type> Parameters { get; set; }
    }
}
