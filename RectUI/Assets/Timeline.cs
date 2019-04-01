using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;


namespace RectUI.Assets
{
    public class Timeline
    {
        public bool Loop = true;

        TimeSpan _duration;
        public TimeSpan Duration
        {
            get { return _duration; }
            set
            {
                if (_duration == value) return;
                _duration = value;
            }
        }

        Subject<TimeSpan> _timeSubject = new Subject<TimeSpan>();
        public IObservable<TimeSpan> TimeObservable
        {
            get { return _timeSubject.AsObservable(); }
        }

        Task _timer;
        CancellationTokenSource _cts;

        async Task Timer(CancellationToken ct)
        {
            _timeSubject.OnNext(TimeSpan.Zero);
            var sw = Stopwatch.StartNew();
            while (true)
            {
                ct.ThrowIfCancellationRequested();

                // 30 FPS
                await Task.Delay(1000 / 30);

                var elapsed = sw.Elapsed;
                if (elapsed >= Duration) {
                    _timeSubject.OnNext(Duration);
                    if (Loop)
                    {
                        _timeSubject.OnNext(TimeSpan.Zero);
                        sw.Restart();
                    }
                    else { 
                        break;
                    }
                }
                else
                {
                    _timeSubject.OnNext(elapsed);
                }
            }
        }

        IDisposable _subscription;

        public void Start(Action<TimeSpan> callback)
        {
            Stop();

            _cts = new CancellationTokenSource();
            _timer = Timer(_cts.Token);
            _subscription = TimeObservable.Subscribe(callback);
        }

        public void Stop()
        {
            if (_subscription != null)
            {
                _subscription.Dispose();
                _subscription = null;
            }

            if (_timer != null)
            {
                _cts.Cancel();
                _cts = null;
                _timer = null;
            }
        }
    }
}
