namespace carenirvana.bre.codegenerator
{
    public interface ICodeMemberConstructor : ICodeMemberMethod
    {
        bool ShouldAddBaseConstructor { get; set; }
    }
}
