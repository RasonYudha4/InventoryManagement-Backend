using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace InventoryManagement.UI.Services; // Adjust to match your folder

public class JwtInterceptor(
    ILocalStorageService localStorage,
    AuthenticationStateProvider authStateProvider,
    NavigationManager navigation
    ) : DelegatingHandler

{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 1. Grab the token from browser storage
        var token = await localStorage.GetItemAsync<string>("authToken");

        // 2. If the user is logged in, attach the Bearer token!
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await localStorage.RemoveItemAsync("authToken");
            ((CustomAuthStateProvider)authStateProvider).NotifyUserLogout();
            navigation.NavigateTo("/login");
        }

        return response;
    }
}