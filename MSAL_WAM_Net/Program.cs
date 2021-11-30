using Microsoft.Identity.Client;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSAL_WAM_Net
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.WriteLine(GetAccessToken().Result);
        }

        /// <summary>
        /// From: https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/wam
        /// </summary>
        /// <returns></returns>
        static async Task<String> GetAccessToken()
        {
            IPublicClientApplication pca = PublicClientApplicationBuilder.Create("5f0219ce-2df4-4a71-9cff-b701ad18e0bd")
                .WithBroker(true)
                .WithTenantId("72f988bf-86f1-41af-91ab-2d7cd011db47")
                .Build();

            TokenCacheHelper.EnableSerialization(pca.UserTokenCache);

            IEnumerable<IAccount> accounts = await pca.GetAccountsAsync();
            IAccount accountToLogin = accounts.FirstOrDefault();

            try
            {
                AuthenticationResult authResult = await pca.AcquireTokenSilent(new[] { "User.Read" }, accountToLogin).ExecuteAsync();
                return authResult.AccessToken;

            } catch (MsalUiRequiredException)
            {
                try
                {
                    // for non-console apps, you must switch to the ui thread before making this call
                    AuthenticationResult authResult = await pca.AcquireTokenInteractive(new[] { "User.Read" })
                        .WithAccount(accountToLogin)
                        // .WithParentActivityOrWindow()
                        .ExecuteAsync();
                    return authResult.AccessToken;
                } catch (Exception ex)
                {
                    Console.WriteLine($"Error during authentication: {ex.Message}");
                    return String.Empty;
                }


            }

        }
    }
}
