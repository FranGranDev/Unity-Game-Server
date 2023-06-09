﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Networking.Attributes;

namespace Networking.Services
{
    public class NetworkMethodInvoker
    {
        private Dictionary<string, MethodInfo> methodDictionary;
        private object targetObject;


        public NetworkMethodInvoker(object target)
        {
            methodDictionary = new Dictionary<string, MethodInfo>();
            targetObject = target;

            Type objectType = target.GetType();
            MethodInfo[] methods = objectType.GetMethods();

            foreach (MethodInfo method in methods)
            {
                NetworkMethod attribute = method.GetCustomAttribute<NetworkMethod>();

                if (attribute == null)
                    continue;

                string methodName = attribute.MethodName;
                methodDictionary.Add(methodName, method);
            }
        }


        public void Invoke(string methodName, params object[] args)
        {
            if (methodDictionary.ContainsKey(methodName))
            {
                MethodInfo method = methodDictionary[methodName];

                try
                {
                    method.Invoke(targetObject, args);
                }
                catch(Exception e)
                {
                    SafeDebugger.Log($"Invalid argument for method {methodName} | Exception: {e}");
                }
            }
            else
            {
                SafeDebugger.Log($"Method {methodName} not found");
            }
        }
    }
}
