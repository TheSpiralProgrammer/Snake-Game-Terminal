#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

static class Program
{
    static void Main(string[] args)
    {
        const int width = 40;
        const int height = 20;
        const int initialSpeed = 200; 
        int speed = initialSpeed;
        int score = 0;
        int lastScore = LoadLastScore();
        bool gameOver = false;

        Console.Clear();
        Console.WriteLine($"Last Score: {lastScore}");
        Console.WriteLine("Press any key to start...");
        Console.ReadKey(true);

        List<Point> snake = new List<Point> { new Point(10, 10) };
        Direction direction = Direction.Right;
        Point food = GenerateFood(width, height, snake);

        while (!gameOver)
        {
            Console.Clear();
            DrawBoard(width, height, snake, food, score);

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                direction = GetNewDirection(direction, key);
            }

            Point head = snake[0];
            Point newHead = new Point(head.X + direction.Dx(), head.Y + direction.Dy());

            if (newHead.X <= 0 || newHead.X >= width - 1 || newHead.Y <= 0 || newHead.Y >= height - 1 || snake.Contains(newHead))
            {
                gameOver = true;
                break;
            }

            snake.Insert(0, newHead);

            if (newHead.Equals(food))
            {
                score++;
                food = GenerateFood(width, height, snake);
                if (score % 5 == 0)
                {
                    speed = Math.Max(50, speed - 20); 
                }
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }

            Thread.Sleep(speed);
        }

        Console.Clear();
        DrawBoard(width, height, snake, food, score);
        Console.SetCursorPosition(width / 2 - 5, height / 2);
        Console.WriteLine("Game Over!");
        Console.SetCursorPosition(width / 2 - 6, height / 2 + 1);
        Console.WriteLine($"Score: {score}");
        SaveLastScore(score);

        Console.SetCursorPosition(0, height + 1);
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey(true);
    }

    static void DrawBoard(int width, int height, List<Point> snake, Point food, int score)
    {
        Console.WriteLine(new string('#', width));

        for (int y = 1; y < height - 1; y++)
        {
            Console.Write('#');
            for (int x = 1; x < width - 1; x++)
            {
                Point p = new Point(x, y);
                if (p.Equals(snake[0]))
                    Console.Write('O'); 
                else if (snake.Contains(p))
                    Console.Write('o'); 
                else if (p.Equals(food))
                    Console.Write('*'); 
                else
                    Console.Write(' ');
            }
            Console.WriteLine('#');
        }

        Console.WriteLine(new string('#', width));

        Console.WriteLine($"Score: {score}");
    }

    static Point GenerateFood(int width, int height, List<Point> snake)
    {
        Random rand = new Random();
        Point food;
        do
        {
            food = new Point(rand.Next(1, width - 1), rand.Next(1, height - 1));
        } while (snake.Contains(food));
        return food;
    }

    static Direction GetNewDirection(Direction current, ConsoleKey key)
    {
        return key switch
        {
            ConsoleKey.UpArrow or ConsoleKey.W => current != Direction.Down ? Direction.Up : current,
            ConsoleKey.DownArrow or ConsoleKey.S => current != Direction.Up ? Direction.Down : current,
            ConsoleKey.LeftArrow or ConsoleKey.A => current != Direction.Right ? Direction.Left : current,
            ConsoleKey.RightArrow or ConsoleKey.D => current != Direction.Left ? Direction.Right : current,
            _ => current
        };
    }

    static int LoadLastScore()
    {
        if (File.Exists("lastscore.txt"))
        {
            return int.Parse(File.ReadAllText("lastscore.txt"));
        }
        return 0;
    }

    static void SaveLastScore(int score)
    {
        File.WriteAllText("lastscore.txt", score.ToString());
    }

    struct Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y) { X = x; Y = y; }

        public override bool Equals(object? obj) => obj is Point p && X == p.X && Y == p.Y;
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }

    enum Direction
    {
        Up = 0, Down, Left, Right
    }

    static int Dx(this Direction d) => d == Direction.Left ? -1 : d == Direction.Right ? 1 : 0;
    static int Dy(this Direction d) => d == Direction.Up ? -1 : d == Direction.Down ? 1 : 0;
}
