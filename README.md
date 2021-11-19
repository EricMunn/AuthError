# AuthError

This is an MRE for an issue with a custom auth provider and `[RequiredPermission]` in ServiceStack 5.13.0.
It is based on a ServiceStack starter template. Visual Studio 2019, .NET 5.0

In 5.12, authenticated requests to `/Hello` return a `200 OK` response.
In 5.13, identical requests return  a `403 Invalid Permission` response, but nevertheless also return results from the service.

## v5.12 Example (v5.12 branch):

```
POST https://localhost:44369/Hello/World HTTP/1.1
Accept: application/json
Authorization: Basic foo:bar
content-type: application/json
```
returns
```
HTTP/1.1 200 OK
Transfer-Encoding: chunked
Content-Type: application/json; charset=utf-8
Vary: Accept
Server: Microsoft-IIS/10.0
Set-Cookie: ss-id=GuV90odr99aHfdCeAq7A; path=/; secure; samesite=lax; httponly,ss-pid=xAJhbbK1ux6VrNQrFzKq; expires=Tue, 19 Nov 2041 20:45:58 GMT; path=/; secure; samesite=lax; httponly,ss-opt=temp; expires=Tue, 19 Nov 2041 20:45:58 GMT; path=/; secure; samesite=lax; httponly
X-Powered-By: ServiceStack/5.120 NetCore/Windows, ASP.NET
Date: Fri, 19 Nov 2021 20:46:00 GMT
Connection: close

{
  "result": "Hello, World!"
}
```

## v5.13 Example (master branch):
```
POST https://localhost:44369/Hello/World HTTP/1.1
Accept: application/json
Authorization: Basic foo:bar
content-type: application/json
```
returns
```
HTTP/1.1 403 Invalid Permission
Transfer-Encoding: chunked
Content-Type: text/plain
Vary: Accept
Server: Microsoft-IIS/10.0
Set-Cookie: ss-id=XZF9s1YIegcFBqmOtTwt; path=/; secure; samesite=lax; httponly,ss-pid=I6XK5t7KuENQxIu2Mect; expires=Tue, 19 Nov 2041 21:20:26 GMT; path=/; secure; samesite=lax; httponly,ss-opt=temp; expires=Tue, 19 Nov 2041 21:20:26 GMT; path=/; secure; samesite=lax; httponly
X-Powered-By: ServiceStack/5.130 NetCore/Windows, ASP.NET
Date: Fri, 19 Nov 2021 21:20:31 GMT
Connection: close

Forbidden

Request.HttpMethod: POST
Request.PathInfo: /Hello/World
Request.QueryString: 

{"result":"Hello, World!"}
```

Not sure if this is related, but debugging with SourceLink in VS 2019 evinces some odd behavior in [AuthenticateAttribute.AuthenticateAsync](https://github.com/ServiceStack/ServiceStack/blob/4421395b0328b081fb24f198211ae7828d111d17/src/ServiceStack/AuthenticateAttribute.cs#L124)

```csharp
public static async Task<bool> AuthenticateAsync(IRequest req, object requestDto=null, IAuthSession session=null, IAuthProvider[] authProviders=null)
{
    if (HostContext.HasValidAuthSecret(req))
        return true;

    session ??= await (req ?? throw new ArgumentNullException(nameof(req))).GetSessionAsync().ConfigAwait();
    authProviders ??= AuthenticateService.GetAuthProviders();
    var authValidate = HostContext.GetPlugin<AuthFeature>()?.OnAuthenticateValidate;
    var ret = authValidate?.Invoke(req);
    if (ret != null)
        return false;

    req.PopulateFromRequestIfHasSessionId(requestDto);

    if (!req.Items.ContainsKey(Keywords.HasPreAuthenticated))
```

The statement at line 131, 

`var authValidate = HostContext.GetPlugin<AuthFeature>()?.OnAuthenticateValidate;`

executes and immediately jumps to line 134:

`return false;`

However, execution continues with line 138, having skipped line 136:

`req.PopulateFromRequestIfHasSessionId(requestDto);`
  


