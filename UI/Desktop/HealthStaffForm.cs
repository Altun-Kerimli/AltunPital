using AltunPital.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

using AltunPital.Interfaces;

using Font = System.Drawing.Font;
using Label = System.Windows.Forms.Label;

namespace AltunPital.UI.Desktop;

public class HealthStaffForm : DesktopShellForm
{
    public HealthStaffForm(User user, IUserAccountService accountService)
        : base(user, accountService, "Sağlık Personeli")
    {
        AddSidebarButton("Ana Sayfa", 160, ShowHome);
        AddSidebarButton("Profilim", 210, OpenProfile);
        AddLogoutButton();

        ShowHome();
    }

    private void ShowHome()
    {
        var panel = new Panel();

        var title = new Label
        {
            Text = $"Hoş geldiniz, " +
                $"{CurrentUser.FirstName} " +
                $"{CurrentUser.LastName}",
            AutoSize = true,
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            Left = 20,
            Top = 25
        };

        var description = new Label
        {
            Text =
                "Sağlık personeli hesabınız aktif.\n\n" +
                "Profil bilgilerinizi görüntüleyebilir " +
                "ve şifrenizi değiştirebilirsiniz.",
            AutoSize = true,
            Left = 20,
            Top = 90
        };

        panel.Controls.Add(title);
        panel.Controls.Add(description);

        ShowPage("Ana Sayfa", panel);
    }
}
