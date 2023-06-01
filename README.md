# Ping Pong Online
**Multiplayer game made on Unity**

https://github.com/FranGranDev/Ping-Pong-Online/assets/87944585/435874ac-5672-4a9f-8d11-3c967f311645

# Server

Server work on .Net, using UdpClient to send and recieve data.
Client communicate using RPC, by sending [Message](./Assets/Scripts/Networking/Messages/Message.cs) class

**Create Message**
```csharp
 public Message(string methodName, params object[] args)
 {
     MethodName = methodName; //method name to call on server
     GenerateID();

     Data = new List<ArgData>();
     foreach (object arg in args) //save all arguments as json
     {
         Data.Add(new ArgData()
         {
             Data = JsonConvert.SerializeObject(arg), //save value
             Type = arg.GetType().FullName, //save type
             Assembly = arg.GetType().Assembly.FullName, //save assembly
         });
     }
 }
```
