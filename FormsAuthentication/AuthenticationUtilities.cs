#region Copyright © Microsoft Corporation. All rights reserved.
/*============================================================================
   File:      AuthenticationStore.cs

  Summary:  Demonstrates how to create and maintain a user store for
            a security extension. 
--------------------------------------------------------------------
  This file is part of Microsoft SQL Server Code Samples.
    
 This source code is intended only as a supplement to Microsoft
 Development Tools and/or on-line documentation. See these other
 materials for detailed information regarding Microsoft code 
 samples.

 THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF 
 ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
 THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 PARTICULAR PURPOSE.
===========================================================================*/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Management;
using System.Xml;
using System.Text;
using System.Globalization;

namespace Microsoft.Samples.ReportingServices.CustomSecurity
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal sealed class AuthenticationUtilities
    {
        private const string wmiNamespace = @"\root\Microsoft\SqlServer\ReportServer\{0}\v12";
        private const string rsAsmx = @"/ReportService2010.asmx";

        // Method that indicates whether 
        // the supplied username and password are valid
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        internal static bool VerifyPassword(string suppliedUserName,
           string suppliedPassword)
        {
            bool passwordMatch = true;
            //todoooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
            return passwordMatch;
        }

        //Method to get the report server url using WMI
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        internal static string GetReportServerUrl(string machineName, string instanceName)
        {
            string reportServerVirtualDirectory = String.Empty;
            string fullWmiNamespace = @"\\" + machineName + string.Format(wmiNamespace, instanceName);

            ManagementScope scope = null;

            ConnectionOptions connOptions = new ConnectionOptions();
            connOptions.Authentication = AuthenticationLevel.PacketPrivacy;

            //Get management scope
            try
            {
                scope = new ManagementScope(fullWmiNamespace, connOptions);
                scope.Connect();

                //Get management class
                ManagementPath path = new ManagementPath("MSReportServer_Instance");
                ObjectGetOptions options = new ObjectGetOptions();
                ManagementClass serverClass = new ManagementClass(scope, path, options);

                serverClass.Get();

                if (serverClass == null)
                    throw new Exception(string.Format(CultureInfo.InvariantCulture,
                      CustomSecurity.WMIClassError));

                //Get instances
                ManagementObjectCollection instances = serverClass.GetInstances();

                foreach (ManagementObject instance in instances)
                {
                    instance.Get();

                    ManagementBaseObject outParams = (ManagementBaseObject)instance.InvokeMethod("GetReportServerUrls",
                       null, null);

                    string[] appNames = (string[])outParams["ApplicationName"];
                    string[] urls = (string[])outParams["URLs"];

                    for (int i = 0; i < appNames.Length; i++)
                    {
                        if (appNames[i] == "ReportServerWebService")
                            reportServerVirtualDirectory = urls[i];
                    }

                    if (reportServerVirtualDirectory == string.Empty)
                        throw new Exception(string.Format(CultureInfo.InvariantCulture,
                           CustomSecurity.MissingUrlReservation));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture,
                    CustomSecurity.RSUrlError + ex.Message), ex);
            }

            return reportServerVirtualDirectory + rsAsmx;
        }
    }
}
