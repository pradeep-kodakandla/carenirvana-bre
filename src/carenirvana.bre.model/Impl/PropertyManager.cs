using carenirvana.bre.model;
using carenirvana.bre.utility;
using System.Collections.Concurrent;
using System.Reflection;

namespace carenirvana.bre.model.Impl
{
    public class PropertyManager : IPropertyManager
    {
        private readonly ConcurrentDictionary<string, PropertyInfo> _properties;
        private readonly object _instance;
        private readonly Type _instanceType;

        public PropertyManager(object instance, ConcurrentDictionary<string, PropertyInfo> properties)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _properties = properties ?? throw new ArgumentNullException(nameof(properties));
            _instanceType = instance.GetType();
            Name = _instanceType.Name;
        }

        public string Name { get; }

        public bool SetValue(string propertyName, object value)
        {
            return SetPropertyValue(propertyName, value);
        }

        public bool SetValue(string propertyName, object value, DataType dataType)
        {
            return SetPropertyValue(propertyName, value);
        }

        public object GetValue(string propertyName)
        {
            return GetPropertyValue(propertyName);
        }

        public object GetValue(string propertyName, DataType dataType)
        {
            return GetPropertyValue(propertyName);
        }

        public T GetValue<T>(string propertyName)
        {
            if (!_properties.TryGetValue(propertyName, out var propertyInfo))
            {
                return default;
            }
            return (T)propertyInfo.GetValue(_instance);
        }

        public void CopyValuesFrom(IPropertyManager source)
        {
            foreach (var property in _properties)
            {
                var value = source.GetValue(property.Key);
                SetValue(property.Key, value);
            }
        }

        private bool SetPropertyValue(string propertyName, object value)
        {
            if (!_properties.TryGetValue(propertyName, out var propertyInfo))
            {
                return false;
            }
            propertyInfo.SetValue(_instance, value);
            return true;
        }

        private object GetPropertyValue(string propertyName)
        {
            if (!_properties.TryGetValue(propertyName, out var propertyInfo))
            {
                return null;
            }
            return propertyInfo.GetValue(_instance);
        }
    }
}