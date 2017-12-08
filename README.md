# Signhost client library
[![join gitter chat](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Evidos/signhost-api)
[![Build status](https://ci.appveyor.com/api/projects/status/696lddgivr6kkhsd/branch/master?svg=true)](https://ci.appveyor.com/project/MrJoe/signhostclientlibrary-xcr5f/branch/master)
[![Nuget package](https://img.shields.io/nuget/v/SignhostClientLibrary.svg)](https://www.nuget.org/Packages/SignhostClientLibrary)
[![Quality Gate](https://sonarqube.com/api/badges/gate?key=SignhostAPIClient)](https://sonarqube.com/dashboard/index/SignhostAPIClient)

This is a client library in c# to demonstrate the usage of the [signhost api](https://api.signhost.com/) using .net.
You will need a valid APPKey and APIKey.
You can request a APPKey for signhost at [ondertekenen.nl](https://www.ondertekenen.nl/api-proefversie/).

### Install
Get it on NuGet:

`PM> Install-Package SignhostClientLibrary`

### Example code
The following code is an example of how to create and start a sign transaction with two documents.
```c#
var client = new SignHostApiClient(new SignHostApiClientSettings("AppName appkey", "apikey or usertoken"));

var transaction = await client.CreateTransactionAsync(new Transaction
{
	Signers = new List<Signer>
	{
		new Signer
		{
			Email = "john.doe@example.com",
			SignRequestMessage = "Could you please sign this document?",
			SendSignRequest = true,
			Verifications = new List<IVerification> {
				new PhoneNumberVerification {
					Number = "+3161234567890"
				},
				new ScribbleVerification {
					ScribbleName = "John Doe",
					RequireHandsignature = true,
				},
			}
		}
	}
});

await client.AddOrReplaceFileToTransactionAsync("PathToFile",    transaction.Id, "First document");
await client.AddOrReplaceFileToTransactionAsync("PathOtherFile", transaction.Id, "General agreement");

/* When everything is setup we can start the transaction flow */
await client.StartTransactionAsync(transaction.Id);

```
