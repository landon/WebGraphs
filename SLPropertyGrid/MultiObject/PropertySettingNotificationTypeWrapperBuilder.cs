using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SLPropertyGrid.MultiObject
{
    public static class PropertySettingNotificationTypeWrapperBuilder
    {
        public static object CreateWrapper(object o)
        {
            return CreateObjectWithProperties(o.GetType().GetProperties().Select(pi => new Tuple<string, Type>(pi.Name, pi.PropertyType)));
        }

        public static object CreateObjectWithProperties(IEnumerable<Tuple<string, Type>> properties)
        {
            return Activator.CreateInstance(CreateTypeWithProperties(properties));
        }

        public static Type CreateTypeWithProperties(IEnumerable<Tuple<string, Type>> properties)
        {
            var typeBuilder = GetTypeBuilder();
            var constructor = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            var valueChangedBuilder = typeBuilder.DefineField("ValueChanged", typeof(Action<string>), FieldAttributes.Public);

            foreach (var tuple in properties)
                CreateProperty(typeBuilder, tuple.Item1, tuple.Item2, valueChangedBuilder);

            return typeBuilder.CreateType();
        }

        static TypeBuilder GetTypeBuilder()
        {
            return AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run)
                            .DefineDynamicModule("ModularlyModulatedModule")
                            .DefineType(Guid.NewGuid().ToString(), TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoLayout, null);
        }

        static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType, FieldBuilder valueChangedBuilder)
        {
            var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            
            var getBuilder = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            var getILGenerator = getBuilder.GetILGenerator();
            getILGenerator.Emit(OpCodes.Ldarg_0);
            getILGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            getILGenerator.Emit(OpCodes.Ret);

            var setBuilder = typeBuilder.DefineMethod("set_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { propertyType });
            var setILGenerator = setBuilder.GetILGenerator();
            setILGenerator.Emit(OpCodes.Ldarg_0);
            setILGenerator.Emit(OpCodes.Ldarg_1);
            setILGenerator.Emit(OpCodes.Stfld, fieldBuilder);

            setILGenerator.Emit(OpCodes.Ldarg_0);
            setILGenerator.Emit(OpCodes.Ldfld, valueChangedBuilder);
            setILGenerator.Emit(OpCodes.Ldstr, propertyName);
            setILGenerator.Emit(OpCodes.Call, typeof(Action<string>).GetMethod("Invoke"));
            setILGenerator.Emit(OpCodes.Ret);

            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            propertyBuilder.SetGetMethod(getBuilder);
            propertyBuilder.SetSetMethod(setBuilder);
        }
    }
}
