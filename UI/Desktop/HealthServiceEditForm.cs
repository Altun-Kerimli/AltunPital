using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.UI.Desktop;

public class HealthServiceEditForm : Form
{
    private readonly TextBox txtName;
    private readonly TextBox txtDescription;
    private readonly NumericUpDown numDuration;


    public string ServiceName =>
        txtName.Text.Trim();

    public string Description =>
        txtDescription.Text.Trim();

    public int Duration =>
        (int)numDuration.Value;


    public HealthServiceEditForm(
        HealthService? service = null)
    {
        Text =
            service == null
                ? "Yeni Hizmet"
                : "Hizmet Düzenle";

        Width = 500;
        Height = 340;

        StartPosition =
            FormStartPosition.CenterParent;

        FormBorderStyle =
            FormBorderStyle.FixedDialog;


        txtName =
            EntityFormHelpers.TextBox(
                150,
                40);

        txtDescription =
            EntityFormHelpers.TextBox(
                150,
                100);


        numDuration =
            new NumericUpDown
            {
                Left = 150,
                Top = 160,
                Width = 150,
                Minimum = 5,
                Maximum = 480,
                Value = 30
            };


        Controls.AddRange(
        [
            EntityFormHelpers.Label(
                "Hizmet Adı",
                30,
                43),

            txtName,

            EntityFormHelpers.Label(
                "Açıklama",
                30,
                103),

            txtDescription,

            EntityFormHelpers.Label(
                "Süre (dk)",
                30,
                163),

            numDuration
        ]);


        var save =
            EntityFormHelpers.SaveButton(
                230,
                220);

        var cancel =
            EntityFormHelpers.CancelButton(
                350,
                220);


        save.Click +=
            (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(
                        txtName.Text))
                {
                    MessageBox.Show(
                        "Hizmet adı zorunludur.");

                    DialogResult =
                        DialogResult.None;
                }
            };


        Controls.Add(save);
        Controls.Add(cancel);

        AcceptButton = save;
        CancelButton = cancel;


        if (service != null)
        {
            txtName.Text =
                service.Name;

            txtDescription.Text =
                service.Description;

            numDuration.Value =
                service.DurationMinutes;
        }
    }
}