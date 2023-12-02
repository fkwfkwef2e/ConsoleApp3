using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {

        Graphics g; // графическая поверхность        
        List<Rectangle> snake = new List<Rectangle>(); // змейка        
        public enum Course { UP, DOWN, LEFT, RIGHT };
        public int lengthSnake; // длина змейки
        public bool existsFood; // существование еды на поле
        Rectangle food = new Rectangle(); // поле с едой

        List<Rectangle> empty_filed = new List<Rectangle>(); // список незанятых ячеек
        public Random rnd = new Random();

        public Course course; // направление движения     

        public int addX, addY; // смещение координат

        public Form1()
        {
            InitializeComponent();
            course = Course.LEFT; // начальное направление
            // добавляем в змейку три начальных сегмента
            snake.Add(new Rectangle(200, 0, 20, 20));
            snake.Add(new Rectangle(220, 0, 20, 20));
            snake.Add(new Rectangle(240, 0, 20, 20));

            timer1.Interval = 200; // устанавливаем таймер            
            lengthSnake = 3; // начальная длина змейки            
            existsFood = false; // еды нет

            g = this.CreateGraphics();// создаем графичскую поверхность

            DrawSnake();  // рисуем змейку             

            DrawFood(); // рисуем еду

            timer1.Enabled = true;  // запускаем таймер         

        } // Form1        

        private void Form1_TickTimer(object sender, EventArgs e)
        {
            Refresh(); // очищаем экран

            timer1.Enabled = false; // останавливаем таймер

            // в зависимости от выбранного направления
            // наращиваем координаты головы (snake[0])
            if (course == Course.UP)
            {
                addX = 0;
                addY = -20;
            }

            if (course == Course.DOWN)
            {
                addX = 0;
                addY = 20;
            }

            if (course == Course.LEFT)
            {
                addX = -20;
                addY = 0;
            }

            if (course == Course.RIGHT)
            {
                addX = 20;
                addY = 0;
            }

            Rectangle prev_segment;
            Rectangle next_segment;

            prev_segment = snake[0]; // запоминаем значение старой головы
                                     // чтобы присвоить его след. сегменту
                                     // по циклу присваиваем значение предыдущего сегмента следующему
            for (int i = 0; i < snake.Count - 1; i++)
            {

                if (i == 0)
                {

                    snake[i] = new Rectangle(
                        snake[i].X + addX,
                        snake[i].Y + addY,
                        20, 20);
                }
                if (!(snake[i + 1].IsEmpty))
                {
                    next_segment = snake[i + 1];
                    snake[i + 1] = prev_segment;
                    prev_segment = next_segment;
                }
            }
            // если голова "съела" еду
            if (snake[0] == food)
            {
                snake.Add(food); // добавляем сегмент с коор-ами еды
                                 // этот сегмент пройдет через всю змейку
                                 // и "прицепиться" в конце
                lengthSnake++; // увеличиваем длину змейки
                existsFood = false; // еды нет
                // увеличиваем скорость через каждые 7 сегментов змейки
                if ((lengthSnake % 7 == 0) &&
                    (timer1.Interval > 50))
                {
                    timer1.Interval -= 30;
                }
            }
            // Проверка на проигрыш
            if (// выход за границу игрового поля
                (snake[0].X < 0 || snake[0].X > 240 ||
                snake[0].Y < 0 || snake[0].Y > 240)
                ||
                // проверка на самосъедение
                EatMySelf())
            {
                MessageBox.Show("Игра закончена\n Длина змейки равна: " + lengthSnake.ToString());
                this.Close();
                return;
            }

            DrawSnake();  // рисуем змейку 
            DrawFood(); // рисуем еду             

            timer1.Enabled = true; // запускаем таймер

        } // Form1_TickTimer

        // метод меняет направление, в зависимости от нажатой стрелки
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && course != Course.DOWN)
            {
                course = Course.UP;
            }
            if (e.KeyCode == Keys.Down && course != Course.UP)
            {
                course = Course.DOWN;
            }
            if (e.KeyCode == Keys.Left && course != Course.RIGHT)
            {
                course = Course.LEFT;
            }
            if (e.KeyCode == Keys.Right && course != Course.LEFT)
            {
                course = Course.RIGHT;
            }

        } // Form1_KeyDown

        private void DrawSnake()
        {
            for (int i = 0; i < snake.Count; i++)
            {
                if (i == 0) // рисуем голову 
                    g.FillRectangle(Brushes.Green, snake[i]);
                else // рисуем оставшиеся сегменты
                    g.FillRectangle(Brushes.Black, snake[i]);
                g.DrawRectangle(Pens.Green, snake[i]);
            }
        } //DrawSnake

        private void DrawFood()
        {
            // если еда съедена - русуем новую
            if (!existsFood)
            {
                for (int i = 0; i < 13; i++)
                {
                    for (int j = 0; j < 13; j++)
                    {
                        Rectangle temp = new Rectangle(i * 20, j * 20, 20, 20);
                        // заполняем список свободных сегментов игрового поля
                        if (snake.IndexOf(temp) == -1)
                        {
                            empty_filed.Add(temp);
                        }
                    }
                }
                // находим случайным образом свободную ячейку
                food = empty_filed[rnd.Next(0, empty_filed.Count - 1)];
                empty_filed.Clear(); // очищаем список свободных сегментов игрового поля
                g.FillRectangle(Brushes.Red, food); // рисуем еду
                existsFood = true; //еда есть 
            }
            else
            {   // иначе рисуем старую еду
                g.FillRectangle(Brushes.Red, food);
            }
        }
        // проверка на самосъедение
        private bool EatMySelf()
        {
            int count = 0; // количество сегментов равных голове
            foreach (Rectangle t in snake)
            {
                if (t == snake[0]) count++;
            }
            // еда совпадает с координатами головы (это не считаем за самосъедение)
            if (count > 1 && food != snake[0])
                return true;
            else
                return false;
        } // EatMySelf            
    }
}