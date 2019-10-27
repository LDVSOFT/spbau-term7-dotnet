using System;

namespace Hw4
{
    internal class Program
    {
        private readonly Game _game;
        private readonly EventLoop _loop;

        private CellState[,] _oldScreen;

        private Program(string levelFileName)
        {
            _game = new Game(levelFileName);
            _loop = new EventLoop();

            _oldScreen = null;
        }

        private void HandleMovement(Movement movement)
        {
            _game.Move(movement);
        }

        private CellState[,] GetNewScreen(
            int baseY, int baseX,
            int displayedHeight, int displayedWidth,
            int displayHeight, int displayWidth
        ) {
            var result = new CellState[displayHeight, displayWidth];
            for (var y = 0; y < displayHeight; ++y)
            for (var x = 0; x < displayWidth; ++x)
            {
                if (y >= displayedHeight || x >= displayedWidth)
                {
                    result[y, x] = CellState.Nothing;
                    continue;
                }
                result[y, x] = _game[baseY + y, baseX + x];
            }
            var cy = _game.CharacterPosition.y - baseY;
            var cx = _game.CharacterPosition.x - baseX;
            result[cy, cx] = CellState.Nothing;
            return result;
        }

        private static void Draw(CellState state)
        {
            switch (state)
            {
                case CellState.Empty:
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write(' ');
                    break;
                case CellState.Wall:
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write('#');
                    break;
                case CellState.Exit:
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write('E');
                    break;
                case CellState.Nothing:
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(' ');
                    break;
            }
        }

        private static void DrawCharacter(bool atExit)
        {
            if (atExit)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            Console.Write('@');
        }

        private void Redraw()
        {
            var displayHeight = Console.WindowHeight - 1;
            var displayWidth = Console.WindowWidth;

            var baseY = _game.CharacterPosition.y - displayHeight / 2;
            var baseX = _game.CharacterPosition.x - displayWidth / 2;
            baseY = Math.Max(0, Math.Min(_game.Height - displayHeight, baseY));
            baseX = Math.Max(0, Math.Min(_game.Width  - displayWidth , baseX));

            var displayedHeight = Math.Min(displayHeight, _game.Height - baseY);
            var displayedWidth  = Math.Min(displayWidth , _game.Width  - baseX);

            if (_oldScreen?.GetLength(0) != displayHeight || _oldScreen?.GetLength(1) != displayWidth)
            {
                _oldScreen = null;
            }

            var newScreen = GetNewScreen(baseY, baseX, displayedHeight, displayedWidth, displayHeight, displayWidth);

            var characterY = _game.CharacterPosition.y - baseY;
            var characterX = _game.CharacterPosition.x - baseX;
            Console.CursorVisible = false;
            for (var y = 0; y < displayedHeight; ++y)
            for (var x = 0; x < displayedWidth; ++x)
            {
                if (_oldScreen?[y, x] == newScreen[y, x])
                {
                    // Fix: if we are close to the character, we might have been overriden by input
                    if (Math.Abs(y - characterY) + Math.Abs(x - characterX + 1) > 3)
                    {
                        continue;
                    }
                }
                Console.SetCursorPosition(x, y);
                Draw(newScreen[y, x]);
            }
            Console.SetCursorPosition(_game.CharacterPosition.x - baseX, _game.CharacterPosition.y - baseY);
            DrawCharacter(_game[_game.CharacterPosition] == CellState.Exit);

            _oldScreen = newScreen;
        }

        private void HandleInput(object sender, EventLoop.InputEventArgs args)
        {
            switch (args.Event)
            {
                case EventLoop.InputEvent.Left:
                    HandleMovement(Movement.Left);
                    break;
                case EventLoop.InputEvent.Right:
                    HandleMovement(Movement.Right);
                    break;
                case EventLoop.InputEvent.Down:
                    HandleMovement(Movement.Down);
                    break;
                case EventLoop.InputEvent.Up:
                    HandleMovement(Movement.Up);
                    break;
                case EventLoop.InputEvent.Exit:
                    _loop.Stop();
                    break;
            }
        }

        private void HandleTick()
        {
            Redraw();
            if (_game.IsOver)
                _loop.Stop();
        }

        private void Run()
        {
            var fg = Console.ForegroundColor;
            var bg = Console.BackgroundColor;
            Console.Clear();

            _loop.InputHandler += HandleInput;
            _loop.TickHandler += HandleTick;

            Redraw();
            _loop.Run();

            _loop.TickHandler -= HandleTick;
            _loop.InputHandler -= HandleInput;

            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            if (_game.IsOver)
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
