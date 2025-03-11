using carenirvana.bre.utility;

namespace carenirvana.bre.model
{
    public interface IPropertyManager
    {
        string Name { get; }

        bool SetValue(string propertyName, object value);

        bool SetValue(string propertyName, object value, DataType dataType);

        object GetValue(string propertyName);

        object GetValue(string propertyName, DataType dataType);

        T GetValue<T>(string propertyName);

        void CopyValuesFrom(IPropertyManager source);
    }
}
