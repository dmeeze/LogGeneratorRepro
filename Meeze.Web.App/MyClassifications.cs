using System.Text;
using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Extensions.Compliance.Redaction;

namespace Web.Classfications;

public static class MyClassifications
{
    public static string Name => nameof(MyClassifications);

    public static DataClassification Secret => new(Name, nameof(Secret));
    public static DataClassification Known => new(Name, nameof(Known));
}

public sealed class SecretAttribute() : DataClassificationAttribute(MyClassifications.Secret);

public sealed class KnownAttribute() : DataClassificationAttribute(MyClassifications.Known);

public sealed class Base64Redactor : Redactor
{
    public override int GetRedactedLength(ReadOnlySpan<char> input) => (Encoding.UTF8.GetByteCount(input) + 2) / 3 * 4;

    public override int Redact(ReadOnlySpan<char> source, Span<char> destination)
    {
        var obfuscated = Convert.ToBase64String(Encoding.UTF8.GetBytes(source.ToString()));
        return obfuscated.TryCopyTo(destination) ? obfuscated.Length : throw new InvalidOperationException($"Buffer too small, needed a size of {obfuscated.Length} but got {destination.Length}");
    }
}