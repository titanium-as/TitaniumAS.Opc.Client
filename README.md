# TitaniumAS.Opc.Client
Open source .NET client library for OPC DA. The library provides you with .NET COM wrappers for OPC DA interoperability.

## Features
- Support of local and network OPC DA servers.
- Support of OPC DA 1.0, 2.05A, 3.0.
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

## Basic usage
The following examples cover basic usages of the library. Assume you have an application with installed NuGet package of the library.

#### Bootstrapping the library
Call `Bootstrap.Initialize()` in the start of your application. An application process should be started under MTA apartment state due to [CoInitializeSecurity](http://www.pinvoke.net/default.aspx/ole32/CoInitializeSecurity.html) call during the library initialization. See [explanation](http://www.pinvoke.net/default.aspx/ole32/CoInitializeSecurity.html).

#### Connecting to an OPC DA server
You should create OPC DA server instance first and then connect to it.
```csharp
// Make an URL of OPC DA server using builder.
Uri url = UrlBuilder.Build("Matrikon.OPC.Simulation.1");
using (var server = new OpcDaServer(url))
{
    // Connect to the server first.
    server.Connect();
    ...
}
```

#### Browsing elements
You can browse all elements of any OPC DA servers versions with `OpcDaBrowserAuto`.
```csharp
// Create a browser and browse all elements recursively.
var browser = new OpcDaBrowserAuto(server);
BrowseChildren(browser);
...

void BrowseChildren(IOpcDaBrowser browser, string itemId = null, int indent = 0)
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

#### Creating a group with items
You can add a group with items to the OPC DA server. 
```csharp
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
Items of a group can be read either synchronously or asynchronously.
```csharp
// Read all items of the group synchronously.
OpcDaItemValue[] values = group.Read(group.Items, OpcDaDataSource.Device);
...

// Read all items of the group asynchronously.
OpcDaItemValue[] values = await group.ReadAsync(group.Items);
...
```

#### Writing values
Also items of a group can be written either synchronously or asynchronously.
```csharp
// Prepare items.
OpcDaItem int2 = group.Items.FirstOrDefault(i => i.ItemId == "Bucket Brigade.Int2");
OpcDaItem int4 = group.Items.FirstOrDefault(i => i.ItemId == "Bucket Brigade.Int4");
OpcDaItem[] items = { int2, int4 };

// Write values to the items synchronously.
object[] values = { 1, 2 };
HRESULT[] results = group.Write(items, values);
...

// Write values to the items synchronously.
object[] values = { 3, 4 };
HRESULT[] results = await group.WriteAsync(items, values);
...
```

#### Getting values by subscription
A group can be configured for providing a client with new values when they are changed.
```csharp
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
