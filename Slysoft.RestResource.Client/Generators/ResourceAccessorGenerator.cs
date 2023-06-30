using System;
using System.Reflection;
using System.Reflection.Emit;
using Slysoft.RestResource.Client.Accessors;

namespace Slysoft.RestResource.Client.Generators;

internal sealed class ResourceAccessorGenerator<T> : AccessorGenerator where T : class {
    public ResourceAccessorGenerator() : base(typeof(T), typeof(ResourceAccessor)) {
    }

    public Type GeneratedType() {
        try {
            AddConstructor();
            AddProperties();

            var generatedType = TypeBuilder.CreateType();
            return generatedType ?? throw new CreateAccessorException(
                $"Error generating accessor based in on interface {typeof(T)}: TypeBuilder returned null");
        } catch (CreateAccessorException) {
            throw;
        } catch (Exception e) {
            throw new CreateAccessorException($"Error generating accessor based in on interface {typeof(T)}.Name", e);
        }
    }

    private void AddConstructor() {
        Type[] constructorArgs = { typeof(Resource) };
        var constructorBuilder = TypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructorArgs);

        var codeGenerator = constructorBuilder.GetILGenerator();

        if (TypeBuilder.BaseType == null) {
            throw new CreateAccessorException($"BaseType of TypeBuilder is null when generating accessor based in on interface {typeof(T).Name}");
        }

        var baseCtor = TypeBuilder.BaseType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, constructorArgs, null);
        if (baseCtor == null) {
            throw new CreateAccessorException($"Base Constructor not found when generating accessor based in on interface {typeof(T).Name}");
        }

        codeGenerator.Emit(OpCodes.Ldarg_0);        //push 'this' onto the stack
        codeGenerator.Emit(OpCodes.Ldarg_1);        //push resource onto the stack
        codeGenerator.Emit(OpCodes.Call, baseCtor); //call the base constructor
        codeGenerator.Emit(OpCodes.Ret);            //return
    }
}