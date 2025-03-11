namespace carenirvana.bre.codegenerator
{
    public interface ICodePersister
    {
        bool Persist(string nameOfItem, string contents);
    }
}
