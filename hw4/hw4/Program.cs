using System;

namespace hw4
{
    internal class Program
    {
        private Game _game;
        private EventLoop _loop;

        private Program(string levelFileName)
        {
            _game = new Game(levelFileName);
            _loop = new EventLoop();
            _loop.LeftHandler  += (sender, args) => HandleMovement(Movement.Left);
            _loop.RightHandler += (sender, args) => HandleMovement(Movement.Right);
            _loop.DownHandler  += (sender, args) => HandleMovement(Movement.Down);
            _loop.UpHandler    += (sender, args) => HandleMovement(Movement.Up);
            _loop.TickHandler  += (sender, args) => Redraw();
        }

        private void HandleMovement(Movement movement)
        {
            _game.Move(movement);
            if (_game.IsOver)
                _loop.Stop();            
        }

        private void Redraw()
        {
            var displayHeight = Console.WindowHeight;
            var displayWidth = Console.WindowWidth;

            var baseY = _game.CharacterPosition.y - displayHeight / 2;
            var baseX = _game.CharacterPosition.y - displayHeight / 2;
            baseY = Math.Max(0, Math.Min(_game.Height - displayHeight, baseY));
            baseX = Math.Max(0, Math.Min(_game.Width  - displayWidth , baseX));

            var displayedHeight = Math.Min(displayHeight, _game.Height - baseY);
            var displayedWidth  = Math.Min(displayWidth , _game.Width  - baseX);

            Console.CursorVisible = false;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            for (var y = 0; y < displayedHeight; ++y)
            for (var x = 0; x < displayedWidth; ++x)
            {
                Console.SetCursorPosition(x, y);
                var gamePosition = (y + baseY, x + baseX);
                switch (_game[gamePosition])
                {
                    case CellState.Empty:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(' ');
                        break;
                    case CellState.Wall:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write('#');
                        break;
                    case CellState.Exit:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write('E');
                        break;
                }
            }
            Console.SetCursorPosition(_game.CharacterPosition.x - baseX, _game.CharacterPosition.y - baseY);
            switch (_game[_game.CharacterPosition])
            {
                case CellState.Empty:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case CellState.Exit:
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    break;
            }
            Console.Write('@');
        }

        private void Run()
        {
            var fg = Console.ForegroundColor;
            var bg = Console.BackgroundColor;
            Console.Clear();

            Redraw();
            _loop.Run();

            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
            Console.Clear();
            Console.WriteLine("You won!");
        }

        public static void Main(string[] args)
        {
            Console.WriteLine(args);
            if (args.Length != 1)
            {
                Console.WriteLine("Wrong arguments");
                Environment.Exit(1);
            }
            try
            {
                new Program(args[0]).Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
