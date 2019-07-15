#region Copyright � Microsoft Corporation. All rights reserved.
/*============================================================================
  File:     Logon.aspx.cs
  Summary:  The code-behind for a logon page that supports Forms
            Authentication in a custom security extension    
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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web.Security;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.Samples.ReportingServices.CustomSecurity.App_LocalResources;
using System.Globalization;

namespace Microsoft.Samples.ReportingServices.CustomSecurity
{
   public class Logon : System.Web.UI.Page
   {
      protected System.Web.UI.WebControls.Label LblUser;
      protected System.Web.UI.WebControls.TextBox TxtPwd;
      protected System.Web.UI.WebControls.TextBox TxtUser;
      protected System.Web.UI.WebControls.Button BtnLogon;
      protected System.Web.UI.WebControls.Label lblMessage;
      protected System.Web.UI.WebControls.Label Label1;
      protected System.Web.UI.WebControls.Label LblPwd;

      private void Page_Load(object sender, System.EventArgs e)
      {

      }

      #region Web Form Designer generated code
      override protected void OnInit(EventArgs e)
      {
            InitializeComponent();
            base.OnInit(e);
      }
      
      private void InitializeComponent()
      {    
         this.BtnLogon.Click += new System.EventHandler(this.ServerBtnLogon_Click);
         this.Load += new System.EventHandler(this.Page_Load);

      }
      #endregion
        
       [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      private void ServerBtnLogon_Click(object sender, 
         System.EventArgs e)
      {
         bool passwordVerified = false;
         try
         {
            passwordVerified = 
               AuthenticationUtilities.VerifyPassword(TxtUser.Text,TxtPwd.Text);
            if (passwordVerified)
            {
               FormsAuthentication.RedirectFromLoginPage(
                  TxtUser.Text, false);
            }
            else
            {
               Response.Redirect("logon.aspx");
            }
         }
         catch(Exception ex)
         {
           lblMessage.Text = string.Format(CultureInfo.InvariantCulture, ex.Message);
            return;
         }
         if (passwordVerified == true )
         {
            // The user is authenticated
            // At this point, an authentication ticket is normally created
            // This can subsequently be used to generate a GenericPrincipal
            // object for .NET authorization purposes
            // For details, see "How To: Use Forms authentication with 
            // GenericPrincipal objects
           lblMessage.Text = string.Format(CultureInfo.InvariantCulture,
              Logon_aspx.LoginSuccess);
         }
         else
         {
           lblMessage.Text = string.Format(CultureInfo.InvariantCulture,
             Logon_aspx.InvalidUsernamePassword);
         }
      }
   }
}
