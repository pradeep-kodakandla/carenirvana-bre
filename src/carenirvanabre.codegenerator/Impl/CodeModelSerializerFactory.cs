namespace carenirvanabre.codegenerator.Impl
{
    public class CodeModelSerializerFactory : ICodeModelSerializerFactory
    {
        public ICodeModelSerializer CreateCSharpSerializer(ICodeModel codeModel)
        {
            return new CodeModelSerializer(codeModel, EnumCollection.CodeModelLanguage.CSharp);
        }
    }
}
