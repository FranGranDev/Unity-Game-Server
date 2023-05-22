using System;
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
                try
                {
                    MethodInfo method = methodDictionary[methodName];
                    method.Invoke(targetObject, args);
                }
                catch
                {
                    Logger.Log($"Invalid argument for method {methodName}");
                }
            }
            else
            {
                Logger.Log($"Method {methodName} not found");
            }
        }
    }
}
