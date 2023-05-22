using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Attributes
{
    public class NetworkMethod : Attribute
    {
        public string MethodName { get; }


        public NetworkMethod(string methodName)
        {
            MethodName = methodName;
        }
    }
}
