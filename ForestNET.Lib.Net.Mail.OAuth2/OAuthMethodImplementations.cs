using MailKit;
using MailKit.Security;

using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;

using Microsoft.Identity.Client;

namespace ForestNET.Lib.Net.Mail.OAuth2
{
    /// <summary>
    /// Collection of OAuth2 method implementations to authenticate asynchronously at mail/smtp-server
    /// </summary>
    public class OAuthMethodImplementations
    {
        /// <summary>
        /// Static method to authentiate with OAuth2 at Google
        /// </summary>
        /// <param name="p_o_clientInstance">current imap or smtp client instance</param>
        /// <param name="p_s_user">user name</param>
        /// <param name="p_s_cid">client id string</param>
        /// <param name="p_s_csecret">client secret string</param>
        /// <param name="p_o_token">cancellation token of current imap or smtp client instance</param>
        public static async Task OAuthGoogleAsync(MailService p_o_clientInstance, string p_s_user, string p_s_cid, string p_s_csecret, CancellationToken p_o_token)
        {
            /* hold client secrets for OAuth2 authentication */
            var o_clientSecrets = new ClientSecrets
            {
                ClientId = p_s_cid,
                ClientSecret = p_s_csecret
            };

            /* code flow of OAuth2 authentication */
            var o_codeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                DataStore = new FileDataStore("CredentialCacheFolder", false),
                Scopes = new[] { "https://mail.google.com/" },
                ClientSecrets = o_clientSecrets
            });

            /* for a web app, you want to use AuthorizationCodeWebApp instead, but here we use AuthorizationCodeInstalledApp */
            var o_authCode = new AuthorizationCodeInstalledApp(o_codeFlow, new LocalServerCodeReceiver());

            /* get OAuth2 token */
            var o_authToken = await o_authCode.AuthorizeAsync(p_s_user, p_o_token);

            /* check if we must refresh token */
            if (o_authToken.Token.IsStale)
            {
                /* refresh token */
                await o_authToken.RefreshTokenAsync(p_o_token);
            }

            /* create OAuth2 Sasl instance */
            SaslMechanism o_oauth2Sasl;

            if (p_o_clientInstance.AuthenticationMechanisms.Contains("OAUTHBEARER"))
            {
                /* use sasl mechanism OAuth Bearer */
                o_oauth2Sasl = new SaslMechanismOAuthBearer(o_authToken.UserId, o_authToken.Token.AccessToken);
            }
            else
            {
                /* use sasl mechanism OAuth */
                o_oauth2Sasl = new SaslMechanismOAuth2(o_authToken.UserId, o_authToken.Token.AccessToken);
            }

            /* authenticate current client instance */
            await p_o_clientInstance.AuthenticateAsync(o_oauth2Sasl, p_o_token);
        }

        /// <summary>
        /// Static method to authentiate with OAuth2 at Microsoft
        /// </summary>
        /// <param name="p_o_clientInstance">current imap or smtp client instance</param>
        /// <param name="p_s_user">user name</param>
        /// <param name="p_s_cid">client id string</param>
        /// <param name="p_s_csecret">client secret string</param>
        /// <param name="p_o_token">cancellation token of current imap or smtp client instance</param>
        public static async Task OAuthMicrosoftAsync(MailService p_o_clientInstance, string p_s_user, string p_s_cid, string p_s_csecret, CancellationToken p_o_token)
        {
            /* options for public client authentication */
            var o_options = new PublicClientApplicationOptions
            {
                ClientId = p_s_cid,
                TenantId = p_s_csecret,

                // Use "https://login.microsoftonline.com/common/oauth2/nativeclient" for apps using embedded browsers or
                // use "http://localhost" for apps that use system browsers

                //RedirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient"
                RedirectUri = "http://localhost"
            };

            /* build public client authentication instance */
            var o_publicClientApplication = PublicClientApplicationBuilder
                .CreateWithApplicationOptions(o_options)
                .Build();

            /* define scopes for access */
            var a_scopes = new string[] {
                "email",
                "offline_access",
                "https://outlook.office.com/IMAP.AccessAsUser.All", /* Only needed for IMAP */
                /* "https://outlook.office.com/POP.AccessAsUser.All", // Only needed for POP */
                "https://outlook.office.com/SMTP.AccessAsUser.All", // Only needed for SMTP
            };

            /* aquire token instance with login hint */
            var o_authToken = await o_publicClientApplication.AcquireTokenInteractive(a_scopes).WithLoginHint(p_s_user).ExecuteAsync(p_o_token);
            /* aquire token silently */
            await o_publicClientApplication.AcquireTokenSilent(a_scopes, o_authToken.Account).ExecuteAsync(p_o_token);

            /* create OAuth2 Sasl instance */
            SaslMechanism o_oauth2Sasl;

            if (p_o_clientInstance.AuthenticationMechanisms.Contains("OAUTHBEARER"))
            {
                /* use sasl mechanism OAuth Bearer */
                o_oauth2Sasl = new SaslMechanismOAuthBearer(o_authToken.Account.Username, o_authToken.AccessToken);
            }
            else
            {
                /* use sasl mechanism OAuth */
                o_oauth2Sasl = new SaslMechanismOAuth2(o_authToken.Account.Username, o_authToken.AccessToken);
            }

            /* authenticate current client instance */
            await p_o_clientInstance.AuthenticateAsync(o_oauth2Sasl, p_o_token);
        }
    }
}
