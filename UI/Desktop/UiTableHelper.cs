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

            SelectionMode =
                DataGridViewSelectionMode.FullRowSelect,

            AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill,

            AutoGenerateColumns = true,

            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,

            ColumnHeadersHeight = 40,

            RowTemplate =
            {
                Height = 34
            }
        };
    }


    public static void Bind<T>(
        DataGridView grid,
        IEnumerable<T> source)
    {
        var items =
            source.ToList();


        var table =
            new DataTable();


        PropertyInfo[] properties =
            typeof(T).GetProperties();


        foreach (PropertyInfo property
                 in properties)
        {
            Type type =
                Nullable.GetUnderlyingType(
                    property.PropertyType)
                ?? property.PropertyType;


            if (type.IsEnum)
                type = typeof(string);


            table.Columns.Add(
                property.Name,
                type);
        }


        foreach (T item
                 in items)
        {
            DataRow row =
                table.NewRow();


            foreach (PropertyInfo property
                     in properties)
            {
                object? value =
                    property.GetValue(item);


                if (value == null)
                {
                    row[property.Name] =
                        DBNull.Value;
                }
                else if (value.GetType().IsEnum)
                {
                    row[property.Name] =
                        value.ToString()!;
                }
                else
                {
                    row[property.Name] =
                        value;
                }
            }


            table.Rows.Add(row);
        }


        DataView view =
            table.DefaultView;


        if (table.Columns.Contains("ID"))
        {
            view.Sort =
                "ID ASC";
        }


        grid.DataSource =
            view;


        foreach (DataGridViewColumn column
                 in grid.Columns)
        {
            column.SortMode =
                DataGridViewColumnSortMode.Automatic;
        }
    }


    public static void AttachSearch(
        TextBox searchBox,
        DataGridView grid)
    {
        searchBox.TextChanged +=
            (_, _) =>
            {
                if (grid.DataSource
                    is not DataView view)
                {
                    return;
                }


                string text =
                    searchBox.Text.Trim();


                if (string.IsNullOrWhiteSpace(
                        text))
                {
                    view.RowFilter =
                        "";

                    return;
                }


                string escaped =
                    text
                        .Replace(
                            "'",
                            "''")
                        .Replace(
                            "[",
                            "[[]")
                        .Replace(
                            "%",
                            "[%]")
                        .Replace(
                            "*",
                            "[*]");


                var filters =
                    view.Table.Columns
                        .Cast<DataColumn>()
                        .Where(c =>
                            c.DataType ==
                                typeof(string))
                        .Select(c =>
                            $"CONVERT([{c.ColumnName}], 'System.String') LIKE '%{escaped}%'");


                string filter =
                    string.Join(
                        " OR ",
                        filters);


                view.RowFilter =
                    filter;
            };
    }


    public static void HideColumns(
        DataGridView grid,
        params string[] columns)
    {
        foreach (string columnName
                 in columns)
        {
            if (grid.Columns.Contains(
                    columnName))
            {
                grid.Columns[
                    columnName]!.Visible =
                        false;
            }
        }
    }


    public static int SelectedInt(
        DataGridView grid,
        string columnName)
    {
        if (grid.CurrentRow ==
            null)
        {
            throw new Exception(
                "Önce bir satır seçiniz.");
        }


        object? value =
            grid.CurrentRow
                .Cells[columnName]
                .Value;


        if (value == null ||
            value == DBNull.Value)
        {
            throw new Exception(
                "Seçili kayıt okunamadı.");
        }


        return Convert.ToInt32(
            value);
    }
}