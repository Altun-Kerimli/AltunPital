using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

using Font = System.Drawing.Font;

namespace AltunPital.UI.Desktop;

public static class UiTheme
{
    public static readonly Color Background = Color.FromArgb(245, 247, 250);
    public static readonly Color Surface = Color.White;
    public static readonly Color Primary = Color.FromArgb(0, 120, 150);
    public static readonly Color PrimaryDark = Color.FromArgb(0, 85, 110);
    public static readonly Color Text = Color.FromArgb(35, 40, 45);
    public static readonly Color Muted = Color.FromArgb(100, 110, 120);
    public static readonly Color Danger = Color.FromArgb(190, 55, 55);

    public static Button CreateButton(string text, int width = 180)
    {
        return new Button
        {
            Text = text,
            Width = width,
            Height = 42,
            FlatStyle = FlatStyle.Flat,
            BackColor = Primary,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Margin = new Padding(5)
        };
    }

    public static System.Windows.Forms.Label CreateTitle(string text)
    {
        return new System.Windows.Forms.Label
        {
            Text = text,
            AutoSize = true,
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            ForeColor = PrimaryDark
        };
    }

    public static System.Windows.Forms.Label CreateSubtitle(string text)
    {
        return new System.Windows.Forms.Label
        {
            Text = text,
            AutoSize = true,
            Font = new Font("Segoe UI", 10),
            ForeColor = Muted
        };
    }
}
