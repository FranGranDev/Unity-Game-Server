using Networking.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;


namespace Networking.Messages
{
    [Serializable]
    public class Message
    {
        public Message()
        {

        }
        public Message(string methodName, params object[] args)
        {
            MethodName = methodName;
            GenerateID();

            Data = new List<ArgData>();
            foreach (object arg in args)
            {
                Data.Add(new ArgData()
                {
                    Data = JsonConvert.SerializeObject(arg),
                    Type = arg.GetType().FullName,
                    Assembly = arg.GetType().Assembly.FullName,
                });
            }
        }
        public Message(string methodName)
        {
            MethodName = methodName;
            GenerateID();

            Data = new List<ArgData>();
        }


        /// <summary>
        /// Method to call on Server/Client with attribute NetworkMethod
        /// </summary>
        public string MethodName { get; set; }
        public string Id { get; set; }
        public List<ArgData> Data { get; set; }


        public object[] GetData()
        {
            object[] data = new object[Data.Count];

            for (int i = 0; i < Data.Count; i++)
            {
                try
                {
                    Type targetType = Type.GetType($"{Data[i].Type}, {Data[i].Assembly}");
                    data[i] = JsonConvert.DeserializeObject(Data[i].Data, targetType);
                }
                catch (Exception e)
                {
                    SafeDebugger.Log($"Can't deserialize object of type {Data[i].Type}. JSON: {Data[i].Data}. Error: {e}");

                    return null;
                }
            }

            return data;
        }


        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }
        public static Message FromBytes(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);

            try
            {
                return JsonConvert.DeserializeObject<Message>(json);
            }
            catch (Exception e)
            {
                return new Message("Error", e.Message);
            }
        }


        private void GenerateID()
        {
            Id = Guid.NewGuid().ToString();
        }


        [Serializable]
        public class ArgData
        {
            public string Type { get; set; }
            public string Assembly { get; set; }
            public string Data { get; set; }
        }
    }


    public class RecieveInfo
    {
        public IPEndPoint EndPoint { get; set; }
        public string MessageId { get; set; }
    }
}
