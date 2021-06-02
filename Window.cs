﻿using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KEE
{
    public partial class Window : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse);

        string config_file = Path.Combine(Environment.CurrentDirectory, "KEE.exe.config");
        public string baselink = "http://kellphy.com/emotes/";

        public void FindOrCreate()
        {
            if (!File.Exists(config_file))
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile($"{baselink}/KEE.exe.config", config_file);
                }
            }
        }

        public void Border(bool border)
        {
            if (!border)
            {
                FormBorderStyle = FormBorderStyle.None;
                Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            }
        }
        public void RefreshWindow()
        {
            ColorProfiles();
            SettingsRefresh();
        }
        public virtual void SettingsRefresh()
        {
            FindOrCreate();
            TopMost = (bool)Properties.Settings.Default["AOT"];
        }
        public virtual void ColorProfiles()
        {
            FindOrCreate();

            BackColor = (Color)Properties.Settings.Default["Color_BG"];
            for (int ix = Controls.Count - 1; ix >= 0; ix--)
            {
                if (Controls[ix] is Button)
                {
                    Controls[ix].BackColor = (Color)Properties.Settings.Default["Button_BG"];
                    Controls[ix].ForeColor = (Color)Properties.Settings.Default["Color_FG"];
                }
                else if (Controls[ix] is Label)
                {
                    Controls[ix].ForeColor = (Color)Properties.Settings.Default["Color_FG"];
                }
            }
        }
        public virtual void SaveColors()
        {
            Properties.Settings.Default.Save();
            ColorProfiles();
        }
    }
}