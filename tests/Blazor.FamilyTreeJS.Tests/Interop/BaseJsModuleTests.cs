// using Microsoft.JSInterop;

// namespace Blazor.Core.Tests.Interop;

// public class BaseJsModuleTests : TestContext, IAsyncLifetime
// {
//   private const string DummyModulePath = "test.js";

//   private const string DummyMethodName = "testFunction";

//   private class TestJsModule : BaseJsModule
//   {
//     protected override string ModulePath => DummyModulePath;

//     public TestJsModule(IJSRuntime jSRuntime) : base(jSRuntime)
//     {}

//     public async Task DummyFunctionAsync()
//       => await Module.InvokeVoidAsync(DummyMethodName);
//   }

//   private readonly BunitJSModuleInterop _mockJsModule;

//   private readonly TestJsModule _testModule;

//   public BaseJsModuleTests()
//   {
//     Services.AddTransient<TestJsModule>();
//     _mockJsModule = JSInterop.SetupModule(DummyModulePath);

//     _testModule = Services.GetRequiredService<TestJsModule>();
//   }

//   public Task InitializeAsync() => Task.CompletedTask;

//   public async Task DisposeAsync()
//   {
//     await _testModule.DisposeAsync();
//   }

//   [InlineData(2)]
//   [InlineData(5)]
//   [Theory]
//   public async Task BaseJsModule_ImportManyTimes_NoExceptionThrown(int importTimes)
//   {
//     for (int i = 0; i < importTimes; i++)
//     {
//       await _testModule.ImportAsync();
//     }
//   }

//   [InlineData(2)]
//   [InlineData(5)]
//   [Theory]
//   public async Task BaseJsModule_DisposeManyTimes_NoExceptionThrown(int disposeTimes)
//   {
//     for (int i = 0; i < disposeTimes; i++)
//     {
//       await _testModule.DisposeAsync();
//     }
//   }

//   [Fact]
//   public async Task BaseJsModule_ImportAfterDispose_ThrowsException()
//   {
//     // Arrange
//     await _testModule.ImportAsync();

//     // Act
//     await _testModule.DisposeAsync();

//     // Assert
//     await Assert.ThrowsAsync<InvalidOperationException>(_testModule.ImportAsync);
//   }

//   [Fact]
//   public async Task BaseJsModule_CallMethodBeforeImport_ThrowsException()
//   {
//     await Assert.ThrowsAsync<InvalidOperationException>(_testModule.DummyFunctionAsync);
//   }

//   [Fact(Skip = "Test hangs")]
//   public async Task BaseJsModule_CallMethodAfterImport_NoExceptionThrown()
//   {
//     // Arrange
//     await _testModule.ImportAsync();
//     _mockJsModule.SetupVoid(DummyMethodName);

//     // Act
//     await _testModule.DummyFunctionAsync();
//   }

//   [Fact]
//   public async Task BaseJsModule_CallMethodAfterDispose_ThrowsException()
//   {
//     // Arrange
//     await _testModule.ImportAsync();
//     await _testModule.DisposeAsync();

//     // Act & Assert
//     await Assert.ThrowsAsync<InvalidOperationException>(_testModule.DummyFunctionAsync);
//   }
// }
