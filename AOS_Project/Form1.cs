using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AOS_Project
{
    public partial class Form1 : Form
    {
        public static List<int> originalList = new List<int>();
        public static List<int> sortedList = new List<int>();
        public static List<int> fullSortedList = new List<int>();
        public static int head = 0, fromRange = 0, toRange = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                originalList.Clear();
                sortedList.Clear();
                fullSortedList.Clear();

                string[] cylinders = textBox1.Text.Split(' ');
                for (int i = 0; i < cylinders.Length; i++)
                {
                    originalList.Add(Convert.ToInt32(cylinders[i]));
                    sortedList.Add(Convert.ToInt32(cylinders[i]));
                    fullSortedList.Add(Convert.ToInt32(cylinders[i]));
                }

                head = (int)numericUpDown1.Value;
                sortedList.Add(head);
                fullSortedList.Add(head);

                fromRange = (int)numericUpDown2.Value;
                fullSortedList.Add(fromRange);
                toRange = (int)numericUpDown3.Value;
                fullSortedList.Add(toRange);

                Form2 form = new Form2();
                form.ShowDialog();
            }
        }
    }
}
