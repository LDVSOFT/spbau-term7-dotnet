using System;

namespace Hw4
{
    public class EventLoop
    {
        public enum InputEvent
        {
            Left,
            Right,
            Down,
            Up,
            Exit
        }

        public class InputEventArgs : EventArgs
        {
            public InputEventArgs(InputEvent aEvent)
            {
                Event = aEvent;
            }

            public InputEvent Event { get; }
        }

        public event EventHandler<InputEventArgs> InputHandler;
        public event Action TickHandler;

        private bool _work = true;

        public void Run()
        {
            Console.TreatControlCAsInput = true;
            while (true)
            {
                TickHandler?.Invoke();
                if (!_work)
                {
                    break;
                }

                while (!Console.KeyAvailable)
                {
                }
                var keyInfo = Console.ReadKey();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        InputHandler?.Invoke(this, new InputEventArgs(InputEvent.Left));
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        InputHandler?.Invoke(this, new InputEventArgs(InputEvent.Right));
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        InputHandler?.Invoke(this, new InputEventArgs(InputEvent.Down));
                        break;
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        InputHandler?.Invoke(this, new InputEventArgs(InputEvent.Up));
                        break;
                    case ConsoleKey.C:
                        if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                        {
                            InputHandler?.Invoke(this, new InputEventArgs(InputEvent.Exit));
                        }
                        break;
                    case ConsoleKey.Escape:
                        InputHandler?.Invoke(this, new InputEventArgs(InputEvent.Exit));
                        break;
                }
            }
        }

        public void Stop()
        {
            _work = false;
        }
    }
}