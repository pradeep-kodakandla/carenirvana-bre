using Moq;
using System.Data;

namespace carenirvana.bre.common.Tests
{
    [TestFixture]
    public class ObjectFactoryTests
    {
        [Test]
        public void CreateInstance_ShouldCreateInstanceOfType()
        {
            // Arrange
            var factory = new ObjectFactory.Impl.ObjectFactory();
            var type = typeof(Mock<IDataReader>);

            // Act
            var instance = factory.CreateInstance(type);

            // Assert
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.InstanceOf<Mock<IDataReader>>());
        }

        [Test]
        public void GetInstanceOfAnObject_ShouldCreateInstanceOfSpecifiedType()
        {
            // Arrange
            var factory = new ObjectFactory.Impl.ObjectFactory();
            var assemblyName = "mscorlib";
            var nameSpace = "System";
            var typeName = "String";

            // Act
            var instance = factory.GetInstanceOfAnObject(assemblyName, nameSpace, typeName);

            // Assert
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.InstanceOf<string>());
        }
    }
}
