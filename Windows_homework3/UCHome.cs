using NAudio.Vorbis;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class UCHome : UserControl
    {
        List<string> localmusiclist = new List<string> { };
        private WaveOutEvent waveOut = new WaveOutEvent();
        private VorbisWaveReader currentVorbisReader;
        public UCHome()
        {
            InitializeComponent();
        }
        private void musicplay(string filename)
        {
            string extension = Path.GetExtension(filename).ToLower();
            if (extension == ".ogg")
            {
                if (currentVorbisReader != null)
                {
                    // 确保先停止当前播放再释放资源
                    if (waveOut.PlaybackState != PlaybackState.Stopped)
                    {
                        waveOut.Stop();
                    }
                    currentVorbisReader.Dispose();
                }

                currentVorbisReader = new VorbisWaveReader(filename);
                waveOut.Init(currentVorbisReader);
                waveOut.Play();
            }
            else
            {
                axWindowsMediaPlayer1.URL = filename;
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (localmusiclist.Count > 0)
            {
                axWindowsMediaPlayer1.URL = localmusiclist[listBox1.SelectedIndex];
                musicplay(axWindowsMediaPlayer1.URL);
                label1.Text = Path.GetFileNameWithoutExtension(localmusiclist[listBox1.SelectedIndex]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] files = { };


            openFileDialog1.Filter = "选择音频|*.mp3;*.flac;*.wav;*.ogg";
            //同时打开多个文件
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //清空原有列表
                listBox1.Items.Clear();
                localmusiclist.Clear();

                if (files != null)
                {
                    Array.Clear(files, 0, files.Length);
                }

                files = openFileDialog1.FileNames;
                string[] array = files;
                foreach (string file in array)
                {
                    listBox1.Items.Add(file);
                    localmusiclist.Add(file);
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.volume = trackBar1.Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (waveOut.PlaybackState == PlaybackState.Playing)
            {
                waveOut.Pause(); // 暂停播放OGG
            }

            axWindowsMediaPlayer1.Ctlcontrols.stop(); // 停止其他格式的播放
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (localmusiclist.Count > 0)
            {
                int index = (listBox1.SelectedIndex + 1);

                if (index >= localmusiclist.Count())
                {
                    index = 0;
                }

                axWindowsMediaPlayer1.URL = localmusiclist[index];

                musicplay(axWindowsMediaPlayer1.URL);

                label1.Text = Path.GetFileNameWithoutExtension(localmusiclist[index]);

                listBox1.SelectedIndex = index;
            }

        }

        
    }
}
