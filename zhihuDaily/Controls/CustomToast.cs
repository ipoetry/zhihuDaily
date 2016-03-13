using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace zhihuDaily.Controls
{
    public class CustomToast
    {
        #region Filed
        Grid toastGrid; //消息 Grid
        Panel popUpParentPanel; //消息在此Panel上弹出
        AppShell RootVisualFrame;  //当前页面的可视根 //原来Frame
        TranslateTransform toastTransform;  //动画
        Duration dua = TimeSpan.FromMilliseconds(600);
        Timer timer = null;
        int seconds = 3;
        bool bLocked;
        bool bInit;
        #endregion

        #region Events

        public event EventHandler<object> ToastShowComplated;
        public event EventHandler<object> ToastHiddenCompleted;

        #endregion

        #region Properties

        public string Title { get; set; }
        public string Message { get; set; }
        public double TitleFontSize { get; set; }
        public double MessageFontSize { get; set; }
        public Orientation TextOrientation { get; set; }

        public bool IsShow { get; private set; }    //只读
        public int TotalHiddenSeconds
        {
            get { return this.seconds; }
            set
            {
                if (seconds < 0)
                {
                    return;
                }
                else
                {
                    this.seconds = value;
                }
            }
        }
        public Panel PopUpParentPanel
        {
            get
            {
                if (popUpParentPanel == null)
                {
                    IEnumerable<ContentPresenter> source = VisualHelper.GetVisualDescendants(this.RootVisualFrame.ToastPanel).OfType<ContentPresenter>();
                    int aaa = source.Count<ContentPresenter>();

                    for (int i = 0; i < source.Count<ContentPresenter>(); i++)
                    {
                        IEnumerable<Panel> enumerable2 = VisualHelper.GetVisualDescendants(source.ElementAt<ContentPresenter>(i)).OfType<Panel>();
                        if (enumerable2.Count<Panel>() > 0)
                        {
                            this.popUpParentPanel = enumerable2.First<Panel>();
                            break;
                        }
                    }
                }
                return this.popUpParentPanel;
            }
        }
        #endregion

        #region Construction
        //构造函数的继承  先执行后面的  后执行前面的   代码复用

        public CustomToast()
            : this(string.Empty, string.Empty)
        {

        }

        public CustomToast(string message)
            : this(string.Empty, message)
        {

        }
        public CustomToast(string title, string message)
        {
            this.Title = title;
            this.Message = message;

            TitleFontSize = MessageFontSize = 16;
            TextOrientation = Orientation.Horizontal;

        }

        #endregion

        #region Public Method

        public void Show()
        {
            if (bLocked)
            {
                return;
            }

            InitControl();

            if (PopUpParentPanel != null)
            {
                if (PopUpParentPanel is StackPanel)
                {
                    PopUpParentPanel.Children.Insert(0, toastGrid);
                }
                else
                {
                    PopUpParentPanel.Children.Add(toastGrid);
                }
                ShowStoryboard();
                bLocked = true;
            }
        }

        #endregion

        #region InitControl

        public void InitControl()
        {
            if (bInit)
            {
                return;
            }
            bInit = true;
            RootVisualFrame = Window.Current.Content as AppShell;

            //   RootVisualFrame = Window.Current.Content as AppShell;
            Brush brush = new SolidColorBrush(Model.AppSettings.themeLightColor);//(Brush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"];
            toastGrid = new Grid() {Margin=new Thickness(0,40,0,0), Height = 40, Width = Window.Current.Bounds.Width / 2 + 40, Background = brush, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Right };
            //toastGrid.ManipulationDelta += toastGrid_ManipulationDelta;
            //toastGrid.ManipulationCompleted += toastGrid_ManipulationCompleted;
            toastTransform = new TranslateTransform();
            toastGrid.RenderTransform = toastTransform;

            StackPanel spContent = new StackPanel()
            {
                // Margin = new Thickness(10, 5, 0, 0),
                Margin = new Thickness(6, 0, 0, 0), 
                Orientation = this.TextOrientation,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            TextBlock txtTitle = new TextBlock() { FontSize = TitleFontSize, Text = Title, VerticalAlignment = VerticalAlignment.Center };
            TextBlock txtMessage = new TextBlock() { FontSize = MessageFontSize, Text = Message, VerticalAlignment = VerticalAlignment.Center, Foreground = new SolidColorBrush(Colors.White) };

            if (spContent.Orientation == Orientation.Horizontal)
            {
                txtMessage.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                toastGrid.Height = 66;
            }

            spContent.Children.Add(txtTitle);
            spContent.Children.Add(txtMessage);
            toastGrid.Children.Add(spContent);
        }
        #endregion

        #region ToastGrid Manipulation

        //void toastGrid_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        //{
        //    toastTransform.X = this.toastTransform.X + e.Delta.Translation.X;
        //    if (toastTransform.X < 0)
        //    {
        //        toastTransform.X = 0;
        //    }
        //}

        //void toastGrid_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        //{
        //    if (e.Cumulative.Translation.X < 60)
        //    {
        //        toastTransform.X = 0;
        //    }
        //    else
        //    {
        //        HideStoryboard(toastTransform.X);
        //    }
        //}

        #endregion

        #region Storyboard & Timer

        public void ShowStoryboard()
        {
            Storyboard sbShow = new Storyboard();
            sbShow.Completed += sbShow_Completed;

            ///X轴方向从消息界面一半处开始动画
            DoubleAnimation da = new DoubleAnimation() { From = toastGrid.Width / 2, To = 0, Duration = dua };
            da.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(da, toastTransform);
            Storyboard.SetTargetProperty(da, "TranslateTransform.X");
            sbShow.Children.Add(da);

            //设置透明度从0变为1
            DoubleAnimation da1 = new DoubleAnimation() { From = 0, To = 1, Duration = dua };
            Storyboard.SetTarget(da1, toastGrid);
            Storyboard.SetTargetProperty(da1, "FrameworkElement.Opacity");
            sbShow.Children.Add(da1);

            sbShow.Begin();
        }

        void sbShow_Completed(object sender, object e)
        {
            IsShow = true;

            timer = new Timer(new TimerCallback(Time_Completed), null, this.TotalHiddenSeconds * 1000, 0);
            if (this.ToastShowComplated != null)
            {
                ToastShowComplated(this, e);
            }
        }

        async void Time_Completed(object e)
        {
            timer.Dispose();
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                HideStoryboard();
            });
        }
        private void HideStoryboard(double From = 0)
        {
            Storyboard sbHide = new Storyboard();
            sbHide.Completed += sbHide_Completed;
            DoubleAnimation da = new DoubleAnimation() { From = From, To = toastGrid.Width / 2, Duration = dua };
            Storyboard.SetTarget(da, toastTransform);
            Storyboard.SetTargetProperty(da, "TranslateTransform.X");
            sbHide.Children.Add(da);

            DoubleAnimation da1 = new DoubleAnimation() { From = 1, To = 0, Duration = dua };
            Storyboard.SetTarget(da1, toastGrid);
            Storyboard.SetTargetProperty(da1, "FrameworkElement.Opacity");
            sbHide.Children.Add(da1);

            sbHide.Begin();
        }

        void sbHide_Completed(object sender, object e)
        {
            IsShow = false;
            bLocked = false;

            if (toastGrid.Parent != null && toastGrid.Parent is Windows.UI.Xaml.Controls.Panel)
            {
                (toastGrid.Parent as Windows.UI.Xaml.Controls.Panel).Children.Remove(toastGrid);
            }

            if (ToastHiddenCompleted != null)
            {
                ToastHiddenCompleted(this, e);
            }
        }
        #endregion
    }

    public class VisualHelper
    {
        public static IEnumerable<DependencyObject> GetVisualChildren(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return GetVisualChildrenAndSelfIterator(element).Skip<DependencyObject>(1);
        }

        public static IEnumerable<DependencyObject> GetVisualDescendants(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return GetVisualDescendantsAndSelfIterator(element).Skip<DependencyObject>(1);
        }

        private static IEnumerable<DependencyObject> GetVisualDescendantsAndSelfIterator(DependencyObject element)
        {
            Queue<DependencyObject> iteratorVariable0 = new Queue<DependencyObject>();
            iteratorVariable0.Enqueue(element);
            while (true)
            {
                if (iteratorVariable0.Count <= 0)
                {
                    yield break;
                }
                DependencyObject iteratorVariable1 = iteratorVariable0.Dequeue();
                yield return iteratorVariable1;
                foreach (DependencyObject obj2 in GetVisualChildren(iteratorVariable1))
                {
                    iteratorVariable0.Enqueue(obj2);
                }
            }
        }

        private static IEnumerable<DependencyObject> GetVisualChildrenAndSelfIterator(DependencyObject element)
        {
            yield return element;
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);
            int childIndex = 0;
            while (true)
            {
                if (childIndex >= childrenCount)
                {
                    yield break;
                }
                yield return VisualTreeHelper.GetChild(element, childIndex);
                childIndex++;
            }
        }
    }
}
