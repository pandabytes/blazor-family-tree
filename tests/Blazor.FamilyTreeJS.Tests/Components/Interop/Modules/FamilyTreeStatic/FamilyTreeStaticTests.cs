namespace Blazor.FamilyTreeJS.Tests.Components.Interop.Modules.FamilyTreeStatic;

public class FamilyTreeStaticTests : TestContext
{
  public static IEnumerable<object[]> GetTestIcons()
  {
    var iconsWithXAndYParams = new List<Icon> { Icon.Share, Icon.User, Icon.AddUser, Icon.Close, Icon.Ft };
    foreach (var icon in iconsWithXAndYParams)
    {
      yield return new object[] { icon };
    }
  }

  private readonly BunitJSModuleInterop _mockModule;

  private readonly FamilyTreeStaticModule _testModule;

  public FamilyTreeStaticTests()
  {
    Services.AddTransient<FamilyTreeStaticModule>();
    _mockModule = JSInterop.SetupModule("./_content/Blazor.FamilyTreeJS/js/Components/Interop/Modules/FamilyTreeStatic/family-tree-static.js");

    _testModule = Services.GetRequiredService<FamilyTreeStaticModule>();
  }

  [Theory]
  [MemberData(nameof(GetTestIcons))]
  public async Task GetIconAsync_IconWithXAndYParams_NoException(Icon icon)
  {
    // Arrange
    const string expected = "result";
    _mockModule
      .Setup<string>($"FamilyTreeObj.icon.{icon}", _ => true)
      .SetResult(expected);

    await _testModule.ImportAsync();

    // Act
    var actual = await _testModule.GetIconAsync(icon, "1", "1", "white", x: "0", y: "0");

    // Assert
    Assert.Equal(expected, actual);
  }

  [Fact]
  public async Task GetIconAsync_IconWithXAndYParams_ThrowsException()
  {
    // Arrange
    const string expected = "result";
    var icon = Icon.Csv;

    _mockModule
      .Setup<string>($"FamilyTreeObj.icon.{icon}", _ => true)
      .SetResult(expected);

    await _testModule.ImportAsync();

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(async ()
      => await _testModule.GetIconAsync(icon, "1", "1", "white", x: "0", y: "0"));
  }

  [Fact]
  public async Task GetIconAsync_IconWithXParam_ThrowsException()
  {
    // Arrange
    const string expected = "result";
    var icon = Icon.Pdf;

    _mockModule
      .Setup<string>($"FamilyTreeObj.icon.{icon}", _ => true)
      .SetResult(expected);

    await _testModule.ImportAsync();

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(async ()
      => await _testModule.GetIconAsync(icon, "1", "1", "white", x: "0"));
  }

  [Fact]
  public async Task GetIconAsync_IconWithYParam_ThrowsException()
  {
    // Arrange
    const string expected = "result";
    var icon = Icon.Xml;

    _mockModule
      .Setup<string>($"FamilyTreeObj.icon.{icon}", _ => true)
      .SetResult(expected);

    await _testModule.ImportAsync();

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(async ()
      => await _testModule.GetIconAsync(icon, "1", "1", "white", y: "0"));
  }
}
