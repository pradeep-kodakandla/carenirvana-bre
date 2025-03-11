namespace carenirvana.bre.codegenerator
{
    public interface ICodeMemberProperty : ICodeMember
    {
        string GetStatementBody { get; set; }

        string SetStatementBody { get; set; }
    }
}
