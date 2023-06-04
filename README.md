# Unity Game Server
**Fast UDP Game Server working on Unity. Ping Pong game made as example**

- Unity 2021.3.20
- UniTask
- DOTween
<br>

https://github.com/FranGranDev/Unity-Game-Server/assets/87944585/f5924bef-4510-4c0f-bb97-35556aa2d853


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

**NetworkMethods**
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

[Player](Assets/Scripts/Networking/Data/Player.cs) class  represents information about the player: his name, id and Index. Index is formed based on ordering all players by Id.

```csharp
public class Player
{
    public static Player ServerPlayer
    {
        get
        {
            return new Player("Server")
            {
                Server = true,
            };
        }
    }

    public Player(string name)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public int Index { get; set; }

    public bool Master { get; set; }
    public bool Server { get; set; }

    public override bool Equals(object obj)
    {
        if (obj is Player otherPlayer)
        {
            return Id.Equals(otherPlayer.Id);
        }
        return false;
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
```
<br>

**Recieve** <br>
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
<br>

**Invoke method by it's attribute name** <br>
The [NetworkMethodInvoker](Assets/Scripts/Networking/Services/NetworkMethodInvoker.cs) class is used to call a method by the [NetworkMethod](Assets/Scripts/Networking/Attributes/NetworkMethod.cs) attribute name 

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
```
<br>

**Send message** <br>
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
<br>

## Server
[Server](Assets/Scripts/Networking/Server/Server.cs) receives messages and broadcast them to all connected clients. The server receives the message in the same way as the client and calls the desired method using the Invoker [NetworkMethodInvoker](Assets/Scripts/Networking/Services/NetworkMethodInvoker.cs) class. In the called methods, the server sends the received message (you can also check the message and change it additionally)

**Player Connected** <br>
```csharp
public override async void Connect(Player player, RecieveInfo info)
{
    if(Handlers.Count >= 2)
    {
        return;
    }

    Handler handler = new Handler(udpClient, info.EndPoint);
    handler.Player = player;
    Handlers.Add(info.EndPoint, handler);

    Message message = new Message(nameof(Connect), player);
    message.Id = info.MessageId;

    await Broadcast(message);

    SafeDebugger.Log($"Server | New player connected: {player.Name} | {info.EndPoint}");
}
```
<br>

**Player Disconnected** <br>
```csharp
public override async void Disconnect(Player player, RecieveInfo info)
{
    if (!Handlers.ContainsKey(info.EndPoint))
    {
        return;
    }
    Handlers.Remove(info.EndPoint);

    Message message = new Message(nameof(Disconnect), player);
    message.Id = info.MessageId;

    await Broadcast(message, info.EndPoint);

    SafeDebugger.Log($"Server | Player disconnected: {player.Name}");
}
```

<br>

**Broadcast message** <br>
```csharp
private async Task Broadcast(Message message)
{
    IEnumerable<Handler> receivers = new List<Handler>(Handlers.Values);

    foreach (Handler handler in receivers)
    {
        await handler.Send(message);
    }
}

private async Task Broadcast(Message message, IPEndPoint except)
{
    IEnumerable<Handler> receivers = new List<Handler>(Handlers.Values
        .Where(x => !x.EndPoint.Equals(except)));

    foreach (Handler handler in receivers)
    {
        await handler.Send(message);
    }
}
```

## Synchronization

[NetworkObject](Assets/Scripts/Management/UnityServer/Synchronizers/NetworkObject.cs) is the base abstract class for synchronizing an object over the network.
```csharp
public abstract class NetworkObject : MonoBehaviour
{
    public string Id { get; set; } //Unique object ID
    public bool Mine { get; set; } //Is object local or remote
    public abstract object Data { get; } //Necessary information about the object to update it



    public void SetId(Player player) //Set ID of object
    {
        Id = $"{player.Id}_{name}";
    }
    
    public abstract void Synchronize(object data); //Method to update object
}
```
<br>

**Example | Rigidbody Object** <br>

```csharp
[RequireComponent(typeof(Rigidbody))]
public class NetworkRigidbody : NetworkObject
{
    private new Rigidbody rigidbody;
    public override object Data
    {
        get
        {
            return new RigidbodyData(rigidbody);
        }
    }
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    public override void Synchronize(object data)
    {
        try
        {
            RigidbodyData rigidbodyData = data as RigidbodyData;

            rigidbody.position = rigidbodyData.Position.GetVector();
            rigidbody.velocity = rigidbodyData.Velocity.GetVector();
            rigidbody.angularVelocity = rigidbodyData.Angular.GetVector();
        }
        catch { Debug.Log($"Cant convert {data} to RigidbodyData", this); }
    }
}
```

**Rigidbody Data** <br>

```csharp
public class RigidbodyData
{
    public RigidbodyData()
    {

    }
    public RigidbodyData(Rigidbody rigidbody)
    {
        Position = new Vector3Data(rigidbody.position);
        Velocity = new Vector3Data(rigidbody.velocity);
        Angular = new Vector3Data(rigidbody.angularVelocity);
    }
    public void Accept(Rigidbody rigidbody)
    {
        rigidbody.position = Position.GetVector();
        rigidbody.velocity = Velocity.GetVector();
        rigidbody.angularVelocity = Angular.GetVector();
    }
    public Vector3Data Position { get; set; }
    public Vector3Data Velocity { get; set; }
    public Vector3Data Angular { get; set; }
}
```
<br>

[ObjectSynchronizer](Assets/Scripts/Management/UnityServer/Synchronizers/ObjectSynchronizer.cs) class is used to keep track of local objects and update remote objects. 

**Initialization** <br>
```csharp
public void Initialize()
{
    IEnumerable<NetworkObject> objects = 
    transform.GetComponentsInChildren<NetworkObject>(true);

    localObjects = objects
        .Where(x => x.Mine)
        .ToDictionary(x => x.Id);

    remoteObjects = objects
        .Where(x => !x.Mine)
        .ToDictionary(x => x.Id);
}
```
Before call `Initialize()` you need to go through all `NetoworkObject` and set value`.Mine` and call method `SetId(Player player)`

**Example** <br>

```csharp
[SerializeField] private List<PlayerHandler> playerHandlers;

private void SetupSynchronize()
{
    foreach (Player player in lobby.Players)
    {
        bool local = player.Equals(lobby.Self);
        PlayerHandler handler = playerHandlers[player.Index];
        handler.SetPlayer(player, local);
        handler.GetComponentsInChildren<NetworkObject>()
            .ToList()
            .ForEach(x =>
            {
                x.SetId(player);
                x.Mine = local;
            });
    }

    objectSynchronizer.OnObjectUpdated += OnObjectUpdated;
    objectSynchronizer.Initialize();
}
```
<br>

In `Update()` [ObjectSynchronizer](Assets/Scripts/Management/UnityServer/Synchronizers/ObjectSynchronizer.cs) invoke `event Action<string, object> OnObjectUpdated`. [Client](Assets/Scripts/Networking/Server/Client.cs) can monitor this event and send it to the server.

```csharp
 private void Update()
 {
     localObjects.Values
         .ToList()
         .ForEach(x => OnObjectUpdated?.Invoke(x.Id, x.Data));
 }
```

After client send `Id` and `Data` another client recieve this message and call `UpdateRemoteObject(string id, object data)` method.

```csharp
 public void UpdateRemoteObject(string id, object data)
 {
     if(remoteObjects.ContainsKey(id))
     {
         remoteObjects[id].Synchronize(data);
     }
 }
```
<br>

The result is that all objects that contain an inheritor of the `NetworkObject` class will be synchronized between the two clients.
