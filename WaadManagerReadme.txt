
WAADMANAGER

README.TXT



In order to get the demo running, you will need to do the following:

1. Sign up for Datownia and publish the two excel documents in the Solution Items folder.
- You may wish to alter the values within these spreadsheets, as they are currently filled with placeholder data.
- http://www.datownia.com

2. Sign up for a Windows Azure Active Directory account and set up an application for multi-tenant authentication
- http://www.windowsazure.com/en-us/home/features/identity/

2. Create a local sql database named:
WindowsAzureAD_Example
- sql express will do
- there are scripts provided under the DbScripts folder for each of the four required tables
- the user credentials can be found in the Web.config or ConnectionStrings.config files.

3. There are a number of configuration values that you will need to enter.
- Some data can be found in your datownia account and some in your AD account.

*** WaadManager.WorkerRole/App.config (.Debug & .Release) AND
*** WaadManager.Web/Web.config (.Debug & .Release)
- Replace:
{DATOWNIA APP KEY} with app key from datownia account
{DATOWNIA APP SECRET} with app secret from datownia account
{DATOWNIA DEVELOPER PORTAL TAG} with developer portal tag from datownia account
{AD Tenant Domain}with AD Tenant Domain name (e.g. "yourCompany.onmicrosoft.com")
{App Principal ID} with Client Id of you App Principal
{App password} with password of your App Principal
{APP LOGON URL} with your release configuration web site login url

*** WaadManager.Web/Web.config (.Debug & .Release)
- In order to set up multi-tenant authentication, you must provide several configuration values
- Placeholders have been put in the following locations:
* ida:FederationMetadataLocation, {FEDERATION METADATA DOCUMENT URI}
* ida:Issuer, {IDENTITY ISSUER}
* audience uris, {APP URI}
* system.identityModel/issuerNameRegistry/authority, {ACS AUTHORITY}
* system.identityModel/issuerNameRegistry/authority/keys, {THUMBPRINT}
* system.identityModel/issuerNameRegistry/authority/validIssuers, {ISSUER NAME}
* system.identityModel.services/federationConfiguration/wsFederation, {ISSUER}
* system.identityModel.services/federationConfiguration/wsFederation, {REALM}
- To find out more about the values that go into these sections, please refer to the documentation here:
- http://msdn.microsoft.com/en-us/library/windowsazure/dn151789.aspx
