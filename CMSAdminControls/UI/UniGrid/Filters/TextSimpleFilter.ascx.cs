using System;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Controls;
using CMS.GlobalHelper;

public partial class CMSAdminControls_UI_UniGrid_Filters_TextSimpleFilter : CMSAbstractBaseFilterControl
{
    #region "Private variables"

    private bool mIncludeNULLCondition = true;
    private string mColumn = null;
    private string[] mColumns = null;
    private int mSize = 1000;
    private string[] mOperators = new string[] { "LIKE", "NOT LIKE", "=", "<>" };

    #endregion


    #region "Public properties"

    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GetCondition();
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    /// <summary>
    /// Name of the column.
    /// </summary>
    public string Column
    {
        get
        {
            return mColumn;
        }
        set
        {
            mColumn = value;
        }
    }


    /// <summary>
    /// Use this property to search within multiple columns.
    /// </summary>
    public string[] Columns
    {
        get
        {
            return mColumns;
        }
        set
        {
            mColumns = value;
        }
    }


    /// <summary>
    /// Maximum length of the entered text.
    /// </summary>
    public int Size
    {
        get
        {
            return mSize;
        }
        set
        {
            mSize = value;
        }
    }


    /// <summary>
    /// Determines whether condition for NULL values is added when 'NOT LIKE'
    /// or '<>' operand is selected.
    /// </summary>
    public bool IncludeNULLCondition
    {
        get
        {
            return mIncludeNULLCondition;
        }
        set
        {
            mIncludeNULLCondition = value;
        }
    }


    /// <summary>
    /// Gets/sets current text in filter.
    /// </summary>
    public string FilterText
    {
        get
        {
            return txtText.Text;
        }
        set
        {
            txtText.Text = value;
        }
    }


    /// <summary>
    /// Gets current operator.
    /// </summary>
    public string FilterOperator
    {
        get
        {
            // Check that selected operator was not modified
            if (mOperators.Contains(drpCondition.SelectedValue))
            {
                return drpCondition.SelectedValue;
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (value != null && mOperators.Contains(value))
            {
                drpCondition.SelectedValue = value;
            }
        }
    }


    /// <summary>
    /// Determines whether the filter is set.
    /// </summary>
    public bool FilterIsSet
    {
        get
        {
            return !string.IsNullOrEmpty(txtText.Text);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        InitFilterDropDown(drpCondition);
        txtText.MaxLength = Size;
    }

    #endregion


    #region "Public methods"

    public string GetCondition()
    {
        // Avoid SQL injection
        string tempVal = txtText.Text.Trim().Replace("'", "''");

        string op = FilterOperator;

        // No condition
        if (String.IsNullOrEmpty(tempVal) || (String.IsNullOrEmpty(Column) && (Columns == null)) || String.IsNullOrEmpty(op))
        {
            return null;
        }

        // Support for exact phrase search
        if (op.Contains("LIKE"))
        {
            tempVal = "%" + tempVal + "%";
        }

        StringBuilder where = new StringBuilder();
        string additionalForNull = string.Empty;

        // Get where condition for multiple columns
        if (Columns != null)
        {
            foreach (string column in Columns)
            {
                // Handling "NOT LIKE" and "<>" operators
                if (IncludeNULLCondition)
                {
                    if (op.Contains("<") || op.Contains("NOT"))
                    {
                        additionalForNull = " OR " + column + " IS NULL";
                    }
                }

                if (!String.IsNullOrEmpty(where.ToString()))
                {
                    if (op.Contains("<") || op.Contains("NOT"))
                    {
                        where.Append(" AND ");
                    }
                    else
                    {
                        where.Append(" OR ");
                    }
                }
                where.Append("(" + column + " " + op + " N'" + tempVal + "' " + additionalForNull + ")");
            }
            return "(" + where.ToString() + ")";
        }
        // Get where condition for single column
        else
        {
            // Handling "NOT LIKE" and "<>" operators
            if (IncludeNULLCondition)
            {
                if (op.Contains("<") || op.Contains("NOT"))
                {
                    additionalForNull = " OR " + Column + " IS NULL";
                }
            }
            return "(" + Column + " " + op + " N'" + tempVal + "' " + additionalForNull + ")";
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes standard filter dropdown.
    /// </summary>
    /// <param name="drp">Dropdown to init</param>
    private void InitFilterDropDown(DropDownList drp)
    {
        if ((drp != null) && (drp.Items.Count <= 0))
        {
            // Add 'LIKE'
            drp.Items.Add(new ListItem(mOperators[0], mOperators[0]));
            // Add 'NOT LIKE'
            drp.Items.Add(new ListItem(mOperators[1], mOperators[1]));
            // Add '='
            drp.Items.Add(new ListItem(mOperators[2], mOperators[2]));
            // Add '<>'
            drp.Items.Add(new ListItem(mOperators[3], mOperators[3]));
        }
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        state.AddValue("Text", FilterText);
        state.AddValue("Operator", FilterOperator);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        FilterText = state.GetString("Text");
        FilterOperator = state.GetString("Operator");
    }


    /// <summary>
    /// Resets the filter settings.
    /// </summary>
    public override void ResetFilter()
    {
        FilterText = String.Empty;
        FilterOperator = mOperators[0];
    }

    #endregion
}