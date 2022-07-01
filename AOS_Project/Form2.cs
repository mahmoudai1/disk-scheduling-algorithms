using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Graphics_Assignments;

namespace AOS_Project
{
    public partial class Form2 : Form
    {
        public class DrawGraph
        {
            public int X, Y;
            public int number;
            public Color color;
            public bool hidden;
            public Point point;
            public int circleSize;

            public DrawGraph(int x, int y, int cylinderNumber, Color whichColor, Point startPoint)
            {
                X = x;
                Y = y;
                number = cylinderNumber;
                color = whichColor;
                hidden = true;
                point = startPoint;
                circleSize = 0;
            }
        }

        Bitmap off;
        Timer t = new Timer();
        int ctTimer = 0;

        int spaceWidth = 0;
        int verticalLineHeight = 35;
        int whichPoint = 0;
        int filledCircle = 1;

        bool stopAnimating = false;
        bool lineIsMoving = false;
        

        List<int> originalList = new List<int>(Form1.originalList);
        List<int> sortedList = new List<int>(Form1.sortedList);
        List<int> fullSortedList = new List<int>(Form1.fullSortedList);
        List<int> printedNumebersList = new List<int>();

        List<DrawGraph> graphList = new List<DrawGraph>();
        List<Color> colorsList = new List<Color>();

        List<PointF> animatedLines = new List<PointF>();
        List<DDALine> hiddenConnectedLines = new List<DDALine>();

        int head = Form1.head, direction = 1, fromRange = Form1.fromRange, toRange = Form1.toRange;

        public Form2()
        {
            InitializeComponent();
            this.Paint += Form2_Paint;
            this.MouseDown += Form2_MouseDown;
            t.Tick += T_Tick;
            t.Start();
        }

        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < graphList.Count; i++)
                {
                    if (e.X > graphList[i].X && e.X < graphList[i].X + graphList[i].circleSize
                        && e.Y > graphList[i].Y && e.Y < graphList[i].Y + graphList[i].circleSize)
                    {
                        filledCircle *= -1;
                        break;
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetAll();
            recreateColors();
            switch (comboBox1.Text)
            {
                case "FCFS":
                    FCFSAlgorithm();
                    break;
                case "SSTF":
                    SSTFAlgorithm();
                    break;
                case "SCAN":
                    scanAlgorithm();
                    break;
                case "C-SCAN":
                    cScanAlgorithm();
                    break;
                case "LOOK":
                    lookAlgorithm();
                    break;
                case "C-LOOK":
                    cLookAlgorithm();
                    break;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            off = new Bitmap(this.Width, this.Height);
            initializeSetup();
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            drawDouble(e.Graphics);
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if(ctTimer % 1 == 0)
            {
                if (graphList.Count > 0)
                {
                    if (!stopAnimating)
                    {
                        if (graphList[whichPoint].circleSize < 40)
                        {
                            graphList[whichPoint].hidden = false;
                            graphList[whichPoint].circleSize += 20;
                        }
                        else
                        {
                            if (!lineIsMoving)
                            {
                                animatedLines.Add(new PointF(graphList[whichPoint].X, graphList[whichPoint].Y));
                                whichPoint++;
                                lineIsMoving = true;
                                if (whichPoint >= graphList.Count) stopAnimating = true;
                            }
                        }
                    }
                }
            }
            if(!stopAnimating && lineIsMoving)
            {
                animatedLines[whichPoint - 1] = hiddenConnectedLines[whichPoint - 1].getNextPoint(animatedLines[whichPoint - 1].X, animatedLines[whichPoint - 1].Y, 10 * (int)numericUpDown1.Value);
                if (hiddenConnectedLines[whichPoint - 1].flagStop)
                {
                    animatedLines[whichPoint - 1] = new PointF(graphList[whichPoint].X + 15, graphList[whichPoint].Y + 15);
                    lineIsMoving = false;
                }
            }
            ctTimer++;
            drawDouble(this.CreateGraphics());
        }

        public void recreateColors()
        {
            colorsList.Clear();
            for (int i = 0; i < fullSortedList.Count; i++)
            {
                Random rand = new Random();
                int num1 = 0;
                int num2 = 0;
                int num3 = 0;
                for (int j = 0; j < 99999; j++)
                {
                    num1 = rand.Next(0, 255);
                    num2 = rand.Next(0, 255);
                    num3 = rand.Next(0, 255);
                }
                colorsList.Add(Color.FromArgb(num1, num2, num3));
            }
        }

        public void resetAll()
        {
            if (radioButton1.Checked) direction = 0;
            else direction = 1;

            label3.Text = "";
            label3.Visible = false;
            graphList.Clear();
            whichPoint = 0;
            stopAnimating = false;
            lineIsMoving = false;
            hiddenConnectedLines.Clear();
            animatedLines.Clear();
        }

        public void initializeSetup()
        {
            sortedList.Sort();
            fullSortedList.Sort();
            spaceWidth = (this.Width - 80) / fullSortedList.Count;
            for (int i = 0; i < fullSortedList.Count; i++)
            {
                Random rand = new Random();
                int num1 = 0;
                int num2 = 0;
                int num3 = 0;
                for (int j = 0; j < 99999; j++)
                {
                    num1 = rand.Next(0, 255);
                    num2 = rand.Next(0, 255);
                    num3 = rand.Next(0, 255);
                }
                colorsList.Add(Color.FromArgb(num1, num2, num3));
            }
            int vx = 120;
            for (int i = 0; i < fullSortedList.Count; i++)
            {
                printedNumebersList.Add(vx);
                vx += spaceWidth - 5;
            }
        }

        public void FCFSAlgorithm()
        {
            int total = 0;
            int pointY = 300;   // Points Start at this Y-Point
            bool entered = false;

            if (true)   // Any Direction
            {
                for (int i = 0; i < originalList.Count; i++)
                {
                    if (i == 0 && !entered)
                    {
                        total += Math.Abs(originalList[i] - head);
                        int index = getFCFSIndexFromTheSortedList(head);
                        graphList.Add(new DrawGraph(printedNumebersList[index], pointY, fullSortedList[index], colorsList[index], new Point(printedNumebersList[index] + 10, pointY + 15)));
                        pointY += 70;
                        i--;
                        entered = true;
                    }
                    else
                    {
                        if(i < originalList.Count - 1)
                            total += Math.Abs(originalList[i] - originalList[i + 1]);

                        int index = getFCFSIndexFromTheSortedList(originalList[i]);
                        graphList.Add(new DrawGraph(printedNumebersList[index], pointY, fullSortedList[index], colorsList[index], new Point(printedNumebersList[index] + 10, pointY + 15)));
                        pointY += 70;
                    }
                }
            }


            pointY = 300;
            for (int i = 0; i < graphList.Count - 1; i++)
            {
                if (graphList[i + 1].X >= graphList[i].X)
                    hiddenConnectedLines.Add(new DDALine(graphList[i].X, pointY + 15, graphList[i + 1].X - 50, pointY + 15 + 70));      // 70 is the height difference between Points
                else
                    hiddenConnectedLines.Add(new DDALine(graphList[i].X, pointY + 15, graphList[i + 1].X + 50, pointY + 15 + 70));

                pointY += 70;
            }

            label3.Text = "Total head movements incurred while servicing these requests = " + total;
            label3.Location = new Point(920, this.Height - 400);
            label3.Visible = true;
        
        }

        public int getFCFSIndexFromTheSortedList(int cylinder)
        {
            for (int i = 0; i < fullSortedList.Count; i++)
            {
                if (fullSortedList[i] == cylinder)
                {
                    return i;
                }
            }
            return 0;
        }

        public void SSTFAlgorithm()
        {
            int total = 0;
            int j = 0;  // Head Index
            int k = 0;  // Holds last cylinder
            int pointY = 300;   // Points Start at this Y-Point

            for (int i = 0; i < fullSortedList.Count; i++)
            {
                if (fullSortedList[i] == head)
                {
                    j = i;
                    break;
                }
            }

            if (direction == 1)
            {
                for (int i = j; i < fullSortedList.Count - 1; i++)
                {
                    
                    if (i < fullSortedList.Count - 2 && fullSortedList[i] != originalList[originalList.Count - 1])
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i + 1]);
                    }

                    graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                    pointY += 70;
                    if (fullSortedList[i] == originalList[originalList.Count - 1])
                    {
                        k = i;
                        break;
                    }
                }

                for (int i = j - 1; i > 0; i--)
                {
                    if (i == j - 1)
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[k]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                    else
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i + 1]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                }

                for(int i = k + 1; i < fullSortedList.Count - 1; i++)
                {
                    if(i == k + 1)
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[1]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                    else
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i - 1]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                }
                //82 170 43 140 24 16 190
            }
            else
            {
                for (int i = j; i > 0; i--)
                {
                    if (i > 1)
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i - 1]);
                        Console.WriteLine(fullSortedList[i] + " - " + fullSortedList[i - 1]);
                    }

                    graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                    pointY += 70;
                }

                for (int i = j + 1; i < fullSortedList.Count - 1; i++)
                {
                    if (i == j + 1)
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[1]);
                        Console.WriteLine(fullSortedList[i] + " - " + fullSortedList[1]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                    else
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i - 1]);
                        Console.WriteLine(fullSortedList[i] + " - " + fullSortedList[i - 1]);

                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                }
            }


            pointY = 300;
            for (int i = 0; i < graphList.Count - 1; i++)
            {
                if (graphList[i + 1].X >= graphList[i].X)
                    hiddenConnectedLines.Add(new DDALine(graphList[i].X, pointY + 15, graphList[i + 1].X - 50, pointY + 15 + 70));      // 70 is the height difference between Points
                else
                    hiddenConnectedLines.Add(new DDALine(graphList[i].X, pointY + 15, graphList[i + 1].X + 50, pointY + 15 + 70));

                pointY += 70;
            }

            label3.Text = "Total head movements incurred while servicing these requests = " + total;
            label3.Location = new Point(920, this.Height - 400);
            label3.Visible = true;
        }

        public void scanAlgorithm()
        {
            int total = 0;
            int j = 0;  // Head Index
            int pointY = 300;   // Points Start at this Y-Point

            for (int i = 0; i < fullSortedList.Count; i++)
            {
                if (fullSortedList[i] == head)
                {
                    j = i;
                    break;
                }
            }

            if (direction == 1)
            {
                for (int i = j; i < fullSortedList.Count; i++)
                {
                    if(i < fullSortedList.Count - 1)
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i + 1]);

                    graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                    pointY += 70;
                }

                for (int i = j - 1; i > 0; i--)
                {
                    if (i == j - 1)
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[fullSortedList.Count - 1]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                    else
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i + 1]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                    
                }
            }
            else
            {
                for (int i = j; i > 0; i--)
                {
                    if(i > 1)
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i - 1]);

                    graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                    pointY += 70;
                }

                for (int i = j + 1; i < fullSortedList.Count; i++)
                {
                    if (i == j + 1)
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[1]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                    else
                    {
                        if (i < fullSortedList.Count - 1)
                            total += Math.Abs(fullSortedList[i] - fullSortedList[i + 1]);

                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                }
            }


            pointY = 300;
            for(int i = 0; i < graphList.Count - 1; i++)
            {
                if(graphList[i + 1].X >= graphList[i].X)
                    hiddenConnectedLines.Add(new DDALine(graphList[i].X, pointY + 15, graphList[i + 1].X - 50, pointY + 15 + 70));      // 70 is the height difference between Points
                else
                    hiddenConnectedLines.Add(new DDALine(graphList[i].X, pointY + 15, graphList[i + 1].X + 50, pointY + 15 + 70));

                pointY += 70;
            }

            label3.Text = "Total head movements incurred while servicing these requests = " + total;
            label3.Location = new Point(920, this.Height - 400);
            label3.Visible = true;
        }

        public void cScanAlgorithm()
        {
            int total = 0;
            int j = 0;  // Head Index
            int pointY = 300;   // Points Start at this Y-Point

            for (int i = 0; i < fullSortedList.Count; i++)
            {
                if (fullSortedList[i] == head)
                {
                    j = i;
                    break;
                }
            }

            if (direction == 1)
            {
                for (int i = j; i < fullSortedList.Count; i++)
                {
                    if (i < fullSortedList.Count - 1)
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i + 1]);

                    graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                    pointY += 70;
                }

                for (int i = 0; i < j; i++)
                {
                    if (i == 0)
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[fullSortedList.Count - 1]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                    else
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i - 1]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }

                }
            }
            else
            {
                for (int i = j; i >= 0; i--)
                {
                    if (i >= 1)
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i - 1]);

                    graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                    pointY += 70;
                }

                for (int i = fullSortedList.Count - 1; i > j; i--)
                {
                    if (i == fullSortedList.Count - 1)
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[0]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                    else
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i + 1]);

                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                }
            }

            //MessageBox.Show("" + graphList.Count());

            pointY = 300;
            for (int i = 0; i < graphList.Count - 1; i++)
            {
                if (graphList[i + 1].X >= graphList[i].X)
                    hiddenConnectedLines.Add(new DDALine(graphList[i].X, pointY + 15, graphList[i + 1].X - 50, pointY + 15 + 70));      // 70 is the height difference between Points
                else
                    hiddenConnectedLines.Add(new DDALine(graphList[i].X, pointY + 15, graphList[i + 1].X + 50, pointY + 15 + 70));

                pointY += 70;
            }

            label3.Text = "Total head movements incurred while servicing these requests = " + total;
            label3.Location = new Point(920, this.Height - 400);
            label3.Visible = true;
        }

        public void lookAlgorithm()
        {
            int total = 0;
            int j = 0;  // Head Index
            int pointY = 300;   // Points Start at this Y-Point

            for (int i = 0; i < fullSortedList.Count; i++)
            {
                if (fullSortedList[i] == head)
                {
                    j = i;
                    break;
                }
            }

            if (direction == 1)
            {
                for (int i = j; i < fullSortedList.Count - 1; i++)
                {
                    if (i < fullSortedList.Count - 2)
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i + 1]);
                    }

                    graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                    pointY += 70;
                }

                for (int i = j - 1; i > 0; i--)
                {
                    if (i == j - 1)
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[fullSortedList.Count - 2]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                    else
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i + 1]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }

                }
            }
            else
            {
                for (int i = j; i > 0; i--)
                {
                    if (i > 1)
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i - 1]);

                    graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                    pointY += 70;
                }

                for (int i = j + 1; i < fullSortedList.Count - 1; i++)
                {
                    if (i == j + 1)
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[1]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                    else
                    {
                        if (i < fullSortedList.Count - 2)
                            total += Math.Abs(fullSortedList[i] - fullSortedList[i + 1]);

                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                }
            }


            pointY = 300;
            for (int i = 0; i < graphList.Count - 1; i++)
            {
                if (graphList[i + 1].X >= graphList[i].X)
                    hiddenConnectedLines.Add(new DDALine(graphList[i].X, pointY + 15, graphList[i + 1].X - 50, pointY + 15 + 70));      // 70 is the height difference between Points
                else
                    hiddenConnectedLines.Add(new DDALine(graphList[i].X, pointY + 15, graphList[i + 1].X + 50, pointY + 15 + 70));

                pointY += 70;
            }

            label3.Text = "Total head movements incurred while servicing these requests = " + total;
            label3.Location = new Point(920, this.Height - 400);
            label3.Visible = true;
        
        }

        public void cLookAlgorithm()
        {
            int total = 0;
            int j = 0;  // Head Index
            int pointY = 300;   // Points Start at this Y-Point

            for (int i = 0; i < fullSortedList.Count; i++)
            {
                if (fullSortedList[i] == head)
                {
                    j = i;
                    break;
                }
            }

            if (direction == 1)
            {
                for (int i = j; i < fullSortedList.Count - 1; i++)
                {
                    if (i < fullSortedList.Count - 2)
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i + 1]);

                    graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                    pointY += 70;
                }

                for (int i = 1; i < j; i++)
                {
                    if (i == 1)
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[fullSortedList.Count - 2]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                    else
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i - 1]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }

                }
            }
            else
            {
                for (int i = j; i > 0; i--)
                {
                    if (i > 1)
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i - 1]);

                    graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                    pointY += 70;
                }

                for (int i = fullSortedList.Count - 2; i > j; i--)
                {
                    if (i == fullSortedList.Count - 2)
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[1]);
                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                    else
                    {
                        total += Math.Abs(fullSortedList[i] - fullSortedList[i + 1]);

                        graphList.Add(new DrawGraph(printedNumebersList[i], pointY, fullSortedList[i], colorsList[i], new Point(printedNumebersList[i] + 10, pointY + 15)));
                        pointY += 70;
                    }
                }
            }

            //MessageBox.Show("" + graphList.Count());

            pointY = 300;
            for (int i = 0; i < graphList.Count - 1; i++)
            {
                if (graphList[i + 1].X >= graphList[i].X)
                    hiddenConnectedLines.Add(new DDALine(graphList[i].X, pointY + 15, graphList[i + 1].X - 50, pointY + 15 + 70));      // 70 is the height difference between Points
                else
                    hiddenConnectedLines.Add(new DDALine(graphList[i].X, pointY + 15, graphList[i + 1].X + 50, pointY + 15 + 70));

                pointY += 70;
            }

            label3.Text = "Total head movements incurred while servicing these requests = " + total;
            label3.Location = new Point(920, this.Height - 400);
            label3.Visible = true;
        }

        void drawScene(Graphics g)
        {
            g.Clear(Color.White);
            int vX = 120;
            for(int i = 0; i < fullSortedList.Count; i++)
            {
                SolidBrush br = new SolidBrush(colorsList[i]);

                if(i < fullSortedList.Count - 1)
                    g.FillRectangle(Brushes.Black, vX, 200, spaceWidth, 6); // Horizontal Line

                g.FillRectangle(Brushes.Black, vX, 185, 6, verticalLineHeight); // Vertical Line
                g.DrawString(fullSortedList[i] + "", new Font("Arial", 12, FontStyle.Bold), br, vX - 15, 230);
                vX += spaceWidth;
            }

            for (int i = 0; i < animatedLines.Count; i++)
            {
                Pen p = new Pen(graphList[i].color, 5);
                if(i < hiddenConnectedLines.Count)
                    g.DrawLine(p, hiddenConnectedLines[i].xs, hiddenConnectedLines[i].ys, animatedLines[i].X, animatedLines[i].Y);
            }

            for (int i = 0; i < graphList.Count; i++)
            {
                if (!graphList[i].hidden)
                {
                    SolidBrush br = new SolidBrush(graphList[i].color);
                    Pen p = new Pen(graphList[i].color, 5);
                    if(filledCircle == 1)
                        g.FillEllipse(br, graphList[i].X, graphList[i].Y, graphList[i].circleSize, graphList[i].circleSize);
                    else
                        g.DrawEllipse(p, graphList[i].X, graphList[i].Y, graphList[i].circleSize, graphList[i].circleSize);

                }
            }
        }

        void drawDouble(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            drawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}
