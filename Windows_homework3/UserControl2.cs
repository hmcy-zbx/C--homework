using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class UserControl2 : UserControl
    {
        public UserControl2()
        {
            InitializeComponent();
        }
        private int selectedIndex = -1; // 当前选中的索引
        private List<string> responseLines = new List<string>(); // 保存响应结果的列表
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
        private async Task<string> GetPlayUrlAsync(int n)
        {
            string apiUrl = $"https://www.hhlqilongzhu.cn/api/dg_qqmusic.php?gm={Uri.EscapeDataString(textBox1.Text)}&n={n}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

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
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedIndex = listBox1.SelectedIndex + 1;
        }
        public void PlayAudio(string url)
        {
            axWindowsMediaPlayer1.URL = url;
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }
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

        private async void button3_Click(object sender, EventArgs e)
        {
            if (selectedIndex == -1 || selectedIndex >= responseLines.Count)
            {
                MessageBox.Show("请选择要播放的歌曲");
                return;
            }

            string playUrl = await GetPlayUrlAsync(selectedIndex); // 调用获取播放链接的异步方法

            if (!string.IsNullOrEmpty(playUrl))
            {
                PlayAudio(playUrl); // 在WebBrowser控件中加载播放链接
            }
            else
            {
                MessageBox.Show("未找到播放链接");
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (responseLines.Count > 0)
            {
                int index = (selectedIndex + 1);

                if (index >= responseLines.Count())
                {
                    index = 0;
                }

                string playUrl = await GetPlayUrlAsync(index); // 调用获取播放链接的异步方法

                PlayAudio(playUrl); // 在WebBrowser控件中加载播放链接

                selectedIndex = index;
                listBox1.SelectedIndex = index - 1;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.volume = trackBar1.Value;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop(); // 停止其他格式的播放
        }
    }
}
