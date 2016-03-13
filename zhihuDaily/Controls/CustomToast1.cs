using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace zhihuDaily.Controls
{
    public class CustomToast1
    {
        public event EventHandler Click;
        public event EventHandler Completed;
        public  void Show(string message)
        {
            try
            {
                Popup p = new Popup();
                ToastBox tb = new ToastBox() { Message = message };
                p.Child = tb;
                p.IsOpen = true;
                p.HorizontalAlignment = HorizontalAlignment.Center;
                p.VerticalAlignment = VerticalAlignment.Bottom;
                p.Margin = new Thickness();
                tb.OpenAnimation.Begin();
                DispatcherTimer timer = new DispatcherTimer();
                tb.OpenAnimation.Completed += new EventHandler<object>((sender, eventargs) =>
                {
                    try
                    {
                        timer.Interval = TimeSpan.FromSeconds(3);
                        timer.Tick += new EventHandler<object>((sd, ea) =>
                        {
                            try
                            {
                                if (timer != null && timer.IsEnabled)
                                {
                                    timer.Stop();
                                    tb.CloseAnimation.Begin();
                                    tb.CloseAnimation.Completed += new EventHandler<object>((s, e) =>
                                    {
                                        try
                                        {
                                            p.IsOpen = false;
                                            if (Completed != null)
                                                Completed.Invoke(this, new EventArgs());
                                        }
                                        catch { }
                                    });
                                }
                            }
                            catch { }
                        });
                        timer.Start();
                    }
                    catch { }
                });
                //tb.Tap += new EventHandler<GestureEventArgs>((sender, eventargs) =>
                //{
                //    try
                //    {
                //        if (Click != null)
                //            Click.Invoke(this, new EventArgs());
                //    }
                //    catch { }
                //});
                tb.ManipulationCompleted += new ManipulationCompletedEventHandler((sender, eventargs) =>
                {
                    try
                    {
                        if (eventargs.Cumulative.Translation.X > 200 || eventargs.Velocities.Linear.X > 1000)
                        {
                            if (timer != null && timer.IsEnabled)
                            {
                                timer.Stop();
                                tb.CloseAnimation.Begin();
                                tb.CloseAnimation.Completed += new EventHandler<object>((sd, ea) =>
                                {
                                    try
                                    {
                                        p.IsOpen = false;
                                        if (Completed != null)
                                            Completed.Invoke(this, new EventArgs());
                                    }
                                    catch { }
                                });
                            }
                        }
                    }
                    catch { }
                });
            }
            catch { }
        }
    }
}
