using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleSnakeGame
{
    enum Direction { Up, Down, Left, Right }

    class Program
    {
        // Oyun alanı boyutları
        const int boardWidth = 40;
        const int boardHeight = 20;
        // Oyun hızı (milisaniye)
        const int gameSpeed = 100;

        static List<Position> snake = new List<Position>();
        static Position food;
        static Direction direction = Direction.Right;
        static Random rnd = new Random();
        static bool gameOver = false;
        static int score = 0;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            InitializeGame();
            while (!gameOver)
            {
                Input();
                Logic();
                Draw();
                Thread.Sleep(gameSpeed);
            }
            Console.SetCursorPosition(0, boardHeight + 2);
            Console.WriteLine($"Oyun bitti! Skor: {score}");
            Console.CursorVisible = true;
            Console.ReadKey();
        }

        // Oyunu başlat: yılanın başlangıç konumunu ve ilk yiyeceği ayarla.
        static void InitializeGame()
        {
            snake.Clear();
            // Yılanın orta noktada başlaması
            snake.Add(new Position { X = boardWidth / 2, Y = boardHeight / 2 });
            direction = Direction.Right;
            score = 0;
            SpawnFood();
        }

        // Rastgele yiyecek oluştur
        static void SpawnFood()
        {
            // Yiyeceğin yılanın üzerine denk gelmemesine dikkat etmek için döngü
            do
            {
                food = new Position { X = rnd.Next(1, boardWidth - 1), Y = rnd.Next(1, boardHeight - 1) };
            }
            while (snake.Exists(p => p.X == food.X && p.Y == food.Y));
        }

        // Klavye girişlerini oku
        static void Input()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (direction != Direction.Down)
                            direction = Direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                        if (direction != Direction.Up)
                            direction = Direction.Down;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (direction != Direction.Right)
                            direction = Direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                        if (direction != Direction.Left)
                            direction = Direction.Right;
                        break;
                }
            }
        }

        // Oyunun mantığını kontrol et (yılan hareketi, çarpışma, yiyecek yeme)
        static void Logic()
        {
            // Yılanın yeni baş pozisyonunu hesapla.
            Position head = new Position { X = snake[0].X, Y = snake[0].Y };
            switch (direction)
            {
                case Direction.Right:
                    head.X++;
                    break;
                case Direction.Left:
                    head.X--;
                    break;
                case Direction.Up:
                    head.Y--;
                    break;
                case Direction.Down:
                    head.Y++;
                    break;
            }

            // Duvara çarpma durumu (sınırlar dahilinde)
            if (head.X < 0 || head.X >= boardWidth || head.Y < 0 || head.Y >= boardHeight)
            {
                gameOver = true;
                return;
            }

            // Kendi kendine çarpma kontrolü
            if (snake.Exists(p => p.X == head.X && p.Y == head.Y))
            {
                gameOver = true;
                return;
            }

            // Yılanın kuyruğunu takip et
            snake.Insert(0, head);

            // Yiyeceği yedi mi?
            if (head.X == food.X && head.Y == food.Y)
            {
                score++;
                SpawnFood();
            }
            else
            {
                // Yılan yiyecek yemediyse kuyruğu sil
                snake.RemoveAt(snake.Count - 1);
            }
        }

        // Oyun alanını çiz
        static void Draw()
        {
            Console.SetCursorPosition(0, 0);
            // Üst sınır
            Console.WriteLine(new string('#', boardWidth));
            // Orta alan
            for (int y = 1; y < boardHeight - 1; y++)
            {
                Console.Write("#");  // Sol kenar
                for (int x = 1; x < boardWidth - 1; x++)
                {
                    if (x == food.X && y == food.Y)
                        Console.Write("O");  // Yiyecek
                    else if (snake.Exists(p => p.X == x && p.Y == y))
                        Console.Write("X");  // Yılan
                    else
                        Console.Write(" ");
                }
                Console.WriteLine("#"); // Sağ kenar
            }
            // Alt sınır
            Console.WriteLine(new string('#', boardWidth));
            // Skor bilgisi
            Console.WriteLine($"Skor: {score}");
        }
    }

    // Yılan ve yiyecek için pozisyon tutan sınıf
    class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
