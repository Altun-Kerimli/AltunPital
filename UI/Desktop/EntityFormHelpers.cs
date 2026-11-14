using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

namespace AltunPital.UI.Desktop;

public static class EntityFormHelpers
{
    public static Label Label(string text, int left, int top)
    {
        return new Label
        {
            Text = text,
            Left = left,
            Top = top,
            AutoSize = true
        };
    }

    public static TextBox TextBox(int left, int top, int width = 320)
    {
        return new TextBox
        {
            Left = left,
            Top = top,
            Width = width
        };
    }

    public static ComboBox ComboBox(int left, int top, int width = 320)
    {
        return new ComboBox
        {
            Left = left,
            Top = top,
            Width = width,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
    }

    public static Button SaveButton(int left, int top)
    {
        return new Button
        {
            Text = "Kaydet",
            Left = left,
            Top = top,
            Width = 110,
            Height = 38,
            DialogResult = DialogResult.OK
        };
    }

    public static Button CancelButton(int left, int top)
    {
        return new Button
        {
            Text = "İptal",
            Left = left,
            Top = top,
            Width = 110,
            Height = 38,
            DialogResult = DialogResult.Cancel
        };
    }
}
