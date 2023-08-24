using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvokeExample
{
    public static void MyMethod() { }

    public static void MyMethod(string param1) => Debug.Log(param1);

    //public static void MyMethod(string param1, int param2) => Debug.Log($"{param1} + {param2}");

    public static void MyMethod(string param1, int param2) => Debug.Log($"{param1} + {param2}");

    public static void MyMethod(int param1, bool param2) => Debug.Log($"{param1} {param2}");

    public static void MyMethod(params string[] param1) {
        for(int i = 0; i < param1.Length; i++) { 
            Debug.Log(param1[i]);
        }
    }
    
    public static void MyMethod(params CommandInvokeSubclass[] param1) => Debug.Log("Is Params Attributes");

    public class CommandInvokeSubclass { }
}
