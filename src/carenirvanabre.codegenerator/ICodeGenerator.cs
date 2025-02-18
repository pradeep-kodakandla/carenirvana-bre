namespace carenirvanabre.codegenerator
{
    public interface ICodeGenerator
    {
        void GenerateCode();

        void GenerateCode(IList<string> ruleNames);

        void GenerateCode(string ruleName);
    }
}
