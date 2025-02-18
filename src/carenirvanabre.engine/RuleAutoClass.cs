using System.Reflection;
using System.Reflection.Emit;

namespace carenirvanabre.engine
{
    internal class RuleAutoClass
    {

    }

    internal class RuleFunction
    {
        public static object MemberAge(DateTime datafieldvalue)
        {
            return DateTime.Now.Subtract(datafieldvalue);
        }
    }

    internal class DynamicClassExample
    {
        public static void Main()
        {
            // Define a dynamic assembly and module
            AssemblyName assemblyName = new("DynamicAssembly");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

            // Define a public class named "MyDynamicClass" in the assembly
            TypeBuilder typeBuilder = moduleBuilder.DefineType("MyDynamicClass", TypeAttributes.Public);

            // Define a public string field named "DynamicField"
            FieldBuilder fieldBuilder = typeBuilder.DefineField("DynamicField", typeof(string), FieldAttributes.Public);

            // Create the type
            Type dynamicType = typeBuilder.CreateType();

            // Instantiate the dynamic class
            object dynamicInstance = Activator.CreateInstance(dynamicType);

            // Set the value of the field
            dynamicType.GetField("DynamicField").SetValue(dynamicInstance, "Hello, Dynamic World!");

            // Get the value of the field
            string fieldValue = (string)dynamicType.GetField("DynamicField").GetValue(dynamicInstance);

            // Print the value
            Console.WriteLine(fieldValue);
        }
    }

    public class AllRules
    {
        // variables...
        private const string var1 = "var1";

        // functions...
        private bool func1()
        {
            return true;
        }

        public void Rule1()
        {

        }
    }


    // input data (json, db connect, csv file)
    // global data unit - mapping between a rule field and data field
    // repository - data source
    // distributed batch manager -- distributing the data in batches (10k)
    // pass the data to the rule engine assembly (complex calculations) -- TPL
    // output generated - for each rule for each data element - add something/update something/delete something
    // rule engine as service
    // soft rules // batch rules
    // cache the data
}
