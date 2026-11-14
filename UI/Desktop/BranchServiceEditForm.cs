using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.UI.Desktop;

public class BranchServiceEditForm : Form
{
    private readonly TextBox txtName;
    private readonly TextBox txtDescription;
    private readonly NumericUpDown numDuration;
    private readonly NumericUpDown numPrice;
    private readonly CheckBox chkActive;

    public string ServiceName => txtName.Text.Trim();
    public string Description => txtDescription.Text.Trim();
    public int Duration => (int)numDuration.Value;
    public decimal Price => numPrice.Value;
    public bool IsActive => chkActive.Checked;

    public BranchServiceEditForm(
        HealthService? service = null,
        decimal price = 0,
        bool active = true)
    {
        Text = service == null ? "Yeni Hizmet" : "Hizmeti Düzenle";

        Width = 570;
        Height = 470;

        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        MaximizeBox = false;
        MinimizeBox = false;

        AddLabel("Hizmet Adı", 35, 35);
        txtName = AddTextBox(180, 32);

        AddLabel("Açıklama", 35, 95);
        txtDescription = AddTextBox(180, 92);
        txtDescription.Multiline = true;
        txtDescription.Height = 75;

        AddLabel("Süre (dk)", 35, 190);
        numDuration = new NumericUpDown
        {
            Left = 180,
            Top = 187,
            Width = 200,
            Minimum = 5,
            Maximum = 600,
            Value = 30
        };
        Controls.Add(numDuration);

        AddLabel("Fiyat", 35, 245);
        numPrice = new NumericUpDown
        {
            Left = 180,
            Top = 242,
            Width = 200,
            Minimum = 0,
            Maximum = 1000000,
            DecimalPlaces = 2,
            Value = price
        };
        Controls.Add(numPrice);

        chkActive = new CheckBox
        {
            Text = "Hizmet aktif",
            Left = 180,
            Top = 300,
            AutoSize = true,
            Checked = active
        };
        Controls.Add(chkActive);

        var save = new Button
        {
            Text = "Kaydet",
            Left = 295,
            Top = 355,
            Width = 105,
            Height = 38,
            DialogResult = DialogResult.OK
        };

        var cancel = new Button
        {
            Text = "İptal",
            Left = 415,
            Top = 355,
            Width = 105,
            Height = 38,
            DialogResult = DialogResult.Cancel
        };

        save.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(ServiceName))
            {
                MessageBox.Show("Hizmet adı zorunludur.");
                DialogResult = DialogResult.None;
            }
        };

        Controls.Add(save);
        Controls.Add(cancel);

        AcceptButton = save;
        CancelButton = cancel;

        if (service != null)
        {
            txtName.Text = service.Name;
            txtDescription.Text = service.Description;
            numDuration.Value = service.DurationMinutes;
            numPrice.Value = price;
            chkActive.Checked = active;
        }
    }

    private void AddLabel(string text, int left, int top)
    {
        Controls.Add(new Label
        {
            Text = text,
            Left = left,
            Top = top,
            Width = 130
        });
    }

    private TextBox AddTextBox(int left, int top)
    {
        var box = new TextBox
        {
            Left = left,
            Top = top,
            Width = 340
        };

        Controls.Add(box);
        return box;
    }
}
