namespace carenirvanabre.codegenerator
{
    public interface ICodeMemberConstructor : ICodeMemberMethod
    {
        bool ShouldAddBaseConstructor { get; set; }
    }
}
