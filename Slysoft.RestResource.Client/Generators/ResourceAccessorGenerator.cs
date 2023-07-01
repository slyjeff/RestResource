using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Slysoft.RestResource.Client.Accessors;
using Slysoft.RestResource.Client.Utils;

namespace Slysoft.RestResource.Client.Generators;

internal sealed class ResourceAccessorGenerator<T> : AccessorGenerator {
    public ResourceAccessorGenerator() : base(typeof(T), typeof(ResourceAccessor)) {
    }

    public Type GeneratedType() {
        try {
            AddConstructor();
            AddProperties();
            AddMethods();

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
        Type[] constructorArgs = { typeof(Resource), typeof(IRestClient) };
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
        codeGenerator.Emit(OpCodes.Ldarg_2);        //push restClient onto the stack
        codeGenerator.Emit(OpCodes.Call, baseCtor); //call the base constructor
        codeGenerator.Emit(OpCodes.Ret);            //return
    }

    private void AddMethods() {
        foreach (var method in InterfaceType.GetAllMethods()) {
            AddMethod(method);
        }
    }

    private void AddMethod(MethodInfo method) {
        var methodParameters = method.GetParameters();
        var parameterTypes = methodParameters.Select(x => x.ParameterType).ToArray();

        var methodBuilder = TypeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.Standard | CallingConventions.HasThis, method.ReturnType, parameterTypes);

        var codeGenerator = methodBuilder.GetILGenerator();

        CreateDictionaryInLocalVariable(codeGenerator, methodParameters);

        if (TypeBuilder.BaseType == null) {
            throw new CreateAccessorException($"BaseType of TypeBuilder is null when generating accessor based in on interface {typeof(T).Name}");
        }

        var callRestLinkMethod = TypeBuilder.BaseType.GetMethod("CallRestLink", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string), typeof(IDictionary<string, object?>) }, null);
        if (callRestLinkMethod == null) {
            throw new CreateAccessorException($"'CallRestLink' not found when generating accessor based in on interface {typeof(T).Name}");
        }
        var typedCallRestLinkMethod = callRestLinkMethod.MakeGenericMethod(method.ReturnType);

        codeGenerator.Emit(OpCodes.Ldarg_0);                            //push 'this' onto the stack
        codeGenerator.Emit(OpCodes.Ldstr, method.Name);                 //push the name of the link onto the stack
        codeGenerator.Emit(OpCodes.Ldloc_0);                            //push the dictionary onto the stack
        codeGenerator.Emit(OpCodes.Callvirt, typedCallRestLinkMethod);  //call the "CallRestLink" method
        codeGenerator.Emit(OpCodes.Ret);                                //return
    }

    public static void CreateDictionaryInLocalVariable(ILGenerator codeGenerator, ParameterInfo[] parameters) {
        var dictionaryType = typeof(Dictionary<string, object?>);
        var dictionaryConstructor = dictionaryType.GetConstructor(Array.Empty<Type>());

        if (dictionaryConstructor == null) {
            throw new CreateAccessorException($"Could not find dictionary constructor when generating accessor based in on interface {typeof(T).Name}");
        }

        var dictionaryAddMethod = dictionaryType.GetMethod("Add");

        if (dictionaryAddMethod == null) {
            throw new CreateAccessorException($"Could not find dictionary add method when generating accessor based in on interface {typeof(T).Name}");
        }

        codeGenerator.DeclareLocal(dictionaryType);                 //declare a local variable of type Dictionary<string, object?>
        codeGenerator.Emit(OpCodes.Newobj, dictionaryConstructor);  //create an instance of the dictionary
        codeGenerator.Emit(OpCodes.Stloc_0);                        //store the dictionary in our local variable

        var parameterLocation = 1;
        foreach (var parameter in parameters) {
            if (string.IsNullOrEmpty(parameter.Name)) {
                continue;
            }

            codeGenerator.Emit(OpCodes.Ldloc_0);                          //load the variable where the dictionary is held
            codeGenerator.Emit(OpCodes.Ldstr, parameter.Name);            //load the parameter name
            codeGenerator.Emit(OpCodes.Ldarg, parameterLocation++);       //load the value of the parameter
            if (parameter.ParameterType.IsValueType) {
                codeGenerator.Emit(OpCodes.Box, parameter.ParameterType); //box value types        
            }
            codeGenerator.Emit(OpCodes.Callvirt, dictionaryAddMethod);    //call the add method of the dictionary
        }
    }
}