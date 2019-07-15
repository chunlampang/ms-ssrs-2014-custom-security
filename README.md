# Microsoft SQL Server Reporting Services 2014 Custom Security Sample
As the other sample SQL server version are < 2014 or > 2014, I created this sample.

## Download Microsoft SQL Server 2014 + Reporting Services
Download and install ExpressAdv from:
https://www.microsoft.com/en-us/download/details.aspx?id=42299

The following path will use &#x1F538; to represent the install path.
The default install path may look like:
C:\Program Files\Microsoft SQL Server\MSRS12.MSSQLSERVER\Reporting Services

## Original Source
https://archive.codeplex.com/?p=msftrsprodsamples

I have removed the connect database part from the sample, and hard coded the user verification to true. You may find "todoooooooooooooooooooo" from the code and replace with your custom security rule.

## Deployment
If the Microsoft.ReportingServices.Interfaces is missing, add it from &#x1F538;\ReportServer\bin\Microsoft.ReportingServices.Interfaces.dll

Build and copy the following file to the specific directory.
Logon.aspx -> &#x1F538;\ReportServer
UILogon.aspx -> &#x1F538;\ReportMnager\Pages
Microsoft.Samples.ReportingServices.CustomSecurity.dll, Microsoft.Samples.ReportingServices.CustomSecurity.pdb -> &#x1F538;\ReportServer\bin, &#x1F538;\ReportManager\bin

## Configuration
Before modify the configuration files, please create a copy for backup.

### &#x1F538;\ReportServer\rsreportserver.config
Replace Configuration.Authentication to:
```
<Authentication>
    <AuthenticationTypes>
        <Custom/>
    </AuthenticationTypes>
    <EnableAuthPersistence>true</EnableAuthPersistence>
    <RSWindowsExtendedProtectionLevel>Off</RSWindowsExtendedProtectionLevel>
    <RSWindowsExtendedProtectionScenario>Proxy</RSWindowsExtendedProtectionScenario>
</Authentication>
```

Replace Configuration.Extensions.Security to:
(For the AdminConfiguration.UserName, you may replace the value to your admin user name.)
(Use need to assess the Report Manager by admin account for kick-up.)
```
<Security>
    <Extension Name="Forms"   Type="Microsoft.Samples.ReportingServices.CustomSecurity.Authorization, Microsoft.Samples.ReportingServices.CustomSecurity" >
        <Configuration>
            <AdminConfiguration>
                <UserName>admin</UserName>
            </AdminConfiguration>
        </Configuration>
    </Extension>
</Security>
```

Replace Configuration.Extensions.Authentication to:
```
<Authentication>
    <Extension Name="Forms" Type="Microsoft.Samples.ReportingServices.CustomSecurity.AuthenticationExtension,Microsoft.Samples.ReportingServices.CustomSecurity" />
</Authentication>
```

Replace Configuration.UI to:
(Set UseSSL to false if not using SSL)
(Set <IP> to your report server IP)
```
<UI>
    <CustomAuthenticationUI>
        <loginUrl>/Pages/UILogon.aspx</loginUrl>
        <UseSSL>True</UseSSL>
    </CustomAuthenticationUI>
    <ReportServerUrl>http://<IP>/ReportServer</ReportServerUrl>
</UI>
```

### &#x1F538;\ReportServer\rssrvpolicy.config
Set PermissionSetName="FullTrust" to configuration.mscorlib.security.policy.PolicyLevel.CodeGroup
```
<CodeGroup
        class="FirstMatchCodeGroup"
        version="1"
        PermissionSetName="FullTrust">
```

Append following CodeGroup to configuration.mscorlib.security.policy.PolicyLevel
```
<CodeGroup
        class="UnionCodeGroup"
        version="1"
        Name="SecurityExtensionCodeGroup"
        Description="Code group for the custom security extension"
        PermissionSetName="FullTrust">
    <IMembershipCondition
            class="UrlMembershipCondition"
            version="1"
            Url="&#x1F538;\ReportServer\bin\Microsoft.Samples.ReportingServices.CustomSecurity.dll" />
</CodeGroup>
```

### &#x1F538;\ReportManager\rsmgrpolicy.config
Set PermissionSetName="FullTrust" to configuration.mscorlib.security.policy.PolicyLevel.CodeGroup
```
<CodeGroup 
        class="FirstMatchCodeGroup"
        version="1"
        PermissionSetName="FullTrust">
```

Set PermissionSetName="FullTrust" where Description="This code group grants MyComputer code Execution permission. "
```
<CodeGroup 
        class="FirstMatchCodeGroup" 
        version="1" 
        PermissionSetName="FullTrust"
        Description="This code group grants MyComputer code Execution permission. ">
```

### &#x1F538;\ReportServer\web.config
Replace configuration.'system.web'.authentication to:
```
<authentication mode="Forms">
    <forms loginUrl="logon.aspx" name="sqlAuthCookie" timeout="60" path="/"></forms>
</authentication>
<authorization>
    <deny users="?" />
</authorization>
```

### &#x1F538;\ReportManager\Web.config
Replace configuration.'system.web'.authentication & configuration.'system.web'.identity to:
```
<authentication mode="Forms" />
<identity impersonate="false" />
```
Append following to configuration.appSettings
(If you use default instance: MSSQLSERVER, the instance name will be RS_MSSQLSERVER)
```
<add key="ReportServer" value="<Your PC Machine Name>"/>
<add key="ReportServerInstance" value="<Instance Name>"/>
```