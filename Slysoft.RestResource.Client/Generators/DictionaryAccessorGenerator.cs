using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Slysoft.RestResource.Client.Accessors;
using Slysoft.RestResource.Client.Utils;

namespace Slysoft.RestResource.Client.Generators;

internal sealed class DictionaryAccessorGenerator {
    private readonly Type _interfaceType;
    private static ModuleBuilder? _moduleBuilder;
    private readonly TypeBuilder _typeBuilder;

    public DictionaryAccessorGenerator(Type interfaceType) {
        _interfaceType = interfaceType;

        if (!interfaceType.IsInterface) {
            throw new CreateAccessorException($"Dictionary Accessor must be based on an interface. {_interfaceType.Name} is not an interface");
        }

        try {
#if NET6_0_OR_GREATER
            var typeName = string.Concat(interfaceType.Name.AsSpan(1), "DictionaryAccessor");
#else 
            var typeName = string.Concat(interfaceType.Name.Substring(1), "ResourceAccessor");
#endif
            _typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public, typeof(DictionaryAccessor), new[] { interfaceType });
        } catch (Exception e) {
            throw new CreateAccessorException($"Error defining type based in on interface {_interfaceType.Name}", e);
        }
    }

    private static ModuleBuilder ModuleBuilder {
        get {
            if (_moduleBuilder != null) {
                return _moduleBuilder;
            }

            var assemblyName = new AssemblyName { Name = Assembly.GetAssembly(typeof(DictionaryAccessorGenerator)).FullName };
            _moduleBuilder = AssemblyBuilder
                .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
                .DefineDynamicModule(assemblyName.Name);

            return _moduleBuilder;
        }
    }

    public Type GeneratedType() {
        try {
            AddConstructor();
            AddProperties();

            var generatedType = _typeBuilder.CreateType();
            return generatedType ?? throw new CreateAccessorException(
                $"Error generating accessor based in on interface {_interfaceType.Name}: TypeBuilder returned null");
        } catch (CreateAccessorException) {
            throw;
        } catch (Exception e) {
            throw new CreateAccessorException($"Error generating accessor based in on interface {_interfaceType.Name}", e);
        }
    }

    private void AddConstructor() {
        Type[] constructorArgs = { typeof(IDictionary<string, object?>) };
        var constructorBuilder = _typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructorArgs);

        var codeGenerator = constructorBuilder.GetILGenerator();

        if (_typeBuilder.BaseType == null) {
            throw new CreateAccessorException($"BaseType of TypeBuilder is null when generating accessor based in on interface {_interfaceType.Name}");
        }

        var baseCtor = _typeBuilder.BaseType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, constructorArgs, null);
        if (baseCtor == null) {
            throw new CreateAccessorException($"Base Constructor not found when generating accessor based in on interface {_interfaceType.Name}");
        }

        codeGenerator.Emit(OpCodes.Ldarg_0);        //push 'this' onto the stack
        codeGenerator.Emit(OpCodes.Ldarg_1);        //push resource onto the stack
        codeGenerator.Emit(OpCodes.Call, baseCtor); //call the base constructor
        codeGenerator.Emit(OpCodes.Ret);            //return
    }

    private void AddProperties() {
        foreach (var property in _interfaceType.GetAllProperties()) {
            AddProperty(property);
        }
    }

    private void AddProperty(PropertyInfo property) {
        var methodBuilder = _typeBuilder.DefineMethod("get_" + property.Name, MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.Standard | CallingConventions.HasThis, property.PropertyType, Type.EmptyTypes);

        if (_typeBuilder.BaseType == null) {
            throw new CreateAccessorException($"BaseType of TypeBuilder is null when generating accessor based in on interface {_interfaceType.Name}");
        }

        var getDataMethod = _typeBuilder.BaseType.GetMethod("GetData", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(string) }, null);
        if (getDataMethod == null) {
            throw new CreateAccessorException($"Could not find 'GetData' method when generating based in on interface {_interfaceType.Name}");
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