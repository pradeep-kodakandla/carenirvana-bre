namespace carenirvanabre.codegenerator
{
    public interface ICodePersister
    {
        bool Persist(string nameOfItem, string contents);
    }
}
