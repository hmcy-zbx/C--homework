using System;

using System.Windows.Forms;
namespace UI
{
    public partial class Form1 : Form
    {
        // 定义各界面对象
        public UCHome uchome;
        public UserControl1 userControl1;
        public UserControl2 userControl2;

        public Form1()
        {
            InitializeComponent();
            panelContain.BringToFront();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            uchome = new UCHome();    //实例化
            uchome.Show(); // 将窗体一进行显示
            panelContain.Controls.Clear(); // 清空原容器上的控件
            panelContain.Controls.Add(uchome); // 将窗体一加入容器panelContain
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            uchome.Show(); // 将窗体一进行显示
            panelContain.Controls.Clear(); // 清空原容器上的控件
            panelContain.Controls.Add(uchome); // 将窗体一加入容器panelContain
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            userControl1 = new UserControl1();    //实例化
            userControl1.Show(); // 将窗体二进行显示
            panelContain.Controls.Clear(); // 清空原容器上的控件
            panelContain.Controls.Add(userControl1); // 将窗体加入容器panelContain
        }

        private void button3_Click(object sender, EventArgs e)
        {
            userControl2 = new UserControl2();    //实例化
            userControl2.Show(); // 将窗体二进行显示
            panelContain.Controls.Clear(); // 清空原容器上的控件
            panelContain.Controls.Add(userControl2); // 将窗体加入容器panelContain
        }
    }
}
