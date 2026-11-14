using Web3CollectionStudio.Domain.Collections;
using Web3CollectionStudio.Domain.Layers;
using Web3CollectionStudio.Domain.Rules;
using Web3CollectionStudio.Domain.Rendering;

namespace Web3CollectionStudio.Domain.Tests;

public sealed class CollectionProjectContractTests
{
    [Fact]
    public void CreatePfpProject_assigns_stable_local_identity_and_square_canvas()
    {
        var project = CollectionProject.Create(
            "Neon Creatures",
            @"C:\Collections\NeonCreatures",
            CollectionProfile.Pfp);

        Assert.NotEqual(Guid.Empty, project.Id);
        Assert.Equal("Neon Creatures", project.DisplayName);
        Assert.Equal(@"C:\Collections\NeonCreatures", project.LocalRoot);
        Assert.Equal(1024, project.Canvas.Width);
        Assert.Equal(1024, project.Canvas.Height);
        Assert.Equal(CollectionProfile.Pfp, project.Profile);
    }

    [Fact]
    public void CreateDigitalArtProject_preserves_a_valid_custom_canvas()
    {
        var project = CollectionProject.Create(
            "Launch Artwork",
            @"C:\Collections\LaunchArtwork",
            CollectionProfile.DigitalArt,
            new CanvasSpec(1600, 900));

        Assert.Equal(1600, project.Canvas.Width);
        Assert.Equal(900, project.Canvas.Height);
    }

    [Fact]
    public void CreateProject_rejects_a_canvas_with_a_zero_dimension()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CollectionProject.Create(
            "Invalid Canvas",
            @"C:\Collections\InvalidCanvas",
            CollectionProfile.DigitalArt,
            new CanvasSpec(0, 900)));
    }

    [Fact]
    public void CreateLayer_creates_an_ordered_trait_option_with_weight_and_relative_png_path()
    {
        var layer = CollectionLayer.Create("Background", 10);
        var trait = layer.AddTrait("Indigo", "assets/background/indigo.png", 35);

        Assert.Equal("Background", layer.Name);
        Assert.Equal(10, layer.DrawOrder);
        Assert.Equal("Indigo", trait.DisplayName);
        Assert.Equal("assets/background/indigo.png", trait.RelativeAssetPath);
        Assert.Equal(35, trait.Weight);
        Assert.NotEqual(Guid.Empty, trait.Id);
    }

    [Fact]
    public void AddTrait_rejects_a_blank_display_name()
    {
        var layer = CollectionLayer.Create("Background", 10);

        Assert.Throws<ArgumentException>(() => layer.AddTrait("   ", "assets/background/indigo.png", 35));
    }

    [Theory]
    [InlineData(@"C:\CollectionAssets\indigo.png")]
    [InlineData("assets/background/indigo.jpg")]
    public void AddTrait_rejects_an_asset_path_that_is_not_relative_local_png(string assetPath)
    {
        var layer = CollectionLayer.Create("Background", 10);

        Assert.Throws<ArgumentException>(() => layer.AddTrait("Indigo", assetPath, 35));
    }

    [Fact]
    public void AddTrait_rejects_a_negative_weight()
    {
        var layer = CollectionLayer.Create("Background", 10);

        Assert.Throws<ArgumentOutOfRangeException>(() => layer.AddTrait("Indigo", "assets/background/indigo.png", -1));
    }

    [Fact]
    public void Layer_tracks_traits_and_rejects_a_duplicate_asset_path()
    {
        var layer = CollectionLayer.Create("Background", 10);
        var first = layer.AddTrait("Indigo", "assets/background/indigo.png", 35);

        Assert.Throws<InvalidOperationException>(() => layer.AddTrait("Indigo Copy", "assets/background/indigo.png", 20));
        Assert.Equal([first], layer.Traits);
    }

    [Fact]
    public void Project_owns_layers_in_draw_order_and_rejects_duplicate_draw_order()
    {
        var project = CollectionProject.Create("Neon Creatures", @"C:\Collections\NeonCreatures", CollectionProfile.Pfp);
        var foreground = project.AddLayer("Foreground", 20);
        var background = project.AddLayer("Background", 10);

        Assert.Equal([background, foreground], project.Layers);
        Assert.Throws<InvalidOperationException>(() => project.AddLayer("Body", 20));
    }

    [Fact]
    public void RequireRule_rejects_a_dependent_trait_when_its_required_trait_is_missing()
    {
        var roboticBody = Guid.NewGuid();
        var goldVisor = Guid.NewGuid();
        var rule = CompatibilityRule.Require(goldVisor, roboticBody);

        Assert.False(rule.IsSatisfiedBy(new HashSet<Guid> { goldVisor }));
        Assert.True(rule.IsSatisfiedBy(new HashSet<Guid> { goldVisor, roboticBody }));
    }

    [Fact]
    public void ExcludeRule_rejects_a_selection_that_contains_both_conflicting_traits()
    {
        var roboticBody = Guid.NewGuid();
        var spectralAura = Guid.NewGuid();
        var rule = CompatibilityRule.Exclude(roboticBody, spectralAura);

        Assert.False(rule.IsSatisfiedBy(new HashSet<Guid> { roboticBody, spectralAura }));
        Assert.True(rule.IsSatisfiedBy(new HashSet<Guid> { roboticBody }));
    }

    [Fact]
    public void GenerationFingerprint_is_stable_when_the_same_traits_arrive_in_a_different_order()
    {
        var firstTrait = Guid.NewGuid();
        var secondTrait = Guid.NewGuid();

        var first = GenerationFingerprint.FromTraits(new[] { firstTrait, secondTrait });
        var second = GenerationFingerprint.FromTraits(new[] { secondTrait, firstTrait });

        Assert.Equal(first.Value, second.Value);
    }
}
