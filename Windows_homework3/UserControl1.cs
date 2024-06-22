using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class UserControl1 : UserControl
    {
        private int selectedIndex = -1; // 当前选中的索引
        private List<string> responseLines = new List<string>(); // 保存响应结果的列表

        public UserControl1()
        {
            InitializeComponent();
            listBox1.SelectedIndexChanged += new EventHandler(listBox1_SelectedIndexChanged);
        }

        // 搜索按钮点击事件处理程序
        private async void button1_Click(object sender, EventArgs e)
        {
            string songName = textBox1.Text;
            if (string.IsNullOrWhiteSpace(songName))
            {
                MessageBox.Show("请输入歌曲名称");
                return;
            }

            listBox1.Items.Clear();
            responseLines.Clear();
            var responseBody = await SearchSongsAsync(songName); // 调用搜索歌曲的异步方法
            if (!string.IsNullOrEmpty(responseBody))
            {
                var lines = responseBody.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    responseLines.Add(line);
                    listBox1.Items.Add(line); // 将结果添加到列表框中
                }
            }
            else
            {
                MessageBox.Show("未找到歌曲");
            }
        }

        // 下载按钮点击事件处理程序
        private async void button2_Click(object sender, EventArgs e)
        {
            if (selectedIndex == -1 || selectedIndex >= responseLines.Count)
            {
                MessageBox.Show("请选择要下载的歌曲");
                return;
            }

            string downloadUrl = await GetDownloadUrlAsync(selectedIndex); // 调用获取下载链接的异步方法

            if (!string.IsNullOrEmpty(downloadUrl))
            {
                string fileName = $"{Guid.NewGuid()}.flac"; // 用GUID作为文件名

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.FileName = fileName;
                    saveFileDialog.Filter = "FLAC files (*.flac)|*.flac|All files (*.*)|*.*";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        progressBar1.Value = 0; // 重置进度条
                        await DownloadSongAsync(downloadUrl, saveFileDialog.FileName); // 调用下载歌曲的异步方法
                        MessageBox.Show("下载完成");
                    }
                }
            }
            else
            {
                MessageBox.Show("未找到播放链接");
            }
        }

        // 搜索歌曲的异步方法
        private async Task<string> SearchSongsAsync(string songName)
        {
            string apiUrl = $"https://www.hhlqilongzhu.cn/api/dg_mgmusic_24bit.php?msg={Uri.EscapeDataString(songName)}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }

        // 获取下载链接的异步方法
        private async Task<string> GetDownloadUrlAsync(int n)
        {
            string apiUrl = $"https://www.hhlqilongzhu.cn/api/dg_mgmusic_24bit.php?msg={Uri.EscapeDataString(textBox1.Text)}&n={n}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                textBox2.Text = responseBody; // 将响应内容放入textBox2中

                // 检查响应内容，解析播放链接
                var lines = responseBody.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    if (line.StartsWith("播放链接："))
                    {
                        return line.Substring(5).Trim();
                    }
                }
            }

            return null;
        }

        // 列表框选中项改变事件处理程序
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedIndex = listBox1.SelectedIndex + 1;
        }

        // 下载歌曲的异步方法
        private async Task DownloadSongAsync(string url, string filePath)
        {
            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromMinutes(30) })
            {
                // 获取响应流
                using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    long? totalBytes = response.Content.Headers.ContentLength;

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                           fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var buffer = new byte[8192];
                        long totalReadBytes = 0;
                        int readBytes;

                        while ((readBytes = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, readBytes);
                            totalReadBytes += readBytes;

                            if (totalBytes.HasValue)
                            {
                                // 更新进度条
                                progressBar1.Value = (int)((totalReadBytes * 100) / totalBytes.Value);
                            }
                        }
                    }
                }
            }
        }
    }
}
