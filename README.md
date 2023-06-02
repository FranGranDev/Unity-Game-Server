# Ping Pong Online
**Multiplayer game made on Unity**

https://github.com/FranGranDev/Ping-Pong-Online/assets/87944585/435874ac-5672-4a9f-8d11-3c967f311645

# Networking

Server and client work on .Net, using UdpClient to send and recieve data.
Client communicate using RPC, by sending [Message](Assets/Scripts/Networking/Messages/Message.cs) class

## Messages

**Create Message**
```csharp
public Message(string methodName, params object[] args)
{
    MethodName = methodName; //method name to call on server
    GenerateID();

    Data = new List<ArgData>();
    foreach (object arg in args) //save all arguments as strings
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
**Serialize Message**
```csharp
public byte[] ToBytes()
{
    return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)); //serialize to json then to bytes
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
```
**Getting Arguments**
```csharp
public object[] GetData()
{
    object[] data = new object[Data.Count];

    for (int i = 0; i < Data.Count; i++)
    {
        try
        {
            Type targetType = Type.GetType($"{Data[i].Type}, {Data[i].Assembly}"); //getting type by it's name and assembly
            data[i] = JsonConvert.DeserializeObject(Data[i].Data, targetType);//deserialize object by it's type
        }
        catch (Exception e)
        {
            SafeDebugger.Log($"Can't deserialize object of type {Data[i].Type}. JSON: {Data[i].Data}. Error: {e}");

            return null;
        }
    }

    return data;
}
```
## RPC Methods

Client and server are inherited from the [NetworkMethods](Assets/Scripts/Networking/Services/NetworkMethods.cs) class, which contains virtual methods that clients communicate with each other. RPC methods must be marked with the NetworkMethod attribute with the name of the method (better to use the method name itself)

```csharp
[NetworkMethod(nameof(ErrorMessage))]
public virtual void ErrorMessage(string error, RecieveInfo info)
{
    SafeDebugger.Log(error);
}
[NetworkMethod(nameof(ChatMessage))]
public virtual void ChatMessage(Player player, string text, RecieveInfo info)
{
    SafeDebugger.Log($"{player.Name}: {text}");
}
[NetworkMethod(nameof(Connect))]
public virtual void Connect(Player player, RecieveInfo info)
{

}
[NetworkMethod(nameof(Disconnect))]
public virtual void Disconnect(Player player, RecieveInfo info)
{

}
[NetworkMethod(nameof(LoadScene))]
public virtual void LoadScene(int sceneIndex, RecieveInfo info)
{

}
[NetworkMethod(nameof(RequestPlayersList))]
public virtual void RequestPlayersList(RecieveInfo info)
{

}
[NetworkMethod(nameof(PlayersList))]
public virtual void PlayersList(List<Player> players, RecieveInfo info)
{

}
[NetworkMethod(nameof(StartRound))]
public virtual void StartRound(Dictionary<string, int> score, RecieveInfo info)
{

}
[NetworkMethod(nameof(EndRound))]
public virtual void EndRound(Player winner, RecieveInfo info)
{

}
[NetworkMethod(nameof(UpdateObject))]
public virtual void UpdateObject(string id, object data, RecieveInfo info)
{

}
```

## Client

**Recieve**
[Client](Assets/Scripts/Networking/Server/Client.cs) class at startup starts a cycle of receiving messages from the server.

```csharp
private async UniTask RecieveLoop()
{
    await SendMessage(new Message(nameof(Connect), player)); //Send that user connected

    try
    {
        while (Working)
        {
            await Recieve();
        }
    }
    catch
    {
        await Stop();
    }
}

private async UniTask Recieve()
{
    UdpReceiveResult receivedResult = await udpClient.ReceiveAsync(); //wait for recieve data

    byte[] data = new byte[receivedResult.Buffer.Length];
    Array.Copy(receivedResult.Buffer, data, receivedResult.Buffer.Length);


    Message message = Message.FromBytes(data); //deserialize message
    RecieveInfo info = new RecieveInfo() //create recieve info
    {
        EndPoint = receivedResult.RemoteEndPoint,
        MessageId = message.Id,
    };


    UnityMainThreadDispatcher.Execute(() => //execute method on main thread.
    {
        Invoker.Invoke(message.MethodName, Concat(message.GetData(), info)); //invoke method on client
    });
}
```

**Invoke method by it's attribute name**
The [NetworkMethodInvoker](Assets/Scripts/Networking/Services/NetworkMethodInvoker.cs) class is used to call a method by the attribute name NetworkMethod

```csharp
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
```


**Send message**
To send a message, you must pass the name of the method and arguments. The arguments must match those of the method you want to call, except for the RecieveInfo argument

```csharp
public async UniTask Send(string method, params object[] args)
{
    await SendMessage(new Message(method, args));
}

private async UniTask SendMessage(Message message)
{
    if (udpClient == null)
        return;

    try
    {
        byte[] data = message.ToBytes();
        await udpClient.SendAsync(data, data.Length, serverEndPoint);
    }
    catch(Exception e)
    {
        Debug.Log($"Client | Exception: {e}");
    }
}
```

