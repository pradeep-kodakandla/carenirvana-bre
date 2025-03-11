namespace carenirvana.bre.codegenerator
{
    public interface ICodeModelSerializerFactory
    {
        ICodeModelSerializer CreateCSharpSerializer(ICodeModel codeModel);
    }
}
