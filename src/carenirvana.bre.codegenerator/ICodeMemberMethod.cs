namespace carenirvana.bre.codegenerator
{
    public interface ICodeMemberMethod : ICodeMember
    {
        string MethodBody { get; set; }

        IDictionary<string, Type> Parameters { get; set; }
    }
}
