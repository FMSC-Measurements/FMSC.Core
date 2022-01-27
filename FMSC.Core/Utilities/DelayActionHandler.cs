using System;
using System.Timers;

namespace FMSC.Core.Utilities
{
    public class DelayActionHandler : IDisposable
    {
        private Timer _Timer;
        public long Delay { get; private set; } = 1000;
        private Action _Action;
        public bool Disposed { get; private set; }


        public DelayActionHandler()
        {
            _Timer = new Timer(Delay);
            _Timer.AutoReset = false;
            _Timer.Elapsed += Timer_Elapsed;
        }

        public DelayActionHandler(long delay) : this()
        {
            Delay = delay;
            _Timer.Interval = Delay;
        }

        public DelayActionHandler(Action action, long delay) : this(delay)
        {
            _Action = action;
        }
        

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _Action?.Invoke();
        }
        

        public void DelayInvoke()
        {
            DelayInvoke(_Action, Delay);
        }

        public void DelayInvoke(Action action)
        {
            DelayInvoke(action, Delay);
        }

        public void DelayInvoke(long delay)
        {
            DelayInvoke(_Action, delay);
        }

        public void DelayInvoke(Action action, long delay)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DelayActionHandler));

            _Timer.Stop();
            _Action = action;

            _Timer.Interval = delay;

            _Timer.Start();
        }


        public void Cancel()
        {
            _Timer.Stop();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                Disposed = true;

                Cancel();
                _Timer.Elapsed -= Timer_Elapsed;
            }
        }
    }
}
