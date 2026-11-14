using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Font = System.Drawing.Font;
using Label = System.Windows.Forms.Label;

namespace AltunPital.UI.Desktop;

public static class PromptDialog
{
    public static string? Ask(string title, string label, string defaultValue = "", bool password = false)
    {
        using var form = new Form
        {
            Text = title,
            Width = 430,
            Height = 200,
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            Font = new Font("Segoe UI", 10)
        };

        var lbl = new Label
        {
            Text = label,
            Left = 25,
            Top = 25,
            AutoSize = true
        };

        var input = new TextBox
        {
            Left = 25,
            Top = 55,
            Width = 360,
            Text = defaultValue,
            UseSystemPasswordChar = password
        };

        var ok = new Button
        {
            Text = "Tamam",
            Left = 205,
            Top = 100,
            Width = 85,
            DialogResult = DialogResult.OK
        };

        var cancel = new Button
        {
            Text = "İptal",
            Left = 300,
            Top = 100,
            Width = 85,
            DialogResult = DialogResult.Cancel
        };

        form.Controls.Add(lbl);
        form.Controls.Add(input);
        form.Controls.Add(ok);
        form.Controls.Add(cancel);

        form.AcceptButton = ok;
        form.CancelButton = cancel;

        return form.ShowDialog() == DialogResult.OK
            ? input.Text
            : null;
    }

    public static int? AskInt(string title, string label, int? defaultValue = null)
    {
        string? value = Ask(title, label, defaultValue?.ToString() ?? "");

        if (value == null)
            return null;

        if (!int.TryParse(value, out int result))
            throw new Exception("Geçerli bir sayı giriniz.");

        return result;
    }

    public static decimal? AskDecimal(string title, string label)
    {
        string? value = Ask(title, label);

        if (value == null)
            return null;

        if (!decimal.TryParse(value, out decimal result))
            throw new Exception("Geçerli bir sayı giriniz.");

        return result;
    }

    public static TimeOnly? AskTime(string title, string label)
    {
        string? value = Ask(title, label);

        if (value == null)
            return null;

        if (!TimeOnly.TryParse(value, out TimeOnly result))
            throw new Exception("Geçerli bir saat giriniz.");

        return result;
    }

    public static T? Choose<T>(string title, string label, IList<T> items, Func<T, string> display)
        where T : class
    {
        using var form = new Form
        {
            Text = title,
            Width = 470,
            Height = 220,
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var lbl = new Label
        {
            Text = label,
            Left = 25,
            Top = 25,
            AutoSize = true
        };

        var combo = new ComboBox
        {
            Left = 25,
            Top = 55,
            Width = 400,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        foreach (var item in items)
            combo.Items.Add(new ChoiceItem<T>(item, display(item)));

        if (combo.Items.Count > 0)
            combo.SelectedIndex = 0;

        var ok = new Button
        {
            Text = "Tamam",
            Left = 245,
            Top = 110,
            Width = 85,
            DialogResult = DialogResult.OK
        };

        var cancel = new Button
        {
            Text = "İptal",
            Left = 340,
            Top = 110,
            Width = 85,
            DialogResult = DialogResult.Cancel
        };

        form.Controls.Add(lbl);
        form.Controls.Add(combo);
        form.Controls.Add(ok);
        form.Controls.Add(cancel);

        if (form.ShowDialog() != DialogResult.OK)
            return null;

        return (combo.SelectedItem as ChoiceItem<T>)?.Value;
    }

    private class ChoiceItem<T>
    {
        public T Value { get; }

        private readonly string _text;

        public ChoiceItem(T value, string text)
        {
            Value = value;
            _text = text;
        }

        public override string ToString()
        {
            return _text;
        }
    }
}
