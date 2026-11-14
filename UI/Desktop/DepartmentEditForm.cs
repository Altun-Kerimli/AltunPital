using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.UI.Desktop;

public class DepartmentEditForm : Form
{
    private readonly ComboBox cmbBranch;
    private readonly TextBox txtName;
    private readonly TextBox txtDescription;

    public Branch SelectedBranch => (Branch)cmbBranch.SelectedItem!;
    public string DepartmentName => txtName.Text.Trim();
    public string Description => txtDescription.Text.Trim();

    public DepartmentEditForm(List<Branch> branches, Department? department = null)
    {
        Text = department == null ? "Yeni Bölüm" : "Bölüm Düzenle";

        Width = 500;
        Height = 350;

        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        MaximizeBox = false;

        cmbBranch = EntityFormHelpers.ComboBox(140, 40);
        cmbBranch.DataSource = branches;
        cmbBranch.DisplayMember = nameof(Branch.Name);

        txtName = EntityFormHelpers.TextBox(140, 100);
        txtDescription = EntityFormHelpers.TextBox(140, 160);

        Controls.AddRange(
        [
            EntityFormHelpers.Label("Şube", 30, 43),
            cmbBranch,
            EntityFormHelpers.Label("Bölüm Adı", 30, 103),
            txtName,
            EntityFormHelpers.Label("Açıklama", 30, 163),
            txtDescription
        ]);

        var save = EntityFormHelpers.SaveButton(230, 225);
        var cancel = EntityFormHelpers.CancelButton(350, 225);

        save.Click += (_, _) =>
        {
            if (cmbBranch.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Şube ve bölüm adı zorunludur.");
                DialogResult = DialogResult.None;
            }
        };

        Controls.Add(save);
        Controls.Add(cancel);

        AcceptButton = save;
        CancelButton = cancel;

        if (department != null)
        {
            txtName.Text = department.Name;
            txtDescription.Text = department.Description;

            cmbBranch.SelectedItem = branches.FirstOrDefault(b =>
                b.BranchId == department.BranchId);
        }
    }
}
