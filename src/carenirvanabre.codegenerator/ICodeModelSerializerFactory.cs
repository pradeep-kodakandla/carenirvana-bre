namespace carenirvanabre.codegenerator
{
    public interface ICodeModelSerializerFactory
    {
        ICodeModelSerializer CreateCSharpSerializer(ICodeModel codeModel);
    }
}
