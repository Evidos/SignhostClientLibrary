using System.Text.Json.Serialization;

namespace Signhost.APIClient.Rest.DataObjects;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(ConsentVerification), "Consent")]
[JsonDerivedType(typeof(DigidVerification), "DigiD")]
[JsonDerivedType(typeof(EidasLoginVerification), "eIDAS Login")]
[JsonDerivedType(typeof(IdealVerification), "iDeal")]
[JsonDerivedType(typeof(IdinVerification), "iDIN")]
[JsonDerivedType(typeof(IPAddressVerification), "IPAddress")]
[JsonDerivedType(typeof(ItsmeIdentificationVerification), "itsme Identification")]
[JsonDerivedType(typeof(PhoneNumberVerification), "PhoneNumber")]
[JsonDerivedType(typeof(ScribbleVerification), "Scribble")]
[JsonDerivedType(typeof(SurfnetVerification), "SURFnet")]
[JsonDerivedType(typeof(CscVerification), "CSC Qualified")]
[JsonDerivedType(typeof(EherkenningVerification), "eHerkenning")]
[JsonDerivedType(typeof(OidcVerification), "OpenID Providers")]
[JsonDerivedType(typeof(OnfidoVerification), "Onfido")]
public interface IVerification
{
}
