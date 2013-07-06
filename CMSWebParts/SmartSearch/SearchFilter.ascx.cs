using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.Controls;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SiteProvider;
using CMS.SettingsProvider;

public partial class CMSWebParts_SmartSearch_SearchFilter : CMSAbstractWebPart
{
    #region "Variables"

    private string mSearchWebpartId = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the repeat layout
    /// </summary>
    public string RepeatLayout
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RepeatLayout"), "table");
        }
        set
        {
            chklstFilter.RepeatLayout = GetLayoutEnumFromString(value);
            radlstFilter.RepeatLayout = chklstFilter.RepeatLayout;
            SetValue("RepeatLayout", value);
        }
    }


    /// <summary>
    /// Gets or sets the number of repeat columns
    /// </summary>
    public int RepeatColumns
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RepeatColumns"), 0);
        }
        set
        {
            chklstFilter.RepeatColumns = value;
            radlstFilter.RepeatColumns = chklstFilter.RepeatColumns;
            SetValue("RepeatColumns", value);
        }
    }


    /// <summary>
    /// Gets or sets search webpart ID.
    /// </summary>
    public string SearchWebpartID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SearchWebpartID"), mSearchWebpartId);
        }
        set
        {
            mSearchWebpartId = value;
            SetValue("SearchWebpartID", value);
        }
    }


    /// <summary>
    /// Gets or sets filter type.
    /// </summary>
    public SearchFilterModeEnum FilterMode
    {
        get
        {
            return SearchHelper.GetSearchFilterModeEnum(ValidationHelper.GetString(GetValue("FilterMode"), ""));
        }
        set
        {
            SetValue("FilterMode", SearchHelper.GetSearchFilterModeString(value));
        }
    }


    /// <summary>
    /// Gets or sets filter layout.
    /// </summary>
    public ControlLayoutEnum FilterLayout
    {
        get
        {
            return CMSControlsHelper.GetControlLayoutEnum(ValidationHelper.GetString(GetValue("FilterLayout"), "horizontal"));
        }
        set
        {
            SetValue("FilterLayout", CMSControlsHelper.GetControlLayoutString(value));
        }
    }


    /// <summary>
    /// Gets or sets filter auto post back.
    /// </summary>
    public bool FilterAutoPostback
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FilterAutoPostback"), false);
        }
        set
        {
            SetValue("FilterAutoPostback", value);
        }
    }


    /// <summary>
    /// Gets or sets filter values.
    /// </summary>
    public string FilterValues
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterValues"), "");
        }
        set
        {
            SetValue("FilterValues", value);
        }
    }


    /// <summary>
    /// Gets or sets filter clause.
    /// </summary>
    public string FilterClause
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterClause"), "");
        }
        set
        {
            SetValue("FilterClause", value);
        }
    }


    /// <summary>
    /// Gets or sets filter query name.
    /// </summary>
    public string FilterQueryName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterQueryName"), "");
        }
        set
        {
            SetValue("FilterQueryName", value);
        }
    }


    /// <summary>
    /// Gets or sets filter is conditional.
    /// </summary>
    public bool FilterIsConditional
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FilterIsConditional"), true);
        }
        set
        {
            SetValue("FilterIsConditional", value);
        }
    }


    /// <summary>
    /// Gets or sets filter WHERE condition.
    /// </summary>
    public string FilterWhere
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterWhere"), "");
        }
        set
        {
            SetValue("FilterWhere", value);
        }
    }


    /// <summary>
    /// Gets or sets filter ORDER BY expression.
    /// </summary>
    public string FilterOrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterOrderBy"), "");
        }
        set
        {
            SetValue("FilterOrderBy", value);
        }
    }


    /// <summary>
    /// Gets or sets default selected index.
    /// </summary>
    public string DefaultSelectedIndex
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DefaultSelectedIndex"), "");
        }
        set
        {
            SetValue("DefaultSelectedIndex", value);
        }
    }


    /// <summary>
    /// The text to show when the control has no value.
    /// </summary>
    public string WatermarkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WatermarkText"), txtFilter.WatermarkText);
        }
        set
        {
            SetValue("WatermarkText", value);
            txtFilter.WatermarkText = value;
        }
    }


    /// <summary>
    /// The CSS class to apply to the TextBox when it has no value (e.g. the watermark text is shown).
    /// </summary>
    public string WatermarkCssClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WatermarkCssClass"), txtFilter.WatermarkCssClass);
        }
        set
        {
            SetValue("WatermarkCssClass", value);
            txtFilter.WatermarkCssClass = value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {

            if (!RequestHelper.IsPostBack() && (FilterMode != SearchFilterModeEnum.TextBox))
            {
                // If query name filled - execute it
                if (!string.IsNullOrEmpty(FilterQueryName))
                {
                    // Execute query
                    DataSet ds = ConnectionHelper.ExecuteQuery(FilterQueryName, null, FilterWhere, FilterOrderBy);
                    if (!DataHelper.DataSourceIsEmpty(ds))
                    {
                        // Check that dataset has at least 3 columns
                        if (ds.Tables[0].Columns.Count < 3)
                        {
                            lblError.ResourceString = "srch.filter.fewcolumns";
                            lblError.Visible = true;
                            return;
                        }

                        // Loop thru all rows
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            AddItem(dr[0].ToString(), dr[1].ToString(), ResHelper.LocalizeString(dr[2].ToString()));
                        }
                    }
                }
                // Else if values are filled - parse them
                else if (!string.IsNullOrEmpty(FilterValues))
                {
                    // Split values into rows
                    string[] rows = FilterValues.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    // Loop thru each row
                    foreach (string row in rows)
                    {
                        string trimmedRow = row.Trim().TrimEnd('\r');
                        if (!string.IsNullOrEmpty(trimmedRow))
                        {
                            string[] values = trimmedRow.Split(';');
                            if (values.Length == 3)
                            {
                                AddItem(values[0], values[1], values[2]);
                            }
                            else
                            {
                                lblError.ResourceString = "srch.filter.badformat";
                                lblError.Visible = true;
                                return;
                            }
                        }
                    }
                }
            }

            // Get webpart ID
            string webpartID = ValidationHelper.GetString(GetValue("WebpartControlID"), ClientID);

            // Try to get selected values from querystring - but only if is not postback
            if (!RequestHelper.IsPostBack())
            {
                string selectedItems = QueryHelper.GetString(webpartID, "");

                // If none of items are selected - try to get default values
                if (string.IsNullOrEmpty(selectedItems))
                {
                    selectedItems = DefaultSelectedIndex;
                }

                if (!string.IsNullOrEmpty(selectedItems))
                {
                    string[] splittedItems = selectedItems.Split(';');
                    foreach (string item in splittedItems)
                    {
                        switch (FilterMode)
                        {
                            case SearchFilterModeEnum.Checkbox:
                                SelectItem(item, chklstFilter);
                                break;
                            case SearchFilterModeEnum.RadioButton:
                                SelectItem(item, radlstFilter);
                                break;
                            case SearchFilterModeEnum.DropdownList:
                                SelectItem(item, drpFilter);
                                break;
                            default:
                                txtFilter.Text = item;
                                break;
                        }
                    }
                }
            }

            string applyFilter = "";
            string ids = "";

            // Set up controls
            switch (FilterMode)
            {
                // Set text box
                case SearchFilterModeEnum.TextBox:
                    txtFilter.Visible = true;
                    txtFilter.WatermarkCssClass = WatermarkCssClass;
                    txtFilter.WatermarkText = WatermarkText;
                    txtFilter.AutoPostBack = FilterAutoPostback;
                    if (!String.IsNullOrEmpty(DefaultSelectedIndex) && String.IsNullOrEmpty(txtFilter.Text) && !URLHelper.IsPostback())
                    {
                        txtFilter.Text = DefaultSelectedIndex;
                    }

                    // Apply filter only of textbox contains something
                    if (!String.IsNullOrEmpty(txtFilter.Text))
                    {
                        // Prepare right condition when filter values aren't empty
                        if (!String.IsNullOrEmpty(FilterValues))
                        {
                            string[] rows = FilterValues.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                            // Loop thru each row
                            foreach (string row in rows)
                            {
                                string trimmedRow = row.Trim();
                                if (!string.IsNullOrEmpty(trimmedRow))
                                {
                                    applyFilter += trimmedRow + ":(" + SearchHelper.ProcessSearchKeywords(txtFilter.Text, SearchOptionsEnum.NoneSearch) + ") ";
                                }
                            }
                        }
                        else
                        {
                            applyFilter = txtFilter.Text;
                        }
                    }

                    ids = HttpUtility.UrlEncode(txtFilter.Text);
                    break;

                // Set checkbox list
                case SearchFilterModeEnum.Checkbox:

                    // Set visibility and layout
                    chklstFilter.Visible = true;
                    chklstFilter.RepeatDirection = RepeatDirection.Vertical;
                    if (FilterLayout == ControlLayoutEnum.Horizontal)
                    {
                        chklstFilter.RepeatDirection = RepeatDirection.Horizontal;
                    }

                    chklstFilter.RepeatLayout = GetLayoutEnumFromString(RepeatLayout);
                    chklstFilter.RepeatColumns = this.RepeatColumns;

                    // Get selected items
                    applyFilter = GetSelectedItems(chklstFilter, out ids);

                    // Set autopostback
                    if (FilterAutoPostback)
                    {
                        chklstFilter.AutoPostBack = true;
                    }
                    break;

                // Set radio list
                case SearchFilterModeEnum.RadioButton:

                    // Set visibility and layout
                    radlstFilter.Visible = true;
                    radlstFilter.RepeatDirection = RepeatDirection.Vertical;
                    if (FilterLayout == ControlLayoutEnum.Horizontal)
                    {
                        radlstFilter.RepeatDirection = RepeatDirection.Horizontal;
                    }

                    radlstFilter.RepeatLayout = GetLayoutEnumFromString(RepeatLayout);
                    radlstFilter.RepeatColumns = this.RepeatColumns;


                    // Get selected items
                    applyFilter = GetSelectedItems(radlstFilter, out ids);

                    // Set autopostback
                    if (FilterAutoPostback)
                    {
                        radlstFilter.AutoPostBack = true;
                    }
                    break;

                // Set dropdown list
                default:

                    // Set visibility
                    drpFilter.Visible = true;

                    // Get selected items
                    applyFilter = GetSelectedItems(drpFilter, out ids);

                    // Set autopostback
                    if (FilterAutoPostback)
                    {
                        drpFilter.AutoPostBack = true;
                    }
                    lblFilter.AssociatedControlID = drpFilter.ID;
                    break;
            }

            // Apply filter and add selected values to querystring
            ISearchFilterable searchWebpart = (ISearchFilterable)CMSControlsHelper.GetFilter(SearchWebpartID);
            if (searchWebpart != null)
            {
                if (FilterIsConditional)
                {
                    // If filter fieldname or value is filled
                    if (!string.IsNullOrEmpty(applyFilter) && (applyFilter.Trim() != ":"))
                    {
                        // Handle filter clause
                        if (!string.IsNullOrEmpty(FilterClause))
                        {
                            applyFilter = FilterClause + "( " + applyFilter + " )";
                        }
                        searchWebpart.ApplyFilter(applyFilter, null);
                    }
                    searchWebpart.AddFilterOptionsToUrl(webpartID, ids);
                }
                else
                {
                    searchWebpart.ApplyFilter(null, applyFilter);
                    searchWebpart.AddFilterOptionsToUrl(webpartID, ids);
                }

            }
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // Check if filter should be displayed
            if (string.IsNullOrEmpty(FilterQueryName) && string.IsNullOrEmpty(FilterValues) && (FilterMode != SearchFilterModeEnum.TextBox))
            {
                Visible = false;
                return;
            }
        }
    }


    /// <summary>
    /// Realoads data.
    /// </summary>
    public override void ReloadData()
    {
        SetupControl();
        base.ReloadData();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns layout enum from string
    /// </summary>
    /// <param name="value">String value</param>
    private RepeatLayout GetLayoutEnumFromString(string value)
    {
        if (value.EqualsCSafe("flow"))
        {
            return System.Web.UI.WebControls.RepeatLayout.Flow;
        }

        return System.Web.UI.WebControls.RepeatLayout.Table;
    }


    /// <summary>
    /// Returns direction enum from string
    /// </summary>
    /// <param name="value">String value</param>
    private RepeatDirection GetDirectionEnumFromString(string value)
    {
        if (value.EqualsCSafe("vertical"))
        {
            return System.Web.UI.WebControls.RepeatDirection.Vertical;
        }

        return System.Web.UI.WebControls.RepeatDirection.Horizontal;
    }


    /// <summary>
    /// Adds item to list control.
    /// </summary>
    /// <param name="fieldName">Field name</param>
    /// <param name="value">Value</param>
    /// <param name="displayName">Display name</param>
    private void AddItem(string fieldName, string value, string displayName)
    {
        // Create new item
        ListItem item;

        if (FilterMode != SearchFilterModeEnum.DropdownList)
        {
            displayName = HTMLHelper.HTMLEncode(displayName);
        }

        if (FilterIsConditional)
        {
            item = new ListItem(displayName, fieldName + ":" + value);
        }
        else
        {
            item = new ListItem(displayName, fieldName);
        }

        switch (FilterMode)
        {
            case SearchFilterModeEnum.Checkbox:
                // Add item to checkbox list
                chklstFilter.Items.Add(item);
                break;

            case SearchFilterModeEnum.RadioButton:
                // Add item to radio button list
                radlstFilter.Items.Add(item);
                break;

            default:
                // Add item to dropdown list
                drpFilter.Items.Add(item);
                break;
        }
    }


    /// <summary>
    /// Selects item in list control.
    /// </summary>
    /// <param name="itemString">Item</param>
    /// <param name="control">Control</param>
    private void SelectItem(string itemString, ListControl control)
    {
        int item = ValidationHelper.GetInteger(itemString, -1);
        if ((item != -1) && item < control.Items.Count)
        {
            control.Items[item].Selected = true;
        }
    }


    /// <summary>
    /// Gets selected items.
    /// </summary>
    /// <param name="control">Control</param>  
    /// <param name="ids">Id's of selected values separed by semicolon</param>
    private string GetSelectedItems(ListControl control, out string ids)
    {
        ids = "";
        string selected = "";

        // loop thru all items
        for (int i = 0; i != control.Items.Count; i++)
        {
            if (control.Items[i].Selected)
            {
                selected += " " + control.Items[i].Value;
                ids += ValidationHelper.GetString(i, "") + ";";
            }
        }

        if (String.IsNullOrEmpty(selected) && (control.SelectedItem != null))
        {
            selected = control.SelectedItem.Value;
            ids = control.SelectedIndex.ToString();
        }

        return selected;
    }

    #endregion
}