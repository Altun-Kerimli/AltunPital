using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.UI.Desktop;

public class BranchEquipmentEditForm : Form
{
    private readonly TextBox txtName;
    private readonly TextBox txtDescription;
    private readonly NumericUpDown numQuantity;
    private readonly CheckBox chkOperational;

    public string EquipmentName => txtName.Text.Trim();
    public string Description => txtDescription.Text.Trim();
    public int Quantity => (int)numQuantity.Value;
    public bool IsOperational => chkOperational.Checked;

    public BranchEquipmentEditForm(
        Equipment? equipment = null,
        int quantity = 1,
        bool operational = true)
    {
        Text = equipment == null ? "Yeni Ekipman" : "Ekipmanı Düzenle";

        Width = 570;
        Height = 430;

        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        MaximizeBox = false;
        MinimizeBox = false;

        AddLabel("Ekipman Adı", 35, 35);
        txtName = AddTextBox(180, 32);

        AddLabel("Açıklama", 35, 95);
        txtDescription = AddTextBox(180, 92);
        txtDescription.Multiline = true;
        txtDescription.Height = 75;

        AddLabel("Adet", 35, 195);
        numQuantity = new NumericUpDown
        {
            Left = 180,
            Top = 192,
            Width = 200,
            Minimum = 0,
            Maximum = 10000,
            Value = quantity
        };
        Controls.Add(numQuantity);

        chkOperational = new CheckBox
        {
            Text = "Çalışır durumda",
            Left = 180,
            Top = 250,
            AutoSize = true,
            Checked = operational
        };
        Controls.Add(chkOperational);

        var save = new Button
        {
            Text = "Kaydet",
            Left = 295,
            Top = 310,
            Width = 105,
            Height = 38,
            DialogResult = DialogResult.OK
        };

        var cancel = new Button
        {
            Text = "İptal",
            Left = 415,
            Top = 310,
            Width = 105,
            Height = 38,
            DialogResult = DialogResult.Cancel
        };

        save.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(EquipmentName))
            {
                MessageBox.Show("Ekipman adı zorunludur.");
                DialogResult = DialogResult.None;
            }
        };

        Controls.Add(save);
        Controls.Add(cancel);

        AcceptButton = save;
        CancelButton = cancel;

        if (equipment != null)
        {
            txtName.Text = equipment.Name;
            txtDescription.Text = equipment.Description;
            numQuantity.Value = quantity;
            chkOperational.Checked = operational;
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
