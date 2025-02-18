using static carenirvanabre.codegenerator.EnumCollection;

namespace carenirvanabre.codegenerator
{
    public interface ICodeMember
    {
        string Name { get; set; }

        string Type { get; set; }

        CodeModelAttribute[] Attributes { get; set; }

        string InlineIntializeExpression { get; set; }

        string ColumnName { get; set; }
    }
}
