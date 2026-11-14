using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.UI.Desktop;

public class EquipmentEditForm : Form
{
    private readonly TextBox txtName;
    private readonly TextBox txtDescription;

    public string EquipmentName => txtName.Text.Trim();
    public string Description => txtDescription.Text.Trim();

    public EquipmentEditForm(Equipment? equipment = null)
    {
        Text = equipment == null ? "Yeni Ekipman" : "Ekipman Düzenle";

        Width = 500;
        Height = 290;

        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        txtName = EntityFormHelpers.TextBox(140, 45);
        txtDescription = EntityFormHelpers.TextBox(140, 105);

        Controls.AddRange(
        [
            EntityFormHelpers.Label("Ekipman Adı", 30, 48),
            txtName,
            EntityFormHelpers.Label("Açıklama", 30, 108),
            txtDescription
        ]);

        var save = EntityFormHelpers.SaveButton(230, 170);
        var cancel = EntityFormHelpers.CancelButton(350, 170);

        save.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
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
        }
    }
}
