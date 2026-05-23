using MudBlazor.Services;

public abstract class BlazorTestContext : TestContext, IDisposable
{
    protected readonly MockHttpMessageHandler MockHttp;
    protected readonly IRenderedComponent<MudSnackbarProvider> SnackbarProvider;
    protected readonly IRenderedComponent<MudDialogProvider> DialogProvider;

    protected BlazorTestContext()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;

        MockHttp = new MockHttpMessageHandler();
        var httpClient = MockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");
        Services.AddSingleton(httpClient);
        Services.AddMudServices();

        RenderComponent<MudPopoverProvider>();
        SnackbarProvider = RenderComponent<MudSnackbarProvider>();
        DialogProvider = RenderComponent<MudDialogProvider>();
    }
}
