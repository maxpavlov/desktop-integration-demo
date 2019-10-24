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

namespace third_party_one
{
    public partial class HeartForm : Form
    {
        private const int skip = 10;
        private int skipped = 0;
        private List<int> cardio;
        private bool completed = false;

        public HeartForm()
        {
            InitializeComponent();
            cardio = new List<int>();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (cardio.Count < 20)
            {
                if (skipped >= skip)
                {
                    cardio.Add(e.X - e.Y);
                    skipped = 0;
                }
                else
                {
                    skipped++;
                }
            }
            else
            {
                completed = true;
                checkBox1.Checked = true;
                panel1.Hide();
                label1.Hide();
            }

        }

        private void HeartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(completed)
            {
                var result = string.Join(",", cardio.ToArray());

                var location = AppDomain.CurrentDomain.BaseDirectory;

                File.WriteAllText(location + "cardio.txt", result);
            }
        }

        private void HeartForm_Load(object sender, EventArgs e)
        {
            var location = AppDomain.CurrentDomain.BaseDirectory;

            if (File.Exists(location + "cardio.txt"))
            {
                File.Delete(location + "cardio.txt");
            }
        }
    }
}
