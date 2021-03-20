using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace CityDistance
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            chart1.ChartAreas[0].AxisX.Interval = 0.1;
        }
        string file;
        Bitmap myBitmap;
        double[,] matr;
        int[,] znak;
        Bitmap secondBitMap;

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {

                openFileDialog1 = new OpenFileDialog
                {
                    InitialDirectory = @"C:\",
                    Title = "Browse Text Files",

                    CheckFileExists = true,
                    CheckPathExists = true,

                    DefaultExt = "bmp",
                    Filter = "bmp files (*.bmp)|*.bmp",
                    FilterIndex = 2,
                    RestoreDirectory = true,

                    ReadOnlyChecked = true,
                    ShowReadOnly = true
                };

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1.Rows.Clear();

                    pictureBox2.Hide();
                    pictureBox1.Show();
                    dataGridView1.Show();
                    file = openFileDialog1.FileName;
                    myBitmap = new Bitmap(file);

                    pictureBox1.Width = myBitmap.Width + 120;
                    pictureBox1.Height = myBitmap.Height + 120;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;


                    pictureBox1.Image = myBitmap;

                    var bmpWidth = myBitmap.Width;
                    var bmpHeight = myBitmap.Height;
                    int[,] pixels = new int[bmpWidth, bmpHeight];

                    dataGridView1.Columns.Clear();
                    for (int i = 0; i < bmpWidth; i++)
                    {
                        dataGridView1.Columns.Add(i + 1 + "", i + 1 + "");

                    }

                    dataGridView1.Rows.Add(bmpWidth - 1);

                    for (int i = 0; i < bmpWidth; i++)
                    {
                        DataGridViewRow row2 = dataGridView1.Rows[i];
                        dataGridView1.Columns[i].Width = 50;
                    }

                    znak = new int[bmpWidth, bmpHeight];
                    for (int i = 0; i < bmpWidth; i++)
                    {
                        for (int j = 0; j < bmpHeight; j++)
                        {
                            Color a = myBitmap.GetPixel(j, i);
                            if (a.R == 255 && a.G == 255 && a.B == 255)
                            {
                                pixels[i, j] = 0;
                                znak[i, j] = 0;
                            }
                            else
                            {
                                pixels[i, j] = 1;
                                znak[i, j] = 1;
                            }
                            dataGridView1[i, j].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dataGridView1[j, i].Value = pixels[i, j];

                        }

                    }
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            button2.Enabled = true;
            button5.Enabled = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button3.Enabled = true;
            button4.Enabled = true;




            for (int i = 0; i < myBitmap.Width; i++)
            {
                DataGridViewRow row2 = dataGridView1.Rows[i];
                dataGridView1.Columns[i].Width = 50;
            }
            dataGridView1.ScrollBars = ScrollBars.None;


            var res = GetCalculateEwclidDistance(GetBitArray());

            int k = 0;

            for (int i = 0; i < myBitmap.Width; i++)
            {
                for (int j = 0; j < myBitmap.Height; j++)
                {
                    if (znak[i, j] == 0)
                    {
                        dataGridView1[j, i].Value = res[k] * -1;

                    }
                    else
                    {
                        dataGridView1[j, i].Value = res[k];
                    }
                    k++;
                }
            }
        }
        public static Bitmap GrauwertBild(Bitmap input)
        {
            Bitmap greyscale = new Bitmap(input.Width, input.Height);

            for (int x = 0; x < input.Width; x++)
            {
                for (int y = 0; y < input.Height; y++)
                {
                    Color pixelColor = input.GetPixel(x, y);
                    if (pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0)
                    {
                        greyscale.SetPixel(x, y, Color.FromArgb(pixelColor.A, 255, 255, 255));
                    }
                    else
                    {
                        greyscale.SetPixel(x, y, Color.FromArgb(pixelColor.A, 0, 0, 0));
                    }
                }
            }
            return greyscale;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            button6.Enabled = true;
            pictureBox2.Show();
            chart1.Series[0].Points.Clear();
            matr = new double[myBitmap.Width, myBitmap.Height];
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    matr[i, j] = double.Parse(dataGridView1.Rows[i].Cells[j].Value.ToString());
                }
            }


            int Count = 0;     ///Коллво цветов в изобр

            Random random = new Random();


            Dictionary<double, double> rezul = new Dictionary<double, double>(); //считаем эл-ты в массиве
            for (int i = 0; i < matr.GetLength(0); i++)
            {
                for (int j = 0; j < matr.GetLength(1); j++)
                {
                    if (rezul.ContainsKey(matr[i, j]))
                    {
                        rezul[matr[i, j]]++;
                    }
                    else
                    {
                        rezul.Add(matr[i, j], 1); Count++;
                    }
                }

            }

            double[] difColor;
            double[] countColor;
            difColor = new double[Count];
            countColor = new double[Count];
            int l = 0;

            foreach (KeyValuePair<double, double> par in rezul)
            {

                chart1.Series[0].Points.AddXY(par.Key, par.Value);
                chart1.Series[0].IsValueShownAsLabel = true;
                difColor[l] = par.Key;
                countColor[l] = par.Value;
                l++;

            }

            Array.Sort(difColor);
            Array.Reverse(difColor);

            int indexForColor = 255 / Count;
            int Position;


            secondBitMap = new Bitmap(myBitmap.Width, myBitmap.Height);

            for (int i = 0; i < secondBitMap.Width; i++)
            {
                for (int j = 0; j < secondBitMap.Height; j++)
                {

                    for (int ii = 0; ii < difColor.GetLength(0); ii++)
                    {
                        if (matr[i, j] == difColor[ii])
                        {
                            Position = ii + 1;
                            Color newColor = Color.FromArgb(Position * indexForColor, Position * indexForColor, Position * indexForColor);
                            secondBitMap.SetPixel(j, i, newColor);

                        }


                    }

                }
            }

            pictureBox2.Image = secondBitMap;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Width = secondBitMap.Width + 120;
            pictureBox2.Height = secondBitMap.Height + 120;
        }
        private double[,] GetBitArray()
        {
            var bmpWidth = myBitmap.Width;
            var bmpHeight = myBitmap.Height;
            int[,] pixels = new int[bmpWidth, bmpHeight];
            double[,] res = new double[bmpWidth, bmpHeight];

            for (int i = 0; i < bmpWidth; i++)
            {
                for (int j = 0; j < bmpHeight; j++)
                {
                    Color a = myBitmap.GetPixel(j, i);
                    if (a.R == 255 && a.G == 255 && a.B == 255)
                    {
                        pixels[i, j] = 0;
                    }
                    else
                    {
                        pixels[i, j] = 1;
                    }

                    res[i, j] = pixels[i, j];
                }
            }

            return res;
        }
        public List<double> GetCalculateEwclidDistance(double[,] pointss)
        {
            Dictionary<string, List<string>> indexes = new Dictionary<string, List<string>>();
            for (int i = 0; i < myBitmap.Width; i++)
            {
                for (int j = 0; j < myBitmap.Height; j++)
                {
                    if (pointss[i, j] == 0)
                    {
                        indexes.Add(i + "," + j, GetIndex(1, pointss));
                    }
                    if (pointss[i, j] == 1)
                    {
                        indexes.Add(i + "," + j, GetIndex(0, pointss));
                    }
                }
            }

            List<double> result = new List<double>();
            foreach (var item in indexes)
            {
                var itemArray = item.Key.Split(',').ToArray();
                var itemValues = item.Value.Select(x => x.Split(',').ToArray()).ToArray();
                List<double> res = new List<double>();

                for (int i = 0; i < itemValues.Length; i++)
                {
                    var a = itemValues[i][0];
                    var b = itemValues[i][1];
                    var c = itemArray[0];
                    var d = itemArray[1];

                    res.Add(Distance(double.Parse(itemArray[0]), double.Parse(itemArray[1]), double.Parse(itemValues[i][0]), double.Parse(itemValues[i][1])));
                }

                result.Add(res.Min());
            }

            return result;
        }

        int globalIForOne = 0;
        int globalJForOne = 0;
        int globalIForZero = 0;
        int globalJForZero = 0;

        private List<string> GetIndex(int number, double[,] pointss)
        {
            List<string> result = new List<string>();
            if (number == 1)
            {
                for (int i = 0; i < myBitmap.Width; i++)
                {
                    for (int j = 0; j < myBitmap.Height; j++)
                    {
                        if (pointss[i, j] == 1 && i >= globalIForOne && j >= globalJForOne)
                        {
                            globalIForOne = i;
                            globalJForOne = j;
                            result.Add(i + "," + j);
                        }
                    }
                    globalJForOne = 0;
                }
            }
            if (number == 0)
            {
                for (int i = 0; i < myBitmap.Width; i++)
                {
                    for (int j = 0; j < myBitmap.Height; j++)
                    {
                        if (pointss[i, j] == 0 && i >= globalIForZero && j >= globalJForZero)
                        {
                            globalIForZero = i;
                            globalJForZero = j;
                            result.Add(i + "," + j);
                        }
                    }
                    globalJForZero = 0;
                }
            }


            globalIForOne = 0;
            globalJForOne = 0;
            globalIForZero = 0;
            globalJForZero = 0;
            return result;
        }


        public double Distance(double x1, double y1, double x2, double y2) => Math.Round(Math.Abs(x1 - x2) + Math.Abs(y1 - y2));

        private void button4_Click(object sender, EventArgs e)
        {
            var changebleBitmap = new Bitmap(myBitmap.Width, myBitmap.Height);
            for (int x = 0; x < changebleBitmap.Width; x++)
            {
                for (int y = 0; y < changebleBitmap.Height; y++)
                {
                    if (int.Parse(dataGridView1[x, y].Value.ToString()) == 1)
                    {
                        changebleBitmap.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        changebleBitmap.SetPixel(x, y, Color.White);
                    }
                }
            }

            pictureBox1.Image = changebleBitmap;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Bitmap a2 = new Bitmap(pictureBox2.Image);
            dataGridView1.ScrollBars = ScrollBars.Horizontal;
            for (int i = 0; i < a2.Width; i++)
            {
                DataGridViewRow row2 = dataGridView1.Rows[i];
                dataGridView1.Columns[i].Width = 120;
            }
            dataGridView1.ScrollBars = ScrollBars.Horizontal;
            for (int i = 0; i < a2.Width; i++)
            {
                for (int j = 0; j < a2.Height; j++)
                {
                    Color a = a2.GetPixel(j, i);
                    dataGridView1[j, i].Value = a.R + "-" + a.G + "-" + a.B;

                }

            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save("test.bmp", ImageFormat.Bmp);
        }
        private void button7_Click(object sender, EventArgs e)
        {
            pictureBox2.Image.Save("test2.bmp", ImageFormat.Bmp);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
