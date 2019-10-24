using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace third_party_two
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var leftNonEmpty = !String.IsNullOrEmpty(textBoxLeft.Text);
            var rightNonEmpty = !String.IsNullOrEmpty(textBoxRight.Text);

            if (leftNonEmpty && rightNonEmpty)
            {
                var result = "LEFT:" + textBoxLeft.Text + "|RIGHT:" + textBoxRight.Text; 

                var location = AppDomain.CurrentDomain.BaseDirectory;

                File.WriteAllText(location + "sight.txt", result);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var location = AppDomain.CurrentDomain.BaseDirectory;

            if (File.Exists(location + "sight.txt"))
            {
                File.Delete(location + "sight.txt");
            }
        }
    }
}
