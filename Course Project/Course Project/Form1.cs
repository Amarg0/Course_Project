using System;
using System.Threading;
using System.Windows.Forms;

namespace Course_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(Parser.GetAnalysisResult));
            thread.Start(new ArgsForAnalysisThread(progressBar1,richTextBox1));
        }

        private void clear_button_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = null;
        }
    }
}