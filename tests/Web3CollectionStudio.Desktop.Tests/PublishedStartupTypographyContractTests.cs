using System;
using System.IO;
using Xunit;

namespace Web3CollectionStudio.Desktop.Tests;

public sealed class PublishedStartupTypographyContractTests
{
    [Fact]
    public void AppResources_DoNotReferenceAnUnbundledInterFontAsset()
    {
        var appAxaml = FindFromTestAssembly("src", "Web3CollectionStudio.Desktop", "App.axaml");
        var resources = File.ReadAllText(appAxaml);

        Assert.DoesNotContain("Assets/Fonts#Inter", resources, StringComparison.Ordinal);
    }

    private static string FindFromTestAssembly(params string[] relativeSegments)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine([directory.FullName, .. relativeSegments]);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException("Unable to locate Collection Studio App.axaml from the test assembly.");
    }
}
