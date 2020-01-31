using System;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using Signhost.APIClient.Rest.DataObjects;
using Xunit;

namespace Signhost.APIClient.Rest.Tests
{
	public class PostbackTests
	{
		[Fact]
		public void PostbackTransaction_should_get_serialized_correctly()
		{
			string json = RequestBodies.MockPostbackValid;
			var postbackTransaction = JsonConvert.DeserializeObject<PostbackTransaction>(json);

			postbackTransaction.Id                    .Should().Be("b10ae331-af78-4e79-a39e-5b64693b6b68");
			postbackTransaction.Status                .Should().Be(TransactionStatus.InProgress);
			postbackTransaction.Seal                  .Should().BeTrue();
			postbackTransaction.Reference             .Should().Be("Contract #123");
			postbackTransaction.PostbackUrl           .Should().Be("https://example.com/postback.php");
			postbackTransaction.SignRequestMode       .Should().Be(2);
			postbackTransaction.DaysToExpire          .Should().Be(30);
			postbackTransaction.SendEmailNotifications.Should().BeTrue();
			postbackTransaction.CreatedDateTime       .Should().Be(DateTimeOffset.Parse("2016-08-31T21:22:56.2467731+02:00"));
			postbackTransaction.CancelledDateTime     .Should().BeNull();
			(postbackTransaction.Context is null)     .Should().BeTrue();
			postbackTransaction.Checksum              .Should().Be("cdc09eee2ed6df2846dcc193aedfef59f2834f8d");

			var signers = postbackTransaction.Signers;
			signers.Should().HaveCount(1);

			var signer = signers.Single();
			signer.Id                  .Should().Be("fa95495d-6c59-48e0-962a-a4552f8d6b85");
			signer.Expires             .Should().BeNull();
			signer.Email               .Should().Be("user@example.com");
			signer.SendSignRequest     .Should().BeTrue();
			signer.SendSignConfirmation.Should().BeNull();
			signer.SignRequestMessage  .Should().Be("Hello, could you please sign this document? Best regards, John Doe");
			signer.DaysToRemind        .Should().Be(15);
			signer.Language            .Should().Be("en-US");
			signer.ScribbleName        .Should().Be("John Doe");
			signer.ScribbleNameFixed   .Should().BeFalse();
			signer.Reference           .Should().Be("Client #123");
			signer.ReturnUrl           .Should().Be("https://signhost.com");
			signer.RejectReason        .Should().BeNull();
			signer.SignUrl             .Should().Be("https://view.signhost.com/sign/d3c93bd6-f1ce-48e7-8c9c-c2babfdd4034");
			(signer.Context is null)   .Should().BeTrue();

			var verifications = signer.Verifications;
			verifications.Should().HaveCount(2);

			var scribbleVerification = verifications[0] as ScribbleVerification;
			scribbleVerification                     .Should().NotBeNull();
			scribbleVerification.Type                .Should().Be("Scribble");
			scribbleVerification.RequireHandsignature.Should().BeTrue();
			scribbleVerification.ScribbleNameFixed   .Should().BeFalse();
			scribbleVerification.ScribbleName        .Should().Be("John Doe");

			var ipAddressVerification = verifications[1] as IPAddressVerification;
			ipAddressVerification          .Should().NotBeNull();
			ipAddressVerification.Type     .Should().Be("IPAddress");
			ipAddressVerification.IPAddress.Should().Be("1.2.3.4");

			var activities = signer.Activities;
			activities.Should().HaveCount(3);

			var openedActivity = activities[0];
			openedActivity.Id             .Should().Be("bcba44a9-c201-4494-9920-2c1f7baebcf0");
			openedActivity.Code           .Should().Be(ActivityType.Opened);
			openedActivity.Info           .Should().BeNull();
			openedActivity.CreatedDateTime.Should().Be(DateTimeOffset.Parse("2016-06-15T23:33:04.1965465+02:00"));

			var documentOpenedActivity = activities[1];
			documentOpenedActivity.Id             .Should().Be("7aacf96a-5c2f-475d-98a5-726e41bfc5d3");
			documentOpenedActivity.Code           .Should().Be(ActivityType.DocumentOpened);
			documentOpenedActivity.Info           .Should().Be("file1");
			documentOpenedActivity.CreatedDateTime.Should().Be(DateTimeOffset.Parse("2020-01-30T16:31:05.6679583+01:00"));

			var signedActivity = activities[2];
			signedActivity.Id             .Should().Be("de94cf6e-e1a3-4c33-93bf-2013b036daaf");
			signedActivity.Code           .Should().Be(ActivityType.Signed);
			signedActivity.Info           .Should().BeNull();
			signedActivity.CreatedDateTime.Should().Be(DateTimeOffset.Parse("2016-06-15T23:38:04.1965465+02:00"));

			var receivers = postbackTransaction.Receivers;
			receivers.Should().HaveCount(1);

			var receiver = receivers.Single();
			receiver.Name             .Should().Be("John Doe");
			receiver.Email            .Should().Be("user@example.com");
			receiver.Language         .Should().Be("en-US");
			receiver.Message          .Should().Be("Hello, please find enclosed the digital signed document. Best regards, John Doe");
			receiver.Reference        .Should().BeNull();
			receiver.Activities       .Should().BeNull();
			(receiver.Context is null).Should().BeTrue();

			var files = postbackTransaction.Files;
			files.Should().HaveCount(1);

			var file = files["file1"];
			file.DisplayName.Should().Be("Sample File");

			var links = file.Links;
			links.Should().HaveCount(1);

			var link = links.Single();
			link.Rel .Should().Be("file");
			link.Type.Should().Be("application/pdf");
			link.Link.Should().Be("https://api.signhost.com/api/transaction/b10ae331-af78-4e79-a39e-5b64693b6b68/file/file1");
		}
	}
}
