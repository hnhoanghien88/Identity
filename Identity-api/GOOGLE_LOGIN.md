# Google external login

Google verifies the external identity. The Identity API then creates or loads the
internal User, assigns the configured default Role, and issues the existing
refresh-token/JWT session.

## Provisioning behavior

- A verified Google email is required.
- An existing link is resolved by `(Provider, ProviderSubject)`, never by email.
- A new identity creates `users`, `external_identities`, and `user_roles` records
  in one transaction.
- A new identity is rejected when its email already belongs to another User.
- Social Users have `PasswordHash = null` and cannot use Code/password login.
- The configured default Role must already exist, be active, and belong to the
  configured JWT Application.

## Google Cloud configuration

Create an OAuth 2.0 Web application in Google Cloud and register:

```text
Authorized JavaScript origin: http://localhost:5173
Authorized redirect URI:     https://localhost:7203/signin-google
```

The redirect URI must match the HTTPS API address used by the selected launch
profile. Use the deployed API origin for staging and production.

Store credentials outside committed configuration:

```powershell
cd Identity-api/src/Identity.Api
dotnet user-secrets init
dotnet user-secrets set "ExternalAuthentication:Google:Enabled" "true"
dotnet user-secrets set "ExternalAuthentication:Google:ClientId" "<client-id>"
dotnet user-secrets set "ExternalAuthentication:Google:ClientSecret" "<client-secret>"
dotnet user-secrets set "ExternalAuthentication:Google:DefaultRoleCode" "Viewer"
```

For deployed environments, use the platform secret store or environment variables:

```text
ExternalAuthentication__Google__Enabled=true
ExternalAuthentication__Google__ClientId=<client-id>
ExternalAuthentication__Google__ClientSecret=<client-secret>
ExternalAuthentication__Google__DefaultRoleCode=Viewer
ExternalAuthentication__FrontendLoginUrl=https://identity.example.com/login
```

Before the first login, apply migrations and ensure the `Identity-api`
Application contains an active non-administrative Role with the configured Code.

```powershell
dotnet ef database update --project ../Identity.Infrastructure --startup-project .
```

When Google is disabled, the frontend discovers that state from
`GET /external-login/providers` and disables the Google button.
