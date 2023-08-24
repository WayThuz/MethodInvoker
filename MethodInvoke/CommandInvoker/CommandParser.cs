using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public interface ICommandParser {
	object Parse(string input);
	bool TryParseCommandInput(out Dictionary<string, object> output);
	bool CanConvert<TFrom, TTo>(TFrom from);
}

public class DefaultCommandParser : ICommandParser{
	public bool TryParseCommandInput(out Dictionary<string, object> output) {
        string[] args = Environment.GetCommandLineArgs();
        string[] varName = null;
        output = new Dictionary<string, object>();
        List<object> variables = new List<object>();
        for(int i = 0; i < args.Length; i++) {
            if(args[i].Equals("-class")) {
                output.Add("className", args[i + 1]);
            }
            else if(args[i].Equals("-method")) {
                output.Add("methodName", args[i + 1]);
            }
            else if(args[i].Equals("-params")) {
                varName = args[i + 1].Split(',');
            }
        }
        if(!output.ContainsKey("className")) {
            Debug.LogError("No Class");
            return false;
        }
        if(!output.ContainsKey("methodName")) {
            Debug.LogError("No Method");
            return false;
        }
        if(varName == null) {
            Debug.LogError("No Params");
            return false;
        }
        for(int i = 0; i < varName.Length; i++) {
            variables.Add(Parse(varName[i]));
        }
        output.Add("variables", variables.ToArray());
        return true;
    }

    public object Parse(string value) {
        if(value == null) {
            throw new ArgumentNullException("value");
        }
        if(bool.TryParse(value, out bool b)) return b;
        if(int.TryParse(value, out int i)) return i;
        if(char.TryParse(value, out char c)) return c;
        return value;
    }

    public bool CanConvert<TFrom, TTo>(TFrom from){
		try {
			TTo to = (TTo)(dynamic)from;
			return true;
		}
		catch { 
			return false; 
		}
	}
}

