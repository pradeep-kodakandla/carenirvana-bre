using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using carenirvanabre.engine.Models;
using Newtonsoft.Json;

namespace carenirvanabre.engine
{
    public class BreAssemblyBuilder
    {
        public void BuildAssembly(RuleSetting ruleSetting)
        {
            var assemblyName = new AssemblyName("carenirvana.engine.dynamic");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            // Define classes
            TypeBuilder typeBuilder = moduleBuilder.DefineType("RuleEngineCalc", TypeAttributes.Public | TypeAttributes.Class);

            // Define fields
            foreach(var variable in ruleSetting.RuleVariable)
            {
                FieldBuilder fieldBuilder = typeBuilder.DefineField(variable.VariableName, typeof(string), FieldAttributes.Public|FieldAttributes.Static|FieldAttributes.Literal);
                fieldBuilder.SetConstant(variable.VariableValue);
            }

            var mytype = typeBuilder.CreateType();

        }

        private static Type CreateClass(ModuleBuilder moduleBuilder, string className, Dictionary<string, Type> properties)
        {
            TypeBuilder typeBuilder = moduleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class);

            foreach (var property in properties)
            {
                FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + property.Key, property.Value, FieldAttributes.Private);
                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(property.Key, PropertyAttributes.HasDefault, property.Value, null);

                MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + property.Key, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, property.Value, Type.EmptyTypes);
                ILGenerator getIL = getMethodBuilder.GetILGenerator();
                getIL.Emit(OpCodes.Ldarg_0);
                getIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getIL.Emit(OpCodes.Ret);

                MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_" + property.Key, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { property.Value });
                ILGenerator setIL = setMethodBuilder.GetILGenerator();
                setIL.Emit(OpCodes.Ldarg_0);
                setIL.Emit(OpCodes.Ldarg_1);
                setIL.Emit(OpCodes.Stfld, fieldBuilder);
                setIL.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getMethodBuilder);
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }

            return typeBuilder.CreateType();
        }
    }
}
