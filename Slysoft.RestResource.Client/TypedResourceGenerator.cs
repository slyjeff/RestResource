using System;
using System.Reflection;
using System.Reflection.Emit;
using Slysoft.RestResource.Client.Utils;

namespace Slysoft.RestResource.Client; 

internal abstract class TypedResourceGenerator {
    private static ModuleBuilder? _moduleBuilder;

    protected static ModuleBuilder ModuleBuilder {
        get {
            if (_moduleBuilder != null) {
                return _moduleBuilder;
            }

            var assemblyName = new AssemblyName { Name = "ResourceAccessor" };
            _moduleBuilder = AssemblyBuilder
                .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
                .DefineDynamicModule(assemblyName.Name);

            return _moduleBuilder;
        }
    }
}

internal sealed class TypedResourceGenerator<T> : TypedResourceGenerator where T : class {
    private readonly TypeBuilder _typeBuilder;

    public TypedResourceGenerator() {
        try {
            var interfaceType = typeof(T);

#if NET6_0_OR_GREATER
            var typeName = string.Concat(interfaceType.Name.AsSpan(1), "ResourceAccessor");
#else 
            var typeName = string.Concat(interfaceType.Name.Substring(1), "ResourceAccessor");
#endif
            _typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public, typeof(ResourceAccessor), new[] { interfaceType });
        } catch (Exception e) {
            throw new CreateAccessorException($"Error defining type based in on interface {typeof(T)}", e);
        }
    }

    public Type GeneratedType() {
        try {
            AddConstructor();
            AddProperties();

            var generatedType = _typeBuilder.CreateType();
            return generatedType ?? throw new CreateAccessorException(
                $"Error generating accessor based in on interface {typeof(T)}: TypeBuilder returned null");
        } catch (CreateAccessorException) {
            throw;
        } catch (Exception e) {
            throw new CreateAccessorException($"Error generating accessor based in on interface {typeof(T)}", e);
        }
    }

    private void AddConstructor() {
        Type[] constructorArgs = { typeof(Resource) };
        var constructorBuilder = _typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructorArgs);

        var codeGenerator = constructorBuilder.GetILGenerator();

        if (_typeBuilder.BaseType == null) {
            throw new CreateAccessorException($"BaseType of TypeBuilder is null when generating accessor based in on interface {typeof(T)}");
        }

        var baseCtor = _typeBuilder.BaseType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, constructorArgs, null);
        if (baseCtor == null) {
            throw new CreateAccessorException($"Base Constructor not found when generating accessor based in on interface {typeof(T)}");
        }

        codeGenerator.Emit(OpCodes.Ldarg_0);        //push 'this' onto the stack
        codeGenerator.Emit(OpCodes.Ldarg_1);        //push resource onto the stack
        codeGenerator.Emit(OpCodes.Call, baseCtor); //call the base constructor
        codeGenerator.Emit(OpCodes.Ret);            //return
    }

    private void AddProperties() {
        foreach (var property in typeof(T).GetAllProperties()) {
            AddProperty(property);
        }
    }

    private void AddProperty(PropertyInfo property) {
        var methodBuilder = _typeBuilder.DefineMethod("get_" + property.Name, MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.Standard | CallingConventions.HasThis, property.PropertyType, Type.EmptyTypes);

        if (_typeBuilder.BaseType == null) {
            throw new CreateAccessorException($"BaseType of TypeBuilder is null when generating accessor based in on interface {typeof(T)}");
        }

        var getDataMethod = _typeBuilder.BaseType.GetMethod("GetData", BindingFlags.Instance | BindingFlags.NonPublic, null, new [] { typeof(string)}, null);
        if (getDataMethod == null) {
            throw new CreateAccessorException($"Could not find 'GetData' method when generating based in on interface {typeof(T)}");
        }

        var typedGetDataMethod = getDataMethod.MakeGenericMethod(property.PropertyType);

        var codeGenerator = methodBuilder.GetILGenerator();

        codeGenerator.Emit(OpCodes.Ldarg_0);                       //push 'this' onto the stack
        codeGenerator.Emit(OpCodes.Ldstr, property.Name);          //push the name of the property onto the stack
        codeGenerator.Emit(OpCodes.Callvirt, typedGetDataMethod);  //call the the "GetData" method
        codeGenerator.Emit(OpCodes.Ret);                           //return

        var propertyBuilder = _typeBuilder.DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, null);
        propertyBuilder.SetGetMethod(methodBuilder);
    }
}