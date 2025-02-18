namespace carenirvanabre.codegenerator
{
    public class EnumCollection
    {
        public enum CodeModelAttribute
        {
            Private,
            Public,
            Override,
            Final,
            Static,
            New
        }

        public enum CodeModelLanguage
        {
            CSharp,
            VisualBasic,
            JScript
        }

        public enum CodeModelType
        {
            Class,
            Interface,
            Enum,
            Struct,
            Delegate
        }

        public enum BracingStyle
        {
            C,
            Block
        }
    }
}
