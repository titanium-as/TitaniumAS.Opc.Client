# TitaniumAS.Opc.Client
Open source .NET client library for OPC DA. The library provides you with .NET COM wrappers for OPC DA interoperability.

## Features
- Support of local and network OPC DA servers.
- Support OPC DA 1.0, 2.05A, 3.0.
- Browsing of OPC DA servers.
- Async/await in read and write operations.
- Subscription to data changes via .NET events.
- Support of server shutdown events.
- Easy resource management.

## Installation
Run the following command in the NuGet Package Manager console:
```
PM> Install-Package TitaniumAS.Opc.Client
```
See [NuGet package](https://www.nuget.org/packages/TitaniumAS.Opc.Client).

## Bootstrapping the library
A process which uses the library should be started under MTA apartment state due to [CoInitializeSecurity](http://www.pinvoke.net/default.aspx/ole32/CoInitializeSecurity.html) call during the library initialization. 

When you initialize the library in a UI application (WinForms/WPF), an `STAThreadAttribute` should be removed from the program entry point. You can use a workaround like the following:
- extract a content of a method `Main` to `RunApplication`;
- call `Bootstrap.Initialize()` first;
- create new thread with STA apartment state to run the UI application:
```csharp
  var thread = new Thread(RunApplication);
  thread.SetApartmentState(ApartmentState.STA);
  thread.Start();
```

## Basic usage
The following examples cover basic usages of the library. Assume you have a console application with a method `Main` as entry point. Also you have installed NuGet package with the library.

#### Connecting to an OPC DA server
You should create OPC DA server instance first and then connect to it.
```csharp
static void Main(string[] args)
{
    // Make URL of OPC DA server using builder.
    Uri url = UrlBuilder.Build("Matrikon.OPC.Simulation.1");
    using (var server = new OpcDaServer(url))
    {
        // Connect to the server first.
        server.Connect();
        ...
        
        Console.ReadLine();
    }
}
```

#### Browsing elements
Here is a helper method `BrowseChildren` you can use to browse all elements of an OPC DA server.
```csharp
static void BrowseChildren(IOpcDaBrowser browser, string itemId = null, int indent = 0)
{
    // When itemId is null, root elements will be browsed.
    OpcDaBrowseElement[] elements = browser.GetElements(itemId);

    // Output elements.
    foreach (OpcDaBrowseElement element in elements)
    {
        // Output the element.
        Console.Write(new String(' ', indent));
        Console.WriteLine(element);

        // Skip elements without children.
        if (!element.HasChildren)
            continue;

        // Output children of the element.
        BrowseChildren(browser, element.ItemId, indent + 2);
    }
}
```
Let's use it in the `Main`. 
```csharp
...
// Browse elements.
var browser = new OpcDaBrowserAuto(server);
BrowseChildren(browser);
...
```

#### Creating a group with items
Let's add a group with two items to the OPC DA server. 
```csharp
...
// Create a group with items.
OpcDaGroup group = server.AddGroup("MyGroup");
group.IsActive = true;

var definition1 = new OpcDaItemDefinition
{
    ItemId = "Random.Int2",
    IsActive = true
};
var definition2 = new OpcDaItemDefinition
{
    ItemId = "Bucket Brigade.Int4",
    IsActive = true
};
OpcDaItemDefinition[] definitions = { definition1, definition2 };
OpcDaItemResult[] results = group.AddItems(definitions);

// Handle adding results.
foreach (OpcDaItemResult result in results)
{
    if (result.Error.Failed)
        Console.WriteLine("Error adding items: {0}", result.Error);
}
...
```

#### Reading values
So we have the group, let's read values of items. It can be made either synchronously or asynchronously as you wish.
Either
```csharp
...
OpcDaItemValue[] values = group.Read(group.Items, OpcDaDataSource.Device);
...
```
or
```csharp
...
Task<OpcDaItemValue[]> task = ReadValuesAsync(group);
task.Wait();
values = task.Result;
...

static async Task<OpcDaItemValue[]> ReadValuesAsync(OpcDaGroup group)
{
    return await group.ReadAsync(group.Items);
}
```

#### Writing values
Let's write value `123` to the item `Bucket Brigade.Int4`. As before it can be made either synchronously (using `group.Write`) or asynchronously (using `group.WriteAsync`).
```csharp
...
// Write value to the item.
OpcDaItem item = group.Items.FirstOrDefault(i => i.ItemId == "Bucket Brigade.Int4")
OpcDaItem[] items = { item };
object[] newValues = { 123 };

HRESULT[] writeResults = group.Write(items, newValues);
// or group.WriteAsync(items, newValues).Result;

// Handle write result.
if (writeResults[0].Failed)
    Console.WriteLine("Error writing value");
...
```

#### Getting values by subscription
A group can be configured for providing a client with new values when they are changed. Let's subscribe to `ValuesChanged` event of the group.
```csharp
...
// Configure subscription.
group.ValuesChanged += OnGroupValuesChanged;
group.UpdateRate = TimeSpan.FromMilliseconds(100); // ValuesChanged won't be triggered if zero
...

static void OnGroupValuesChanged(object sender, OpcDaItemValuesChangedEventArgs args)
{
    // Output values.
    foreach (OpcDaItemValue value in args.Values)
    {
        Console.WriteLine("ItemId: {0}; Value: {1}; Quality: {2}; Timestamp: {3}",
            value.Item.ItemId, value.Value, value.Quality, value.Timestamp);
    }
}
```

## API documentation
Comming soon...

##License
The MIT License (MIT) – [LICENSE](https://github.com/titanium-as/TitaniumAS.Opc.Client/blob/master/LICENSE).
