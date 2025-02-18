namespace carenirvanabre.codegenerator.Impl
{
    public class CodeMemberProperty(string name, string type, EnumCollection.CodeModelAttribute[] codeModelAttribute, string columnName, string getStatementBody, string setStatementBody) : CodeMember(name, type, codeModelAttribute, columnName), ICodeMemberProperty
    {
        public string GetStatementBody { get; set; } = getStatementBody;

        public string SetStatementBody { get; set; } = setStatementBody;
    }
}
