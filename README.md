# Signhost client library
[![join gitter chat](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Evidos/signhost-api)
[![Build status](https://ci.appveyor.com/api/projects/status/696lddgivr6kkhsd/branch/master?svg=true)](https://ci.appveyor.com/project/MrJoe/signhostclientlibrary-xcr5f/branch/master)
[![Nuget package](https://img.shields.io/nuget/v/SignhostClientLibrary.svg)](https://www.nuget.org/Packages/SignhostClientLibrary)

This is a client library in c# to demonstrate the usage of the [signhost api](https://api.signhost.com/) using .net.
You will need a valid APPKey and APIKey.
You can request a APPKey for signhost at [ondertekenen.nl](https://www.ondertekenen.nl/api-proefversie/).

```c#
var client = new SignHostApiClient(new SignHostApiClientSettings("AppName appkey", "apikey"));

var transaction = await client.CreateTransaction(new Transaction
{
	Signers = new List<Signer>
	{
		new Signer
		{
			Email = "john.doe@example.com",
			ScribbleName = "John Doe",
			SignRequestMessage = "Could you please sign this document?",
			SendSignRequest = true
		}
	}
});

await client.AddOrReplaceFileToTransaction("PathToFile",    transaction.Id, "First document");
await client.AddOrReplaceFileToTransaction("PathOtherFile", transaction.Id, "General agreement");

/* When everything is setup we can start the transaction flow */
await client.StartTransaction(transaction.Id);

```

Get it on NuGet:

`PM> Install-Package SignhostClientLibrary`
