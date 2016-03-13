using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zhihuDaily
{
    public sealed class TaskCompletionNotifier<TResult> : INotifyPropertyChanged
    {
        private TResult _result;
        private IAsyncResult _task;

        public TaskCompletionNotifier()
        {
            this._result = default(TResult);
        }

        public void SetTask<T>(Task<T> task, Func<T, TResult> factoryFunc)
        {
            this._task = task;
            if (!task.IsCompleted)
            {
                var scheduler = (SynchronizationContext.Current == null)
                    ? TaskScheduler.Current
                    : TaskScheduler.FromCurrentSynchronizationContext();
                task.ContinueWith(t =>
                {
                    var propertyChanged = PropertyChanged;
                    if (propertyChanged != null)
                    {
                        this.OnPropertyChanged("IsCompleted");
                        if (t.IsFaulted)
                        {
                            InnerException = t.Exception;
                            this.OnPropertyChanged("ErrorMessage");
                        }
                        else
                        {
                            try
                            {
                                this._result = factoryFunc(task.Result);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine("Factory error: " + ex.Message);
                                this.InnerException = ex;
                                this.OnPropertyChanged("ErrorMessage");
                            }
                            this.OnPropertyChanged("Result");
                        }
                    }
                },
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    scheduler);
            }
            else
            {
                this._result = factoryFunc(task.Result);
            }
        }

        public void SetTask(Task<TResult> task)
        {
            this._task = task;
            if (!task.IsCompleted)
            {
                var scheduler = (SynchronizationContext.Current == null)
                    ? TaskScheduler.Current
                    : TaskScheduler.FromCurrentSynchronizationContext();
                task.ContinueWith(t =>
                {
                    var propertyChanged = PropertyChanged;
                    if (propertyChanged != null)
                    {
                        this.OnPropertyChanged("IsCompleted");
                        if (t.IsFaulted)
                        {
                            InnerException = t.Exception;
                            this.OnPropertyChanged("ErrorMessage");
                        }
                        else
                        {
                            this._result = task.Result;
                            this.OnPropertyChanged("Result");
                        }
                    }
                },
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    scheduler);
            }
            else
            {
                this._result = task.Result;
            }
        }

        public TResult Result
        {
            get { return this._result; }
        }

        public bool IsCompleted
        {
            get { return _task.IsCompleted; }
        }

        public Exception InnerException { get; set; }

        public string ErrorMessage
        {
            get { return (InnerException == null) ? null : InnerException.Message; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
