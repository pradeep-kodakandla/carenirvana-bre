namespace carenirvana.bre.codegenerator
{
    public interface ICodeGenerator
    {
        void GenerateCode();

        void GenerateCode(IList<string> ruleNames);

        void GenerateCode(string ruleName);
    }
}
