# components.yml vs DataObjects discrepancies

_All discrepancies assume `components.yml` is the source of truth. File paths are relative to the repository root._

## Top-level transaction models
- `CreateTransactionRequest` exists in `components.yml` but there is no matching POCO in `src/SignhostAPIClient/Rest/DataObjects`. The runtime uses `Transaction`/`Signer`/`Receiver` directly, so schema rules such as `required: Seal`, the `SignRequestMode` enum (0/1/2), or receiver message requirements cannot be enforced in code.
- `Transaction` (`src/SignhostAPIClient/Rest/DataObjects/Transaction.cs`) lacks schema properties `ModifiedDateTime`, `CancelationReason` (note the single "l" in the schema) and exposes `CancelledDateTime`/`CancellationReason` (double "l") instead, so serialized JSON names drift from the OpenAPI contract.
- `Transaction.SignRequestMode` is a plain `int` and currently allows `0` even though the schema restricts the response values to `{1, 2}`; conversely the create schema allows `{0,1,2}` but there is no validation layer in the POCO.
- `Transaction.Context`/`Signer.Context`/`Receiver.Context` are typed as `dynamic`, which allows arrays, primitives, or nulls while the schema explicitly restricts them to JSON objects (`additionalProperties: true`).

## Signer and receiver models
- There is no separate `CreateSignerRequest` model. `Signer` (`src/SignhostAPIClient/Rest/DataObjects/Signer.cs`) is reused for both request and response payloads, so request-only constraints (e.g., `SendSignRequest` default `true`, prohibiting `AllowDelegation` in combination with `Authentications`) and response-only fields cannot be represented per the schema.
- The `Signer` POCO exposes response-only properties such as `SignUrl`, `DelegateSignUrl`, `ScribbleName`, and `ScribbleNameFixed` that are not part of either `CreateSignerRequest` or `Signer` schemas. At the same time it is missing schema fields `SignedDateTime`, `RejectDateTime`, `CreatedDateTime`, `SignerDelegationDateTime`, `ModifiedDateTime`, `ShowUrl`, and `ReceiptUrl`.
- `Signer.SendSignConfirmation` is nullable (`bool?`) whereas both schemas treat it as a concrete boolean (defaulting to `SendSignRequest`).
- `Signer.DaysToRemind` is nullable with no minimum validation; the schema requires an integer `>= -1` and declares the default `7`.
- There is no `CreateReceiverRequest` class. `Receiver` (`src/SignhostAPIClient/Rest/DataObjects/Receiver.cs`) omits schema fields `Id`, `CreatedDateTime`, and `ModifiedDateTime`, and does not enforce the `Message` requirement that the create schema mandates.

## Activity models
- `Activity` (`src/SignhostAPIClient/Rest/DataObjects/Activity.cs`) is missing the `Activity` string property that the schema exposes for human-readable descriptions.
- `Activity.Code` uses the `ActivityType` enum (`src/SignhostAPIClient/Rest/DataObjects/ActivityType.cs`) which defines many additional codes (e.g., `InvitationReceived=102`, `IdentityApproved=110`, `Finished=500`, `EmailBounceHard=901`, etc.) that do not appear in `components.yml`. Conversely, the schema constrains `Code` to a smaller set, so the enum values and schema enumerations are out of sync.

## File metadata & attachment models
- `FileEntry` references `FileLink` objects (`src/SignhostAPIClient/Rest/DataObjects/FileEntry.cs` and `FileLink.cs`) while the schema names the nested type `Link`. Although the properties match, the type name drift means the generated contract does not reflect the C# surface area.
- `FileMeta` (`src/SignhostAPIClient/Rest/DataObjects/FileMeta.cs`) is the implementation of `FileMetadata`, but `Field.Value` (`src/SignhostAPIClient/Rest/DataObjects/Field.cs`) is hard-coded as a `string`. The schema allows numbers, booleans, or objects in addition to strings, so typed values cannot be represented correctly.
- `Field.Type` is an unconstrained `string`, but the schema constrains it to the `FileFieldType` enum (`Seal`, `Signature`, `Check`, `Radio`, `SingleLine`, `Number`, `Date`).

## Authentication and verification types
- The client only has a single `IVerification` interface (`src/SignhostAPIClient/Rest/DataObjects/IVerification.cs`) that backs both `Authentications` and `Verifications`. `components.yml` differentiates between `SignerAuthentication` (currently only PhoneNumber + DigiD) and `SignerVerification` (15 discriminators with ordering rules). The POCO layer cannot express this split, so contracts that rely on the discriminator sets cannot be modeled.
- Missing discriminators: there is no implementation for `SignerEmailVerification` or `SignerEHerkenningVerification`, so those schema options cannot be produced or consumed.
- Extra discriminators: `ItsmeSignVerification.cs`, `KennisnetVerification.cs`, `SigningCertificateVerification.cs`, and `UnknownVerification.cs` exist in code but are absent from the schema, so they would produce undocumented payloads.
- `PhoneNumberVerification` lacks the `SecureDownload` flag that `SignerPhoneNumberIdentification` requires.
- `DigidVerification` omits the schema properties `Betrouwbaarheidsniveau` and `SecureDownload`.
- `IdealVerification` exposes writable `AccountHolderName`/`AccountHolderCity` properties that are not part of `SignerIDealVerification`.
- `SurfnetVerification` has no `Uid` or `Attributes` even though the schema marks both as read-only fields.
- `IdinVerification` marks every schema field (`AccountHolderName`, `AccountHolderAddress*`, `AccountHolderDateOfBirth`, `Attributes`) as writable and even requires `AccountHolderDateOfBirth` (`DateTime` non-nullable), whereas the schema states they are read-only and optional.
- `IPAddressVerification` aligns with `SignerIPAddressVerification`.
- `EHerkenning` verification is completely missing, so the `SignerEHerkenningVerification` schema entry cannot be serialized.
- `EidasLoginVerification` models `Level` as a custom enum (`Level.cs`) and exposes all properties as writable, conflicting with the schema that documents string values (`de-DE` style) and `readOnly: true` fields.
- `ItsmeIdentificationVerification` omits the `Attributes` dictionary (`readOnly`) defined in the schema.
- `CscVerification` treats `Provider`, `Issuer`, `Subject`, `Thumbprint`, and `AdditionalUserData` as writable, while the schema marks them `readOnly`.
- `OidcVerification` matches `SignerOidcIdentification`.
- `OnfidoVerification` requires GUID `WorkflowId` and `WorkflowRunId` plus writable `Version`/`Attributes`. The schema expects `WorkflowId` as a client-supplied UUID string, but `WorkflowRunId`, `Version`, and `Attributes` are read-only service outputs.

## Transaction cancellation options
- The schema names the object `TransactionDeleteOptions`, but the POCO is `DeleteTransactionOptions` (`src/SignhostAPIClient/Rest/DataObjects/DeleteTransactionOptions.cs`). While the properties align, the type name mismatch leaks to any generated clients.
