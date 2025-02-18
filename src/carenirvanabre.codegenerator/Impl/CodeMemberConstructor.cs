namespace carenirvanabre.codegenerator.Impl
{
    public class CodeMemberConstructor : CodeMemberMethod, ICodeMemberConstructor
    {
        public CodeMemberConstructor(string name, string type, EnumCollection.CodeModelAttribute[] codeModelAttribute, string columnName, string methodBody, IDictionary<string, Type> parameters, bool shouldAddBaseConstructor) :
                base(name, type, codeModelAttribute, columnName, methodBody, parameters)
        {
            ShouldAddBaseConstructor = shouldAddBaseConstructor;
        }

        public bool ShouldAddBaseConstructor { get; set; }
    }
}
