# Signhost client library
[![join gitter chat](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Evidos/signhost-api)

This is a client library in c# to demonstrate the usage of the [signhost api](https://api.signhost.com/) using .net.
You will need a valid APPKey and APIKey.
You can request a APPKey for signhost at [ondertekenen.nl](https://www.ondertekenen.nl/api-proefversie/).

```c#
var client = new SignHostApiClient("AppName appkey", "apikey");

var transaction = await client.CreateTransaction(new Transaction
{
	Signers = new List<Signer>
	{
		new Signer
		{
			Email = "john.doe@example.com",
			ScribbleName = "John Doe",
			SignRequestMessage = "Could you please sign this document?"
		}
	}
});

await client.AddOrReplaceFileToTansaction("PathToFile",    transaction.Id, "First document");
await client.AddOrReplaceFileToTansaction("PathOtherFile", transaction.Id, "General agreement");

/* When everything is setup we can start the transaction flow */
await client.StartTransaction(transaction.Id);

```
