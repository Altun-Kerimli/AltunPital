using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.UI.Desktop;

public class BranchEditForm : Form
{
    private readonly TextBox txtName;
    private readonly TextBox txtCity;
    private readonly TextBox txtDistrict;
    private readonly TextBox txtAddress;
    private readonly TextBox txtPhone;

    private readonly DateTimePicker dtOpening;
    private readonly DateTimePicker dtClosing;


    public string BranchName =>
        txtName.Text.Trim();

    public string City =>
        txtCity.Text.Trim();

    public string District =>
        txtDistrict.Text.Trim();

    public string Address =>
        txtAddress.Text.Trim();

    public string Phone =>
        txtPhone.Text.Trim();

    public TimeOnly Opening =>
        TimeOnly.FromDateTime(
            dtOpening.Value);

    public TimeOnly Closing =>
        TimeOnly.FromDateTime(
            dtClosing.Value);


    public BranchEditForm(
        Branch? branch = null)
    {
        Text =
            branch == null
                ? "Yeni Şube"
                : "Şube Düzenle";

        Width = 500;
        Height = 570;

        StartPosition =
            FormStartPosition.CenterParent;

        FormBorderStyle =
            FormBorderStyle.FixedDialog;

        MaximizeBox = false;
        MinimizeBox = false;


        txtName =
            EntityFormHelpers.TextBox(
                130,
                35);

        txtCity =
            EntityFormHelpers.TextBox(
                130,
                90);

        txtDistrict =
            EntityFormHelpers.TextBox(
                130,
                145);

        txtAddress =
            EntityFormHelpers.TextBox(
                130,
                200);

        txtPhone =
            EntityFormHelpers.TextBox(
                130,
                255);


        dtOpening =
            new DateTimePicker
            {
                Left = 130,
                Top = 310,
                Width = 150,
                Format =
                    DateTimePickerFormat.Time,
                ShowUpDown = true
            };

        dtClosing =
            new DateTimePicker
            {
                Left = 130,
                Top = 365,
                Width = 150,
                Format =
                    DateTimePickerFormat.Time,
                ShowUpDown = true
            };


        Controls.AddRange(
        [
            EntityFormHelpers.Label(
                "Şube Adı",
                25,
                38),

            txtName,

            EntityFormHelpers.Label(
                "Şehir",
                25,
                93),

            txtCity,

            EntityFormHelpers.Label(
                "İlçe",
                25,
                148),

            txtDistrict,

            EntityFormHelpers.Label(
                "Adres",
                25,
                203),

            txtAddress,

            EntityFormHelpers.Label(
                "Telefon",
                25,
                258),

            txtPhone,

            EntityFormHelpers.Label(
                "Açılış",
                25,
                313),

            dtOpening,

            EntityFormHelpers.Label(
                "Kapanış",
                25,
                368),

            dtClosing
        ]);


        var save =
            EntityFormHelpers.SaveButton(
                235,
                435);

        var cancel =
            EntityFormHelpers.CancelButton(
                355,
                435);


        save.Click +=
            (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(
                        txtName.Text) ||
                    string.IsNullOrWhiteSpace(
                        txtCity.Text))
                {
                    MessageBox.Show(
                        "Şube adı ve şehir zorunludur.");

                    DialogResult =
                        DialogResult.None;
                }
            };


        Controls.Add(save);
        Controls.Add(cancel);

        AcceptButton = save;
        CancelButton = cancel;


        if (branch != null)
        {
            txtName.Text =
                branch.Name;

            txtCity.Text =
                branch.City;

            txtDistrict.Text =
                branch.District;

            txtAddress.Text =
                branch.Address;

            txtPhone.Text =
                branch.Phone;

            dtOpening.Value =
                DateTime.Today.Add(
                    branch.OpeningTime.ToTimeSpan());

            dtClosing.Value =
                DateTime.Today.Add(
                    branch.ClosingTime.ToTimeSpan());
        }
        else
        {
            dtOpening.Value =
                DateTime.Today.AddHours(8);

            dtClosing.Value =
                DateTime.Today.AddHours(18);
        }
    }
}