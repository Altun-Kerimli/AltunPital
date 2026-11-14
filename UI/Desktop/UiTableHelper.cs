using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Reflection;

namespace AltunPital.UI.Desktop;

public static class UiTableHelper
{
    public static DataGridView CreateGrid()
    {
        return new DataGridView
        {
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            MultiSelect = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AutoGenerateColumns = true,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            ColumnHeadersHeight = 40,
            RowTemplate = { Height = 34 }
        };
    }

    public static void Bind<T>(DataGridView grid, IEnumerable<T> source)
    {
        List<T> items = source.ToList();
        DataTable table = new();
        PropertyInfo[] properties = typeof(T).GetProperties();

        foreach (PropertyInfo property in properties)
        {
            Type type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (type.IsEnum)
                type = typeof(string);

            table.Columns.Add(property.Name, type);
        }

        foreach (T item in items)
        {
            DataRow row = table.NewRow();

            foreach (PropertyInfo property in properties)
            {
                object? value = property.GetValue(item);

                if (value == null)
                    row[property.Name] = DBNull.Value;
                else if (value.GetType().IsEnum)
                    row[property.Name] = value.ToString()!;
                else
                    row[property.Name] = value;
            }

            table.Rows.Add(row);
        }

        DataView view = table.DefaultView;

        if (table.Columns.Contains("ID"))
            view.Sort = "ID ASC";

        grid.DataSource = view;

        foreach (DataGridViewColumn column in grid.Columns)
            column.SortMode = DataGridViewColumnSortMode.Automatic;
    }

    public static void AttachSearch(TextBox search, DataGridView grid)
    {
        search.TextChanged += (_, _) => ApplySearch(search.Text, grid);
    }

    public static void AttachSearchAndFilter(
        TextBox search,
        ComboBox column,
        TextBox filter,
        DataGridView grid)
    {
        void Apply()
        {
            if (grid.DataSource is not DataView view)
                return;

            List<string> expressions = new();
            string searchText = Escape(search.Text.Trim());

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                List<string> searchParts = new();

                foreach (DataColumn dataColumn in view.Table.Columns)
                {
                    searchParts.Add(
                        $"CONVERT([{dataColumn.ColumnName}], 'System.String') LIKE '%{searchText}%'");
                }

                if (searchParts.Count > 0)
                    expressions.Add("(" + string.Join(" OR ", searchParts) + ")");
            }

            string filterText = Escape(filter.Text.Trim());

            if (!string.IsNullOrWhiteSpace(filterText) &&
                column.SelectedItem is string columnName &&
                view.Table.Columns.Contains(columnName))
            {
                expressions.Add(
                    $"CONVERT([{columnName}], 'System.String') LIKE '%{filterText}%'");
            }

            view.RowFilter = string.Join(" AND ", expressions);
        }

        search.TextChanged += (_, _) => Apply();
        filter.TextChanged += (_, _) => Apply();
        column.SelectedIndexChanged += (_, _) => Apply();
    }

    public static void RefreshFilterColumns(ComboBox combo, DataGridView grid)
    {
        combo.DataSource = null;

        if (grid.DataSource is not DataView view)
            return;

        combo.DataSource = view.Table.Columns
            .Cast<DataColumn>()
            .Select(c => c.ColumnName)
            .ToList();

        if (combo.Items.Count > 0)
            combo.SelectedIndex = 0;
    }

    private static void ApplySearch(string text, DataGridView grid)
    {
        if (grid.DataSource is not DataView view)
            return;

        string escaped = Escape(text.Trim());

        if (string.IsNullOrWhiteSpace(escaped))
        {
            view.RowFilter = "";
            return;
        }

        List<string> filters = new();

        foreach (DataColumn column in view.Table.Columns)
        {
            filters.Add(
                $"CONVERT([{column.ColumnName}], 'System.String') LIKE '%{escaped}%'");
        }

        view.RowFilter = string.Join(" OR ", filters);
    }

    private static string Escape(string value)
    {
        return value
            .Replace("'", "''")
            .Replace("[", "[[]")
            .Replace("%", "[%]")
            .Replace("*", "[*]");
    }

    public static void HideColumns(DataGridView grid, params string[] columns)
    {
        foreach (string columnName in columns)
        {
            if (grid.Columns.Contains(columnName))
                grid.Columns[columnName]!.Visible = false;
        }
    }

    public static int SelectedInt(DataGridView grid, string columnName)
    {
        if (grid.CurrentRow == null)
            throw new Exception("Önce bir satır seçiniz.");

        object? value = grid.CurrentRow.Cells[columnName].Value;

        if (value == null || value == DBNull.Value)
            throw new Exception("Seçili kayıt okunamadı.");

        return Convert.ToInt32(value);
    }
}
