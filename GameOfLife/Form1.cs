using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        public Graphics graphics;
        private int resolution;         //введенные параметры клетки
        private bool[,] field;               //массив состояния клеток
        private int rows;                   //параметры массива
        private int cols;
        private int currentGeneration = 0;

        private void StartGame()
        {
            if (timer1.Enabled)             //если таймер включен, нет прорисовки
                return;

            currentGeneration = 0;
            Text = $"Generaion {currentGeneration}";

            nudDensity.Enabled = false;         //блокировка изменения параметров
            nudResolution.Enabled = false;          //с началом игры
            resolution = (int)nudResolution.Value;  //введенные параметры клетки
            rows = pictureBox1.Height / resolution;
            cols = pictureBox1.Width / resolution;

            //создание игрового поля 
            field = new bool[cols, rows];  //информация о клетках игрового поля
            Random random = new Random();
            for(int i=0; i<cols; i++)
            {
                for (int j=0; j<rows;j++)
                {
                    field[i, j] = random.Next((int)nudDensity.Value)==0;
                }
            }

            pictureBox1.Image = new Bitmap(pictureBox1.Width,pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            timer1.Start();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "The Life Game";
        }

        private void NextGeneration()
        {
            graphics.Clear(Color.Black);

            var NewField = new bool[cols,rows];  

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    var neibCount = CountNeighbours(i,j);
                    var isLife = field[i, j];
                    if (!isLife && (neibCount==3))                       NewField[i, j] = true;
                    else if (isLife && (neibCount<2 || neibCount>3))     NewField[i, j] = false;
                    else                                                 NewField[i, j] = field[i, j];

                    if (isLife)
                    {
                        graphics.FillRectangle(Brushes.Red, i * resolution, j * resolution, resolution, resolution);

                    }
                }
            }
            field = NewField;
            pictureBox1.Refresh();
            Text = $"Generaion {++currentGeneration}";
        }

        private int CountNeighbours(int x, int y )
        {
            int count = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int col = (x + i + cols) % cols;
                    int row = (y + j + rows) % rows;
                    bool isSelf = (col == x) && (row == y);
                    if (field[col, row] && !isSelf)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            StartGame();
        }


        private void StopGame()
        {
            if (!timer1.Enabled) return; //если таймер выкл, то, собсна, нечего выключать
            timer1.Stop();
            nudDensity.Enabled = true;         //возвр возможность изменения параметров
            nudResolution.Enabled = true;          
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
                return;

            if (e.Button==MouseButtons.Left)
            {
                var x = e.Location.X/resolution;
                var y = e.Location.Y / resolution;
                if (ValidateMousePosition(x,y))
                field[x, y] = true;
            }
            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                if (ValidateMousePosition(x, y))
                field[x, y] = false;
            }
        }

        private bool ValidateMousePosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < cols && y < rows;
        }
    }
}
