﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.GlobalHelper;

public partial class CMSModules_Settings_FormControls_ScreenLockTimeoutValidator : FormEngineUserControl
{
    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        int input = ValidationHelper.GetInteger(txtInput.Text, 1);
        if (input <= 0 || input > SessionHelper.SessionTimeout)
        {
            return false;
        }
        return true;
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (!String.IsNullOrEmpty(txtInput.Text))
            {
                return ValidationHelper.GetInteger(txtInput.Text, 0);
            }
            return null;
        }
        set
        {
            if (ValidationHelper.IsInteger(value))
            {
                txtInput.Text = ValidationHelper.GetInteger(value, 0).ToString();
            }
            else
            {
                txtInput.Text = String.Empty;
            }
        }
    }


    /// <summary>
    /// Returns ClientID of the CMSTextBox with input...
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtInput.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            txtInput.Enabled = value;
        }
    }
}