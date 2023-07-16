using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using SlySoft.RestResource.Client.Accessors;
using SlySoft.RestResource.Client.Extensions;

namespace SlySoft.RestResource.Client.Generators;

internal sealed class ObjectDataAccessorGenerator : AccessorGenerator {
    public ObjectDataAccessorGenerator(Type interfaceType) : base(interfaceType, typeof(ObjectDataAccessor)) {
    }

    public Type GeneratedType() {
        try {
            AddConstructor();
            AddProperties();

            var generatedType = TypeBuilder.CreateType();
            return generatedType ?? throw new CreateAccessorException(
                $"Error generating accessor based in on interface {InterfaceType.Name}: TypeBuilder returned null");
        } catch (CreateAccessorException) {
            throw;
        } catch (Exception e) {
            throw new CreateAccessorException($"Error generating accessor based in on interface {InterfaceType.Name}", e);
        }
    }

    private void AddConstructor() {
        Type[] constructorArgs = { typeof(ObjectData) };
        var constructorBuilder = TypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructorArgs);

        var codeGenerator = constructorBuilder.GetILGenerator();

        if (TypeBuilder.BaseType == null) {
            throw new CreateAccessorException($"BaseType of TypeBuilder is null when generating accessor based in on interface {InterfaceType.Name}");
        }

        var baseCtor = TypeBuilder.BaseType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, constructorArgs, null);
        if (baseCtor == null) {
            throw new CreateAccessorException($"Base Constructor not found when generating accessor based in on interface {InterfaceType.Name}");
        }

        codeGenerator.Emit(OpCodes.Ldarg_0);        //push 'this' onto the stack
        codeGenerator.Emit(OpCodes.Ldarg_1);        //push resource onto the stack
        codeGenerator.Emit(OpCodes.Call, baseCtor); //call the base constructor
        codeGenerator.Emit(OpCodes.Ret);            //return
    }

    private void AddProperties() {
        foreach (var property in InterfaceType.GetAllProperties()) {
            if (property.IsFromResourceAccessorInterface()) {
                continue;
            }

            AddProperty(property);
        }
    }
}