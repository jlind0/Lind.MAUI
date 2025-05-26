using Lind.MAUI.Example.Security.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.MAUI.Example.Security
{
    public class SecurityProvider : ISecurityProvider
    {
        protected IPublicClientApplication PublicClientApplication { get; }
        protected ILogger Logger { get; }
        protected string SignUpSignInPolicy { get; }
        public SecurityProvider(IPublicClientApplication publicClientApplication, ILogger<SecurityProvider> logger, IConfiguration config)
        {
            PublicClientApplication = publicClientApplication;
            Logger = logger;
            SignUpSignInPolicy = config["SignUpSignInPolicyId"] ?? throw new ArgumentNullException("SignUpSignInPolicyId is not configured in appsettings.json or environment variables.");
        }
        public bool IsAuthenticated{ get; protected set; }

        public string? UserName{ get; protected set; }

        public async Task<string?> GetAccessToken(string[] scopes, CancellationToken token = default)
        {
            try
            {
                if (IsAuthenticated)
                { 
                    var accounts = await PublicClientApplication.GetAccountsAsync(SignUpSignInPolicy).ConfigureAwait(false);
                    if (accounts.Any())
                    {
                        var result = await PublicClientApplication.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                            .ExecuteAsync(token).ConfigureAwait(false);
                        return result.AccessToken;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting access token");
                throw;
            }
            return null;
        }

        public async Task<string?> Login(string[] scopes, bool forceInteractive = false, CancellationToken token = default)
        {
            bool tryInteractive = false;
            string? accessToken = null;
            if (!forceInteractive)
            {
                try
                {
                    var accounts = (await PublicClientApplication.GetAccountsAsync(SignUpSignInPolicy)).ToList();
                    if (accounts.Any())
                    {
                        var result = await PublicClientApplication.AcquireTokenSilent(scopes, accounts.First()).ExecuteAsync();
                        accessToken = result.AccessToken;
                        IsAuthenticated = !string.IsNullOrEmpty(accessToken);
                        tryInteractive = !IsAuthenticated;
                    }
                    else
                        tryInteractive = true;
                }
                catch (MsalUiRequiredException)
                {
                    tryInteractive = true;
                }
                catch (Exception ex)
                {
                   Logger.LogError(ex, "Error acquiring token silently");
                   throw;
                }
            }
            else
                tryInteractive = true;
            if (tryInteractive)
            {
                try
                {
                    var result = await PublicClientApplication.AcquireTokenInteractive(scopes)
                                .ExecuteAsync(token);
                    accessToken = result.AccessToken;
                    IsAuthenticated = !string.IsNullOrEmpty(accessToken);

                }
                catch (Exception ex)
                {
                   Logger.LogError(ex, "Error acquiring token interactively");
                   throw;
                }
            }
            if (IsAuthenticated)
            {
                UserName = (await PublicClientApplication.GetAccountsAsync(SignUpSignInPolicy)).FirstOrDefault()?.Username;
            }
            return accessToken;
        }

        public async Task Logout(CancellationToken token = default)
        {
            try
            {
                await PublicClientApplication.RemoveAsync((await PublicClientApplication.GetAccountsAsync(SignUpSignInPolicy)).FirstOrDefault()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error logging out");
                throw;
            }
            finally
            {
                IsAuthenticated = false;
                UserName = null;
            }
        }
    }
}
