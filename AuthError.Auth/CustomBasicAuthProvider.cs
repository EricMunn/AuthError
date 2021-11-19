using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Web;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuthError.Auth
{
    public class CustomBasicAuthProvider : BasicAuthProvider
    {
        public override async Task<bool> TryAuthenticateAsync(IServiceBase authService, string userName, string password, CancellationToken token = default)
        {
            return true;
        }

        public override async Task<IHttpResult> OnAuthenticatedAsync(IServiceBase authService,
        IAuthSession session, IAuthTokens tokens, Dictionary<string, string> authInfo,
        CancellationToken token = default)
        {
            session.Permissions = session.Permissions ?? new List<string>();
            session.Permissions.AddIfNotExists("CustomPermission");

            session.IsAuthenticated = true;
            await authService.SaveSessionAsync(session, SessionExpiry, token);
            authService.Request.Items[Keywords.DidAuthenticate] = true;
            return null;
        }
    }
}
