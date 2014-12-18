aerogear-windows-oauth2
=======================

OAuth2 Client for windows phone. Taking care of:

* account manager for multiple OAuth2 accounts
* request access and refresh token
* grant access through secure external browser and URI schema to re-enter app
* (implicit or explicit) refresh tokens
* permanent secure storage

Example Usage
-------------

```csharp
var config = new Config()                                                         //[1]
{
    baseURL = new Uri("https://accounts.google.com/"),
    authzEndpoint = "o/oauth2/auth",
    redirectURL = "com.aerogear.oauth.test:/oauth2Callback",
    accessTokenEndpoint = "o/oauth2/token",
    refreshTokenEndpoint = "o/oauth2/token",
    revokeTokenEndpoint = "rest/revoke",
    clientId = "517285908032-8m6kbdccps1tpsnsrb5281sglvb2qo9g.apps.googleusercontent.com",
    scopes = new List<string>(new string[] { "https://www.googleapis.com/auth/drive" }),
    accountId = "google"
};

var module = AccountManager.AddAccount(config);                                   //[2]
await module.RequestAccess();

var request = AuthzWebRequest.Create("https://www.googleapis.com/upload/drive/v2/files", module);    //[3]
request.Method = "POST";

using (var postStream = await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, request))
{
    using (var stream = await file.OpenAsync(FileAccessMode.Read))
    {
        Stream s = stream.AsStreamForRead();
        s.CopyTo(postStream);
    }
}
```
Fill-in the OAuth2 configuration [1] this is an example for google.

Create an OAuth2Module from AccountManager's factory method in [2].

Inject OAuth2Module WebRequest http object in [3] and uses the WebRequest to GET/POST etc...

Because of the browser dance you'll have to add the following to OnActivated. When the application gets relaunced with the token from the browser.
```csharp
protected async override void OnActivated(IActivatedEventArgs args)
{
    if (args.Kind == ActivationKind.Protocol)
    {
        ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
        var module = AccountManager.GetAccountByName(currentAccountName);
        await module.ExtractCode(eventArgs.Uri.Query);
    }
    
    ...
```
