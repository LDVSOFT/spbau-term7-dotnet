using System;
using System.Threading;

namespace hw4
{
    public class EventLoop
    {
        public event EventHandler<EventArgs> LeftHandler;
        public event EventHandler<EventArgs> RightHandler;
        public event EventHandler<EventArgs> DownHandler;
        public event EventHandler<EventArgs> UpHandler;
        public event EventHandler<EventArgs> TickHandler;

        private bool _work = true;

        public void Run()
        {
            while (_work)
            {
                OnEvent(ref TickHandler);
                var keyInfo = Console.ReadKey();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        OnEvent(ref LeftHandler);
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        OnEvent(ref RightHandler);
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        OnEvent(ref DownHandler);
                        break;
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        OnEvent(ref UpHandler);
                        break;
                }
            }
        }

        public void Stop()
        {
            _work = false;
        }

        private void OnEvent(ref EventHandler<EventArgs> handler)
        {
            Volatile.Read(ref handler)?.Invoke(this, EventArgs.Empty);
        }
    }
}