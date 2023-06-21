using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Numerics;
using System.IO;

namespace VKR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //Функция быстрого возведения в степень по модулю
        public static BigInteger powmod(BigInteger a, BigInteger b, BigInteger n)
        {
            if (b == 0)
                return 1;
            BigInteger c = powmod(a, b / 2, n);
            if (b % 2 == 0)
            {
                return (c * c) % n;
            }
            else
            {
                return (a * c * c) % n;
            }
        }
        //Функция генерации всех чисел между двумя числа определенного количества бит
        public static List<int> GenerateAllNumbers(int bitCount1, int bitCount2)
        {
            int minNumber = (int)Math.Pow(2, bitCount1 - 1);
            int maxNumber = (int)Math.Pow(2, bitCount2) - 1;
            List<int> nums = new List<int>();
            for (int i = minNumber; i <= maxNumber; i++)
            {
                nums.Add(i);
            }
            return nums;
        }
        //Функция для получения эталонных простых чисел из интервала
        public static List<int> Prime_numbers(int num1, int num2)
        {
            string filePath = "E:/educ/дипломушка/кодик/primes.txt";
            string content = File.ReadAllText(filePath);
            string[] nnumbers = content.Replace("[", "").Replace("]", "").Split(',');
            List<int> array_of_all_numbers = new List<int>();

            for (int i = 0; i < nnumbers.Length; i++)
            {
                if (int.TryParse(nnumbers[i].Trim(), out int value))
                {
                    if (value >= num1 && value <= num2)
                    { array_of_all_numbers.Add(value); }
                }
            }
            return array_of_all_numbers;
        }
        
        //Функция поиска наибольшего общего делителя
        public static BigInteger GCD(BigInteger x, BigInteger y)
        {
            return y == 0 ? x : GCD(y, x % y);
        }
        //Функция нахождения символа Якоби
        public static int Jacobi(BigInteger a, BigInteger b)
        {
            if (GCD(a, b) != 1) { return 0; }
            int r = 1;
            if (a < 0)
                a = -a;
            if (b % 4 == 3) { r = -r; }
            while (a != 0)
            {
                int t = 0;
                while (a % 2 == 0)
                {
                    t++;
                    a /= 2;
                }
                if (t % 2 != 0)
                {
                    if (b % 8 == 3 || b % 8 == 5) { r = -r; }
                }
                if (a % 4 == 3 && b % 4 == 3) { r = -r; }
                BigInteger c = a;
                a = b % c;
                b = c;
            }
            return r;
        }
        //вспомогательная функция для нахождения символа Якоби
        private static int Jacobi_Exp(BigInteger n)
        {
            if (n % 2 == 0)
                return 1;
            return -1;
        }
        //Функция нахождения символа Лежандра
        private static int Legendre_symbol(BigInteger a, BigInteger n)
        {
            if (a == 0)
                return 0;
            if (a < 0)
                return Legendre_symbol(a *= 1, n) * Jacobi_Exp((n - 1) / 2);
            if (a % 2 == 0)
                return Legendre_symbol(a / 2, n) * Jacobi_Exp((BigInteger.Pow(n, 2) - 1) / 8);
            if (a == 1)
                return 1;
            if (a < n)
                return Legendre_symbol(n, a) * Jacobi_Exp((a - 1) / 2 * (n - 1) / 2);
            return Legendre_symbol(a % n, n);
        }
        // Функция произведения матриц типа BigInteger
        private static BigInteger[,] MatrixMultiply(BigInteger[,] a, BigInteger[,] b)
        {
            BigInteger[,] r = new BigInteger[a.GetLength(0), b.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    for (int k = 0; k < b.GetLength(0); k++)
                    {
                        r[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return r;
        }

        //Вычисление n-ого члена Фибоначчи
        private static BigInteger Fibonacci(BigInteger n)
        {
            if (n == 0)
                return 0;
            else if (n == 1 || n == 2)
                return 1;
            else
            {
                BigInteger[,] a = new BigInteger[,] { { 1, 1 }, { 1, 0 } };
                BigInteger[,] b = new BigInteger[,] { { 1, 1 }, { 1, 0 } };
                for (int i = 1; i < n; i++)
                {
                    a = MatrixMultiply(a, b);
                }
                return a[0, 1];
            }
        }
        //Функция теста Миллера-Рабина
        public static bool Miller_Rabin(BigInteger number, int iterations)
        {
            BigInteger t = number - 1;
            int s = 0;
            while (t % 2 == 0)
            {
                t /= 2;
                s += 1;
            }
            for (int j = 0; j < iterations; j++)
            {
                
                RNGCryptoServiceProvider rngCSP = new RNGCryptoServiceProvider();
                byte[] c = new byte[number.ToByteArray().LongLength];
                BigInteger a;
                do
                {
                    rngCSP.GetBytes(c);
                    a = new BigInteger(c);
                } while (a < 2 || a > number - 2);
                
                /* версия для реализации определенной базы алгоритма:
                List<BigInteger> a_numbers = new List<BigInteger>() { 2,3,5,7,11,13,17,19,23,29};
                BigInteger a = a_numbers[j];
                */
                BigInteger x = powmod(a, t, number);
                if (x == 1 || x == number - 1) { continue; }
                for (int i = 0; i < s-1; i++)
                {
                    x = powmod(x, 2, number);
                    if (x == 1) { return false; }
                    if (x == number - 1) { break; }

                }
                if (x != number - 1)
                    return false;
            }
            return true;
        }
        //Функция теста Лукаса-Миллера-Рабина
        public static bool Lucas_Miller_Rabin(BigInteger number)
        {
            if (number > 5)
            {
                int e = Jacobi(number, 5);
                BigInteger t = number - e;
                BigInteger s = 0;
                while (t % 2 == 0)
                {
                    t /= 2;
                    s += 1;
                }
                BigInteger Ft = Fibonacci(t);
                if (Ft % number == 0) return true;
                for (int i = 0; i < s; i++)
                {
                    BigInteger m = BigInteger.Pow(2, i) * t;
                    if ((Fibonacci(m - 1) + Fibonacci(m + 1)) % number == 0) return true;
                }
                return false;
            }
            else
            {
                if (number == 2 || number == 3 || number == 5)
                    return true;
                else return false;
            }
        }
        //Функция теста Соловея-Штрассена
        public static bool Solov_Shtrass(BigInteger number, int iterations)
        {
            if (number <= 2)
                return false;
            int i = 0;
            while (i < iterations)
            {
                /* версия для реализации определенной базы алгоритма:
                List<BigInteger> a_numbers = new List<BigInteger>() { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };
                BigInteger a = a_numbers[i];
                */

                RNGCryptoServiceProvider rngCSP = new RNGCryptoServiceProvider();
                byte[] c = new byte[number.ToByteArray().LongLength];
                BigInteger a;
                do
                {
                    rngCSP.GetBytes(c);
                    a = new BigInteger(c);
                } while (a < 2 || a > number - 2);
                
                if (GCD(a, number) > 1) { return false; }
                if (powmod(a, (number - 1) / 2, number) != Legendre_symbol(a, number) && powmod(a, (number - 1) / 2, number) != Legendre_symbol(a, number) + number)
                { return false; }
                i += 1;
            }
            return true;
        }
        //Функция теста Леманна
        public static bool Lemann(BigInteger number, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                /* версия для реализации определенной базы алгоритма:
                List<BigInteger> a_numbers = new List<BigInteger>() { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };
                BigInteger a = a_numbers[i];
                */

                RNGCryptoServiceProvider rngCSP = new RNGCryptoServiceProvider();
                byte[] c = new byte[number.ToByteArray().LongLength];
                BigInteger a;
                do
                {
                    rngCSP.GetBytes(c);
                    a = new BigInteger(c);
                } while (a < 2 || a > number - 1);
                
                if (GCD(a, number) == 1)
                {
                    BigInteger k = (number - 1) / 2;
                    BigInteger r = powmod(a, k, number);
                    if (r != 1 && r != number - 1) { return false; }
                }
            }
            return true;
        }
        //Функция для кнопки, выполняющая поиск простых чисел алгоритмом Миллера-Рабина
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "" || textBox2.Text == "")
                {
                    MessageBox.Show("Не введены значения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    int num1 = int.Parse(textBox1.Text);
                    int num2 = int.Parse(textBox2.Text);
                    bool flag = false;
                    if (num1 >= num2) { MessageBox.Show("Конец интервала не больше начала!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    else
                    {
                        if (num1 < 4) { MessageBox.Show("Введено слишком маленькое значение для начала интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        else
                        {
                            if (num1 > 20) { MessageBox.Show("Введено слишком большое значение для начала интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                            else
                            {
                                if (num2 < 5) { MessageBox.Show("Введено слишком маленькое значение для конца интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                                else
                                {
                                    if (num2 > 30) { MessageBox.Show("Введено слишком большое значение для конца интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                                    else
                                    {
                                        flag = true;
                                    }
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        List<int> numbers = GenerateAllNumbers(num1, num2); //все числа интервала
                        List<int> true_prime = Prime_numbers((int)numbers[0], (int)numbers[numbers.Count - 1]); //эталонные простые
                        List<int> arr = new List<int>(); //массив для найденных простых чисел
                        List<int> arrmis1 = new List<int>(); //массив для чисел, которые алгоритм посчитал простыми, а они такими не явл
                        List<int> arrmis2 = new List<int>(); //массив для чисел, которые алгоритм пропустил
                        int itter = int.Parse(textBox12.Text);

                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();
                        for (int i = 0; i < numbers.Count; i++)
                        {
                            if (numbers[i] % 2 != 0 && Miller_Rabin(numbers[i], itter))
                            {
                                arr.Add(numbers[i]);
                            }
                        }
                        stopWatch.Stop();
                        
                        IEnumerable<int> c = true_prime.Except(arr);
                        arrmis1 = new List<int>(c);
                        IEnumerable<int> c1 = arr.Except(true_prime);
                        arrmis2 = new List<int>(c1);
                        
                        textBox3.Text = "Всего найдено " + arr.Count + " простых чисел. Среди них " + arrmis1.Count + " ошибочно отнесены к простым:";
                        foreach (int item in arrmis1)
                        {
                            textBox3.Text += " " + item;
                        }
                        textBox3.Text += ". Не найдено " + arrmis2.Count + " простых чисел:";
                        foreach (int item in arrmis2)
                        {
                            textBox3.Text += " " + item;
                        }
                        textBox3.Text += ". Найденные простые числа: ";
                        foreach (int item in arr)
                        {
                            textBox3.Text += " " + item;
                        }
                        
                        TimeSpan ts = stopWatch.Elapsed;
                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts.Hours, ts.Minutes, ts.Seconds,
                            ts.Milliseconds / 10);
                        textBox4.Text = elapsedTime;
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Введите значения правильно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (OverflowException)
            {
                MessageBox.Show("Введены слишком большие значения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Функция для кнопки, выполняющая поиск простых чисел алгоритмом Леманна
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "" || textBox2.Text == "")
                {
                    MessageBox.Show("Не введены значения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    int num1 = int.Parse(textBox1.Text);
                    int num2 = int.Parse(textBox2.Text);
                    bool flag = false;
                    if (num1 >= num2) { MessageBox.Show("Конец интервала не больше начала!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    else
                    {
                        if (num1 < 4) { MessageBox.Show("Введено слишком маленькое значение для начала интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        else
                        {
                            if (num1 > 20) { MessageBox.Show("Введено слишком большое значение для начала интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                            else
                            {
                                if (num2 < 5) { MessageBox.Show("Введено слишком маленькое значение для конца интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                                else
                                {
                                    if (num2 > 30) { MessageBox.Show("Введено слишком большое значение для конца интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                                    else
                                    {
                                        flag = true;
                                    }
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        List<int> numbers = GenerateAllNumbers(num1, num2); //все числа интервала
                        List<int> true_prime = Prime_numbers((int)numbers[0], (int)numbers[numbers.Count - 1]); //эталонные простые
                        List<int> arr = new List<int>(); //массив для найденных простых чисел
                        List<int> arrmis1 = new List<int>(); //массив для чисел, которые алгоритм посчитал простыми, а они такими не явл
                        List<int> arrmis2 = new List<int>(); //массив для чисел, которые алгоритм пропустил
                        int itter = int.Parse(textBox14.Text);

                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();
                        for (int i = 0; i < numbers.Count; i++)
                        {
                            if (numbers[i] % 2 != 0 && Lemann(numbers[i], itter))
                            {
                                arr.Add(numbers[i]);
                            }
                        }
                        stopWatch.Stop();
                        
                        IEnumerable<int> c = true_prime.Except(arr);
                        arrmis1 = new List<int>(c);
                        IEnumerable<int> c1 = arr.Except(true_prime);
                        arrmis2 = new List<int>(c1);
                        
                        textBox7.Text = "Всего найдено " + arr.Count + " простых чисел. Среди них " + arrmis1.Count + " ошибочно отнесены к простым:";
                        foreach (int item in arrmis1)
                        {
                            textBox7.Text += " " + item;
                        }
                        textBox7.Text += ". Не найдено " + arrmis2.Count + " простых чисел:";
                        foreach (int item in arrmis2)
                        {
                            textBox7.Text += " " + item;
                        }
                        textBox7.Text += ". Найденные простые числа: ";
                        foreach (int item in arr)
                        {
                            textBox7.Text += " " + item;
                        }
                        
                        TimeSpan ts = stopWatch.Elapsed;
                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts.Hours, ts.Minutes, ts.Seconds,
                            ts.Milliseconds / 10);
                        textBox8.Text = elapsedTime;
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Введите значения правильно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (OverflowException)
            {
                MessageBox.Show("Введены слишком большие значения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Функция для кнопки, выполняющая поиск простых чисел алгоритмом Соловея-Штрассена
        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "" || textBox2.Text == "")
                {
                    MessageBox.Show("Не введены значения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    int num1 = int.Parse(textBox1.Text);
                    int num2 = int.Parse(textBox2.Text);
                    bool flag = false;
                    if (num1 >= num2) { MessageBox.Show("Конец интервала не больше начала!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    else
                    {
                        if (num1 < 4) { MessageBox.Show("Введено слишком маленькое значение для начала интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        else
                        {
                            if (num1 > 20) { MessageBox.Show("Введено слишком большое значение для начала интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                            else
                            {
                                if (num2 < 5) { MessageBox.Show("Введено слишком маленькое значение для конца интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                                else
                                {
                                    if (num2 > 30) { MessageBox.Show("Введено слишком большое значение для конца интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                                    else
                                    {
                                        flag = true;
                                    }
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        List<int> numbers = GenerateAllNumbers(num1, num2); //все числа интервала
                        List<int> true_prime = Prime_numbers((int)numbers[0], (int)numbers[numbers.Count - 1]); //эталонные простые
                        List<int> arr = new List<int>(); //массив для найденных простых чисел
                        List<int> arrmis1 = new List<int>(); //массив для чисел, которые алгоритм посчитал простыми, а они такими не явл
                        List<int> arrmis2 = new List<int>(); //массив для чисел, которые алгоритм пропустил
                        int itter = int.Parse(textBox13.Text);

                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();
                        for (int i = 0; i < numbers.Count; i++)
                        {
                            if (numbers[i] % 2 != 0 && Solov_Shtrass(numbers[i], itter))
                            {
                                arr.Add(numbers[i]);
                            }
                        }
                        stopWatch.Stop();
                        
                        IEnumerable<int> c = true_prime.Except(arr);
                        arrmis1 = new List<int>(c);
                        IEnumerable<int> c1 = arr.Except(true_prime);
                        arrmis2 = new List<int>(c1);
                        
                        textBox5.Text = "Всего найдено " + arr.Count + " простых чисел. Среди них " + arrmis1.Count + " ошибочно отнесены к простым:";
                        foreach (int item in arrmis1)
                        {
                            textBox5.Text += " " + item;
                        }
                        textBox5.Text += ". Не найдено " + arrmis2.Count + " простых чисел:";
                        foreach (int item in arrmis2)
                        {
                            textBox5.Text += " " + item;
                        }
                        textBox5.Text += ". Найденные простые числа: ";
                        foreach (int item in arr)
                        {
                            textBox5.Text += " " + item;
                        }
                        
                        TimeSpan ts = stopWatch.Elapsed;
                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts.Hours, ts.Minutes, ts.Seconds,
                            ts.Milliseconds / 10);
                        textBox6.Text = elapsedTime;
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Введите значения правильно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (OverflowException)
            {
                MessageBox.Show("Введены слишком большие значения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Функция для кнопки, выполняющая поиск простых чисел алгоритмом Лукаса-Миллера-Рабина
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "" || textBox2.Text == "")
                {
                    MessageBox.Show("Не введены значения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    int num1 = int.Parse(textBox1.Text);
                    int num2 = int.Parse(textBox2.Text);
                    bool flag = false;
                    if (num1 >= num2) { MessageBox.Show("Конец интервала не больше начала!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    else {
                        if (num1 < 4) { MessageBox.Show("Введено слишком маленькое значение для начала интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        else
                        {
                            if (num1 > 20) { MessageBox.Show("Введено слишком большое значение для начала интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                            else
                            {
                                if (num2 < 5) { MessageBox.Show("Введено слишком маленькое значение для конца интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                                else
                                {
                                    if (num2 > 30) { MessageBox.Show("Введено слишком большое значение для конца интервала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                                    else
                                    {
                                        flag = true;
                                    }
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        List<int> numbers = GenerateAllNumbers(num1, num2); //все числа интервала
                        List<int> true_prime = Prime_numbers((int)numbers[0], (int)numbers[numbers.Count-1]); //эталонные простые
                        List<int> arr = new List<int>(); //массив для найденных простых чисел
                        List<int> arrmis1 = new List<int>(); //массив для чисел, которые алгоритм посчитал простыми, а они такими не явл
                        List<int> arrmis2 = new List<int>(); //массив для чисел, которые алгоритм пропустил
                        
                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();
                        for (int i = 0; i < numbers.Count; i++)
                        {
                            if (numbers[i] % 2 != 0 && numbers[i] % 5 != 0 && Lucas_Miller_Rabin(numbers[i]))
                            {
                                arr.Add(numbers[i]);
                            }
                        }
                        stopWatch.Stop();
                        
                        IEnumerable<int> c = true_prime.Except(arr);
                        arrmis1 = new List<int>(c);
                        IEnumerable<int> c1 = arr.Except(true_prime);
                        arrmis2 = new List<int>(c1);
                        
                        textBox9.Text = "Всего найдено " + arr.Count + " простых чисел. Среди них " + arrmis1.Count + " ошибочно отнесены к простым:";
                        foreach(int item in arrmis1)
                        {
                            textBox9.Text += " " + item;
                        }
                        textBox9.Text += ". Не найдено " + arrmis2.Count + " простых чисел:";
                        foreach (int item in arrmis2)
                        {
                            textBox9.Text += " " + item;
                        }
                        textBox9.Text += ". Найденные простые числа: ";
                        foreach (int item in arr)
                        {
                            textBox9.Text += " " + item;
                        }
                        
                        TimeSpan ts = stopWatch.Elapsed;
                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts.Hours, ts.Minutes, ts.Seconds,
                            ts.Milliseconds / 10);
                        textBox10.Text = elapsedTime;
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Введите значения правильно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (OverflowException)
            {
                MessageBox.Show("Введены слишком большие значения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Функция для кнопки, выполяняющая проверку числа каждым алгоритмом
        private void button5_Click(object sender, EventArgs e)
        {
            Stopwatch stopWatch = new Stopwatch();
            BigInteger number = int.Parse(textBox11.Text);

            int it1 = int.Parse(textBox12.Text);
            int it2 = int.Parse(textBox13.Text);
            int it3 = int.Parse(textBox14.Text);

            stopWatch.Start();
            if (Lucas_Miller_Rabin(number) && number % 5 != 0)
            { textBox9.Text = "PRIME"; }
            else { textBox9.Text = "NOT PRIME"; }
            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            textBox10.Text = elapsedTime;

            Stopwatch stopWatch1 = new Stopwatch();
            stopWatch1.Start();
            if (Miller_Rabin(number, it1) && number % 5 != 0)
            { textBox3.Text = "PRIME"; }
            else { textBox3.Text = "NOT PRIME"; }
            stopWatch1.Stop();

            TimeSpan ts1 = stopWatch1.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts1.Hours, ts1.Minutes, ts1.Seconds,
                ts1.Milliseconds / 10);
            textBox4.Text = elapsedTime;

            Stopwatch stopWatch2 = new Stopwatch();
            stopWatch2.Start();
            if (Lemann(number, it3) && number % 5 != 0)
            { textBox7.Text = "PRIME"; }
            else { textBox7.Text = "NOT PRIME"; }
            stopWatch2.Stop();

            TimeSpan ts2 = stopWatch2.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts2.Hours, ts2.Minutes, ts2.Seconds,
                ts2.Milliseconds / 10);
            textBox8.Text = elapsedTime;

            Stopwatch stopWatch3 = new Stopwatch();
            stopWatch3.Start();
            if (Solov_Shtrass(number, it2) && number % 5 != 0)
            { textBox5.Text = "PRIME"; }
            else { textBox5.Text = "NOT PRIME"; }
            stopWatch3.Stop();

            TimeSpan ts3 = stopWatch3.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts3.Hours, ts3.Minutes, ts3.Seconds,
                ts3.Milliseconds / 10);
            textBox6.Text = elapsedTime;
        }
    }
}
