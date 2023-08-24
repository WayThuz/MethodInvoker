using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class CommandInvoker
{
    private static object paramsParameter = null;

    [MenuItem("MethodInvoke/Execute")]
    public static void ExecuteExample() {
        List<object> variables = new List<object>();
        string[] varName = new string[2] { "https://blog.miniasp.com/post/2010/09/24/How-to-parse-text-from-file-or-command-using-Batch", 
                                           "https://www.google.com/imghp?hl=zh-TW&tab=ri" };
        DefaultCommandParser parser = new DefaultCommandParser();
        for(int i = 0; i < varName.Length; i++) {
            variables.Add(parser.Parse(varName[i]));
        }
        var parseResult = new Dictionary<string, object>() { { "className", "CommandInvokeExample" }, { "methodName", "MyMethod" }, { "variables", variables.ToArray() } };
        string className = parseResult["className"] as string;
        string methodName = parseResult["methodName"] as string;
        object[] inputParameters = parseResult["variables"] as object[];
        Type type = Assembly.Load("Assembly-CSharp").GetType(className);
        if(type == null) {
            Debug.Log($"Class {className} not found.");
            return;
        }
        if(!TryGetSuitableMethod(type, methodName, out var targetMethod, inputParameters)) {
            Debug.Log($"Method {methodName} not found");
            return;
        }
        InvokeMethod(type, targetMethod, inputParameters);
    }

    public static void Execute() {
        DefaultCommandParser parser = new DefaultCommandParser();
        if(!parser.TryParseCommandInput(out var parseResult)) return;
        string className = parseResult["className"] as string;
        string methodName = parseResult["methodName"] as string;
        object[] inputParameters = parseResult["variables"] as object[]; 
        Type type = Assembly.Load("Assembly-CSharp").GetType(className);
        if(type == null) {
            Debug.Log($"Class {className} not found.");
            return;
        }
        if(!TryGetSuitableMethod(type, methodName, out var targetMethod, inputParameters)) {
            Debug.Log($"Method {methodName} not found");
            return;
        }
        if(paramsParameter as Array != null) {
            CollectParamsInputTogether(ref inputParameters);
        }
        InvokeMethod(type, targetMethod, inputParameters);
    }

    private static bool TryGetSuitableMethod(Type type, string methodName, out MethodInfo targetMethod, object[] inputParameters) {
        targetMethod = null;
        var methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static);
        for(int i = 0; i < methodInfos.Length; i++) {
            if(!IsTargetMethod(methodInfos[i], methodName, inputParameters)) continue;
            targetMethod = methodInfos[i];
            return true;
        }
        return false;
    }

    private static void InvokeMethod(Type type, MethodInfo targetMethod, object[] parameters) {
        bool isStaticMethod = type.IsAbstract && type.IsSealed;
        if(isStaticMethod) {
            targetMethod.Invoke(null, parameters);
        }
        else {
            object instance = Activator.CreateInstance(type);
            targetMethod.Invoke(instance, parameters);
        }
    }

    private static void CollectParamsInputTogether(ref object[] inputParameters) {
        try {
            int length = (paramsParameter as Array).Length;
            var duplicate = inputParameters;
            inputParameters = new object[inputParameters.Length - length + 1];
            for(int i = 0; i < inputParameters.Length - 1; i++) {
                inputParameters[i] = duplicate[i];
            }
            inputParameters[^1] = paramsParameter;
        }
        catch(Exception e) {
            Debug.LogException(e);
        }
    }

    private static bool IsTargetMethod(MethodInfo methodInfo, string methodName, object[] inputPara) {
        if(methodInfo.Name != methodName) return false;
        var funcPara = methodInfo.GetParameters();
        int max = Math.Max(inputPara.Length, funcPara.Length);
        string paramsParaName = null;
        for(int i = 0; i < max; i++) {
            if(inputPara.Length <= i) {
                if(funcPara[i].DefaultValue == null) return false;
            }
            else if(funcPara.Length <= i) {
                if(string.IsNullOrEmpty(paramsParaName) || inputPara[i].GetType().FullName != paramsParaName) return false;  
            }
            else {
                if(funcPara[i].ParameterType == inputPara[i].GetType()) continue;
                return funcPara[i].IsDefined(typeof(ParamArrayAttribute)) && TryGetCompatibleParamsArray(inputPara, i, funcPara[i]);
            }
        }
        return true;
    }

    private static bool TryGetCompatibleParamsArray(object[] inputPara, int startIndex, ParameterInfo parameterInfo) {
        paramsParameter = null;
        Type type = parameterInfo.ParameterType.GetElementType();
        string paramsParaName = type.FullName;
        Array array = Array.CreateInstance(type, inputPara.Length - startIndex);
        for(int i =  startIndex; i < inputPara.Length; i++) {
            if(paramsParaName != inputPara[i].GetType().FullName) return false;
            array.SetValue(inputPara[i], i - startIndex);
        }
        paramsParameter = array;
        return true;
    }
}
