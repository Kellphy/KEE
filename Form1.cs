﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KEE
{
    public partial class Form1 : Form
    {
        public int option, integer, division, page, paging;
        public string searchEmotes, firstLabel;
        public bool processStop, processStopped;
        public Color color_bg, button_bg, color_fg, nonTextColor, textbox_bg, color_link,color_vlink;

        public List<Image> imagesList;
        public List<Button> buttonList;
        public List<string> emoteString;

        MatchCollection matches;

        public Form1()
        {
            InitializeComponent();
        }
        public void Form1_Load(object sender, EventArgs e)
        {
            ColorProfiles();

            option = 1;
            division = 10;
            page = 0;
            paging = division * 50;
            searchEmotes = "Emote to Search";
            firstLabel = "Click the info button in the bottom right corner for tips.";

            processStop = false;
            processStopped = true;

            label1.Text = firstLabel;
            textBox2.Text = searchEmotes;

            ImageFirstGetting();
            ImageFilter();
        }
        //Color Profiles
        public void DefaultColors()
        {
            color_bg = Color.FromArgb(125, 133, 151);
            color_fg = Color.FromArgb(0, 48, 102);
            button_bg = Color.FromArgb(113, 122, 142);
            nonTextColor = Color.FromArgb(92, 103, 125);
            textbox_bg = Color.FromArgb(151, 157, 172);
            color_link = Color.FromArgb(0, 0, 255);
            color_vlink = Color.FromArgb(128, 0, 128);

            string curFile = $"{Environment.CurrentDirectory}\\KEE.exe.config";
            if (File.Exists(curFile))
            {
                Properties.Settings.Default["Color_BG"]= color_bg;
                Properties.Settings.Default["Color_FG"]= color_fg;
                Properties.Settings.Default["Button_BG"]= button_bg;
                Properties.Settings.Default["Color_NonText"]= nonTextColor;
                Properties.Settings.Default["TextBox_BG"]= textbox_bg;
                Properties.Settings.Default["Color_Link"]= color_link;
                Properties.Settings.Default["Color_VLink"]= color_vlink;
            }
        }
        public void ColorProfiles()
        {
            string curFile = $"{Environment.CurrentDirectory}\\KEE.exe.config";
            if (File.Exists(curFile))
            {
                color_bg = (Color)Properties.Settings.Default["Color_BG"];
                color_fg = (Color)Properties.Settings.Default["Color_FG"];
                button_bg = (Color)Properties.Settings.Default["Button_BG"];
                nonTextColor = (Color)Properties.Settings.Default["Color_NonText"];
                textbox_bg = (Color)Properties.Settings.Default["TextBox_BG"];
                color_link = (Color)Properties.Settings.Default["Color_Link"];
                color_vlink = (Color)Properties.Settings.Default["Color_VLink"];
            }
            else
            {
                DefaultColors();
            }

            this.BackColor = color_bg;
            for (int ix = this.Controls.Count - 1; ix >= 0; ix--)
            {
                if (this.Controls[ix] is Button)
                {
                    this.Controls[ix].BackColor = button_bg;
                    this.Controls[ix].ForeColor = color_fg;
                }
                else if (this.Controls[ix] is LinkLabel)
                {
                    ((LinkLabel)this.Controls[ix]).LinkColor = color_link;
                    ((LinkLabel)this.Controls[ix]).VisitedLinkColor = color_vlink;
                }
                else if (this.Controls[ix] is Label)
                {
                    this.Controls[ix].ForeColor = color_fg;
                }
                else if ( this.Controls[ix] is TextBox)
                {
                    this.Controls[ix].ForeColor = nonTextColor;
                    this.Controls[ix].BackColor = textbox_bg;
                }
            }
        }
        //First Get
        public void ImageFirstGetting()
        {
            emoteString = new List<string>();
            string url = "http://kellphy.com/emotes/";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string html = reader.ReadToEnd();
                    Regex regex = new Regex(GetDirectoryListingRegexForUrl(url));
                    matches = regex.Matches(html);
                }
            }
        }
        public static string GetDirectoryListingRegexForUrl(string url)
        {
            if (url.Equals("http://kellphy.com/emotes/"))
            {
                return "<a href=\".*.png\">(?<name>.*).png</a>";
            }
            throw new NotSupportedException();
        }
        //Options
        private void button2_Click(object sender, EventArgs e)
        {
            option = 1;
            label1.ForeColor = color_fg;
            label1.Text = "Now using the 1st Option.";
        }
        private void button3_Click(object sender, EventArgs e)
        {
            option = 2;
            label1.ForeColor = color_fg;
            label1.Text = "Now using the 2nd Option.";
        }
        private void button4_Click(object sender, EventArgs e)
        {
            option = 3;
            label1.ForeColor = color_fg;
            label1.Text = "Now using the 3rd Option.";
        }
        //Links
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitLink(linkLabel1, "http://kellphy.com/");
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitLink(linkLabel2, "https://discord.gg/ycYmMmP");
        }
        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitLink(linkLabel4, "https://github.com/Kellphy/KEE/releases/");
        }
        public void VisitLink(LinkLabel label, string link)
        {
            try
            {
                label.LinkVisited = true;
                System.Diagnostics.Process.Start(link);
            }
            catch (Exception ex) { SendErrorMessage(ex.Message.ToString()); }
        }
        //Search
        private void button5_Click(object sender, EventArgs e)
        {
            page = 0;
            NewSearch(true);
        }
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                page = 0;
                NewSearch(false);
            }
        }
        public async void NewSearch(bool button)
        {
            try
            {
                processStop = true;
                while (!processStopped) await Task.Delay(100);

                if (textBox2.Text == searchEmotes || textBox2.Text.Length < 1)
                {
                    label1.ForeColor = color_fg;
                    label1.Text = firstLabel;
                    ImageFilter("");
                }
                else
                {
                    label1.ForeColor = color_fg;
                    label1.Text = $"Searching for {textBox2.Text.ToLower()} ...";
                    ImageFilter(textBox2.Text.ToLower());
                }

                if (!button) textBox2.Text = ""; else TextColor(textBox2, searchEmotes, "", nonTextColor, true);
            }
            catch (Exception ex) { SendErrorMessage(ex.Message.ToString()); }
        }
        public async void ImageFilter(string keyword = "")
        {
            try
            {
                processStop = false;
                processStopped = false;

                integer = 0;
                imagesList = new List<Image>();
                emoteString = new List<string>();
                buttonList = new List<Button>();

                for (int ix = flowLayoutPanel1.Controls.Count - 1; ix >= 0; ix--)
                {
                    if (flowLayoutPanel1.Controls[ix] is Button) flowLayoutPanel1.Controls[ix].Dispose();
                }

                ImageGetting(keyword);
                label2.Text = $"{emoteString.Count} Emotes";
                label3.Text = $"{page+1} / {emoteString.Count/paging+1}";

                int emotesOnPage = Math.Min(paging, emoteString.Count - page * paging);
                for (int x = 0; x <= emotesOnPage/division; x++)
                {
                    int max = Math.Min(x * division + division, emotesOnPage);
                    await Task.Run(() => ImageLoading(x, division, max));

                    if (processStop) break;

                    label4.Text = $"Loaded {integer} / {emotesOnPage}";
                    for (int y = x * division; y < max; y++)
                    {
                        flowLayoutPanel1.Controls.Add(buttonList[y]);
                    }
                }
                processStopped = true;
                processStop = false;
            }
            catch (Exception ex) { SendErrorMessage(ex.Message.ToString()); }
        }
        public void ImageGetting(string keyword = "")
        {
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        if (match.Groups["name"].ToString() == keyword)
                        {
                            Execution(keyword);
                        }
                        if (match.Groups["name"].ToString().Contains(keyword))
                        {
                            emoteString.Add(match.Groups["name"].ToString());
                        }
                    }
                }
            }
        }
        public void Execution(string emoteToSearch)
        {
            try
            {
                string image_name = emoteToSearch;

                string link = $"http://kellphy.com/emotes/{image_name}.png";

                Image i = DownloadImage(link);
                Bitmap image = new Bitmap(i);

                switch (option)
                {
                    case 1:
                        Bitmap blank = new Bitmap(Convert.ToInt32(i.Width), Convert.ToInt32(i.Height));
                        Graphics g = Graphics.FromImage(blank);
                        g.Clear(Color.FromArgb(54, 57, 63));
                        g.DrawImage(i, 0, 0, Convert.ToInt32(i.Width), Convert.ToInt32(i.Height));

                        Bitmap tempImage = new Bitmap(blank);
                        blank.Dispose();

                        Clipboard.SetImage(new Bitmap(tempImage));
                        tempImage.Dispose();
                        break;
                    case 2:
                        new Option2().Start(image);
                        break;
                    case 3:
                        Clipboard.SetText(link);
                        break;
                    default:
                        break;
                }

                i.Dispose();
                image.Dispose();

                label1.ForeColor = Color.LightGreen;
                label1.Text = $"{image_name} - Copied to clipboard!";
            }
            catch (Exception ex) { SendErrorMessage(ex.Message.ToString()); }
        }
        public Image DownloadImage(string fromUrl)
        {
            using (WebClient webClient = new WebClient())
            {
                using (Stream stream = webClient.OpenRead(fromUrl))
                {
                    return Image.FromStream(stream);
                }
            }
        }
        public async Task ImageLoading(int x, int division, int max)
        {
            for (int y = x * division; y < max; y++)
            {
                if (processStop) break;
                string link = $"http://kellphy.com/emotes/{emoteString[y+page*paging]}.png";

                imagesList.Add(DownloadImage(link));

                Button button = new Button();
                button.Name = emoteString[y + page*paging];
                button.TabStop = false;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.Size = new Size(48, 48);
                button.Click += this.buttonGenerated_Click;
                button.BackgroundImageLayout = ImageLayout.Zoom;
                button.BackgroundImage = imagesList.ElementAt(y);
                buttonList.Add(button);
                integer++;
            }
            await Task.CompletedTask;
        }
        private void buttonGenerated_Click(object sender, EventArgs e)
        {
            try
            {
                Execution((sender as Button).Name);
            }
            catch (Exception ex) { SendErrorMessage(ex.Message.ToString()); }
        }
        //Pages & Reset
        private void button6_Click(object sender, EventArgs e)
        {
            if(page > 0)
            {
                page--;
                NewSearch(true);
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (page < emoteString.Count / paging)
            {
                page++;
                NewSearch(true);
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            ColorProfiles();
            page = 0;
            TextColor(textBox2, searchEmotes, "", nonTextColor, true);
            NewSearch(true);
        }
        //TextBox placeholder text
        private void textBox2_Enter(object sender, EventArgs e)
        {
            TextColor(textBox2, "", searchEmotes, color_fg);
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {
            TextColor(textBox2, searchEmotes, "", nonTextColor);
        }
        public void TextColor(TextBox textBox, string text, string reqText, Color color, bool forcedReplace = false)
        {
            if (textBox.Text == reqText || forcedReplace)
            {
                textBox.ForeColor = color;
                textBox.Text = text;
            }
        }
        //Errors
        public void SendErrorMessage(string error)
        {
            label1.ForeColor = Color.DarkRed;
            label1.Text = error;
        }
        //Info
        private void button1_Click(object sender, EventArgs e)
        {
            new Form2().Start();
        }
        //Profiles
        private void button9_Click(object sender, EventArgs e)
        {
            Form3 Frm = new Form3();
            Frm.ClosePanel += HandleCloseRequest;
            Frm.Start();
        }
        private void HandleCloseRequest(object sender, EventArgs e)
        {
            ColorProfiles();
        }
    }
}