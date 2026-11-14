using System;
using System.IO;
using SkiaSharp;

namespace Web3CollectionStudio.Desktop.Tests;

public sealed class FlatPrismaticLBrandContractTests
{
    [Fact]
    public void Product_windows_use_flat_prismatic_l_and_do_not_retain_globe_identity()
    {
        var projectRoot = FindProjectRoot();
        var desktopRoot = Path.Combine(projectRoot, "src", "Web3CollectionStudio.Desktop");
        var login = File.ReadAllText(Path.Combine(desktopRoot, "Views", "LoginWindow.axaml"));
        var splash = File.ReadAllText(Path.Combine(desktopRoot, "Views", "SplashWindow.axaml"));
        var main = File.ReadAllText(Path.Combine(desktopRoot, "Views", "MainWindow.axaml"));
        var splashCode = File.ReadAllText(Path.Combine(desktopRoot, "Views", "SplashWindow.axaml.cs"));

        Assert.Contains("/Assets/Brand/nft-studio-prismatic-l.png", login, StringComparison.Ordinal);
        Assert.Contains("/Assets/Brand/nft-studio-prismatic-l.png", splash, StringComparison.Ordinal);
        Assert.Contains("/Assets/Brand/nft-studio-prismatic-l.png", main, StringComparison.Ordinal);
        Assert.DoesNotContain("globe", login, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("globe", splash, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("RotateGlobe", splashCode, StringComparison.Ordinal);
    }

    [Fact]
    public void Runtime_prismatic_l_asset_has_true_transparent_background()
    {
        var projectRoot = FindProjectRoot();
        var assetPath = Path.Combine(
            projectRoot,
            "src",
            "Web3CollectionStudio.Desktop",
            "Assets",
            "Brand",
            "nft-studio-prismatic-l.png");

        using var bitmap = SKBitmap.Decode(assetPath);
        Assert.NotNull(bitmap);

        var backgroundPixels = new[]
        {
            bitmap!.GetPixel(0, 0),
            bitmap.GetPixel(bitmap.Width - 1, 0),
            bitmap.GetPixel(0, bitmap.Height - 1),
            bitmap.GetPixel(bitmap.Width - 1, bitmap.Height - 1),
            bitmap.GetPixel(bitmap.Width / 8, bitmap.Height / 8),
            bitmap.GetPixel(bitmap.Width * 7 / 8, bitmap.Height / 8),
            bitmap.GetPixel(bitmap.Width / 8, bitmap.Height * 7 / 8),
            bitmap.GetPixel(bitmap.Width * 7 / 8, bitmap.Height * 7 / 8)
        };

        Assert.All(backgroundPixels, pixel => Assert.Equal((byte)0, pixel.Alpha));

        var opaqueNeutralLightPixelCount = 0;
        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                var minimum = Math.Min(pixel.Red, Math.Min(pixel.Green, pixel.Blue));
                var maximum = Math.Max(pixel.Red, Math.Max(pixel.Green, pixel.Blue));
                if (pixel.Alpha == 255 && minimum >= 180 && maximum - minimum <= 12)
                {
                    opaqueNeutralLightPixelCount++;
                }
            }
        }

        Assert.Equal(0, opaqueNeutralLightPixelCount);
    }

    private static string FindProjectRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Web3CollectionStudio.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Web3 Collection Studio project root was not found.");
    }
}
