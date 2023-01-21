﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicDLP
{
    public partial class OptionsForm : Form
    {

        public OptionsForm()
        {
            InitializeComponent();
        }

        public void LoadAllSettings() {
            cbShowOutputConsole.Checked = (bool) Properties.Settings.Default["showConsoleOutput"];
            cbClearPreviousOutput.Checked = (bool) Properties.Settings.Default["clearPreviousOutput"];
        }

        public void SaveAllSettings() {
            Properties.Settings.Default["showConsoleOutput"] = cbShowOutputConsole.Checked;
            Properties.Settings.Default["clearPreviousOutput"] = cbClearPreviousOutput.Checked;

            Properties.Settings.Default.Save();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            lblYtdlpInstalled.Text = File.Exists(GlobalHelpers.applicationDownloadPath + "\\yt-dlp.exe") 
                ? "Installed!" 
                : "Not installed!";

            downloadProgress.Visible = false;

            LoadAllSettings();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            // Apply settings here
        }

        private void lnkLblYtdlp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/yt-dlp/yt-dlp/");
        }

        private void btnDownloadYtdlp_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("This will now download and install yt-dlp to your system (local install, not adding to PATH). Continue?", "Confirmation required", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Directory.CreateDirectory(GlobalHelpers.applicationDownloadPath);

                using (WebClient wc = new WebClient())
                {
                    wc.DownloadProgressChanged += Wc_DownloadProgressChanged; ;
                    wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(
                        // Param1 = Link of file
                        new Uri(URLConstants.ytdlpDownloadUrl),
                        // Param2 = Path to save
                        GlobalHelpers.ytdlpPath
                    );

                    lblYtdlpInstalled.Text = "Downloading...";
                    downloadProgress.Visible = true;
                }
            }
        }

        private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            downloadProgress.Visible = false;

            if (e.Error != null)
            {
                MessageBox.Show("The operation did not complete successfully because of the following error: " + e.Error.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblYtdlpInstalled.Text = "Installation Failed!";
            } else
            {
                lblYtdlpInstalled.Text = "Installed!";

                DialogResult result = MessageBox.Show("yt-dlp was installed successfully!", "Operation Completed",  MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (result == DialogResult.OK)
                {
                    Process.Start("explorer.exe", GlobalHelpers.applicationDownloadPath);
                }
            }
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadProgress.Value = e.ProgressPercentage;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            SaveAllSettings();
        }
    }
}