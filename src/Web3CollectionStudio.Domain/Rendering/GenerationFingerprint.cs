using System.Security.Cryptography;
using System.Text;

namespace Web3CollectionStudio.Domain.Rendering;

public sealed record GenerationFingerprint(string Value)
{
    public static GenerationFingerprint FromTraits(IEnumerable<Guid> traitIds)
    {
        var orderedIds = traitIds
            .OrderBy(identifier => identifier)
            .Select(identifier => identifier.ToString("N"));
        var payload = Encoding.UTF8.GetBytes(string.Join("|", orderedIds));
        var hash = SHA256.HashData(payload);

        return new GenerationFingerprint(Convert.ToHexString(hash));
    }
}
