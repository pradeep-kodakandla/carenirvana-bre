using static carenirvanabre.codegenerator.EnumCollection;

namespace carenirvanabre.codegenerator.Impl
{
    public class CodeMember(string name, string type, EnumCollection.CodeModelAttribute[] codeModelAttribute, string columnName) : ICodeMember
    {
        public CodeMember(string name, string type, CodeModelAttribute[] codeModelAttribute, string columnName, string inlineIntializeExpression) : 
                this(name, type, codeModelAttribute, columnName)
        {
            InlineIntializeExpression = inlineIntializeExpression;
        }

        public string Name { get; set; } = name;

        public string Type { get; set; } = type;

        public CodeModelAttribute[] Attributes { get; set; } = codeModelAttribute;

        public string InlineIntializeExpression { get; set; }

        public string ColumnName { get; set; } = columnName;
    }
}
