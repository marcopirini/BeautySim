using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BeautySim2023
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MultipleChoiceControl : UserControl
    {
        public static readonly DependencyProperty AnswersProperty =
            DependencyProperty.Register("Answers", typeof(IEnumerable<string>), typeof(MultipleChoiceControl));

        public static readonly DependencyProperty CorrectAnswersProperty =
           DependencyProperty.Register("CorrectAnswers", typeof(IEnumerable<string>), typeof(MultipleChoiceControl));

        public static readonly DependencyProperty FontSizeAnswersProperty =
           DependencyProperty.Register("FontSizeAnswers", typeof(double), typeof(MultipleChoiceControl));

        public static readonly DependencyProperty FontSizeQuestionProperty =
           DependencyProperty.Register("FontSizeQuestion", typeof(double), typeof(MultipleChoiceControl));

        public static readonly DependencyProperty QuestionProperty =
                           DependencyProperty.Register("Question", typeof(string), typeof(MultipleChoiceControl));

        public MultipleChoiceControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void FillControls(bool imTeacher)
        {
            for (int i = 0; i < Answers.Count(); i++)
            {
                CustomBorder c = new CustomBorder();
                c.BorderBrush = Brushes.Black;
                c.BorderThickness = new Thickness(2);
                c.CornerRadius = new CornerRadius(5);
                TextBlock tb = new TextBlock();
                tb.TextAlignment = TextAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.Text = Answers.ToList()[i];
                c.Width = imTeacher ? 40 : 60;
                c.Child = tb;
                tb.Background = Brushes.Transparent;
                c.Background = Brushes.Transparent;
                c.Margin = new Thickness(5);
                c.Width = imTeacher ? 110 : 160;
                c.Height = HeightBorders;
                spAnswers.Children.Add(c);
                c.PreviewMouseDown += C_PreviewMouseDown;
            }
        }

        private void C_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (((CustomBorder)sender).IsEnabled)
            {
                if (((CustomBorder)sender).YourIntegerProperty == 0)
                {
                    ((CustomBorder)sender).YourIntegerProperty = 1;
                }
                else if (((CustomBorder)sender).YourIntegerProperty == 1)
                {
                    ((CustomBorder)sender).YourIntegerProperty = 0;
                }
            }
            if (((CustomBorder)sender).YourIntegerProperty == 1)
            {
                for (int i = 0; i < spAnswers.Children.Count; i++)
                {
                    if ((CustomBorder)sender != spAnswers.Children[i])
                    {
                        ((CustomBorder)spAnswers.Children[i]).YourIntegerProperty = 0;
                    }
                }
            }
            UpdateBackgrounds();

            int buttonIndex = GetCustomBorderIndex((CustomBorder)sender);
            OnSomethingChanged(buttonIndex, ((CustomBorder)sender).YourIntegerProperty);
        }

        public void UpdateBackgrounds()
        {
            for (int i = 0; i < spAnswers.Children.Count; i++)
            {
                CustomBorder r = (CustomBorder)spAnswers.Children[i];
                r.UpdateBackground();
            }
        }

        public delegate void SomethingChangedDelegate(MultipleChoiceControl aa, int index, int state);

        public event SomethingChangedDelegate SomethingChangedEvent;

        public IEnumerable<string> Answers
        {
            get { return (IEnumerable<string>)GetValue(AnswersProperty); }
            set { SetValue(AnswersProperty, value); }
        }

        public IEnumerable<string> CorrectAnswers
        {
            get { return (IEnumerable<string>)GetValue(CorrectAnswersProperty); }
            set { SetValue(CorrectAnswersProperty, value); }
        }

        public double FontSizeAnswers
        {
            get { return (double)GetValue(FontSizeAnswersProperty); }
            set { SetValue(FontSizeAnswersProperty, value); }
        }

        public double FontSizeQuestion
        {
            get { return (double)GetValue(FontSizeQuestionProperty); }
            set { SetValue(FontSizeQuestionProperty, value); }
        }

        public string Question
        {
            get { return (string)GetValue(QuestionProperty); }
            set { SetValue(QuestionProperty, value); }
        }

        public int HeightBorders { get; private set; }

        internal void SetSameMultipleChoiceOption(int index, int state)
        {
            CustomBorder tb = (CustomBorder)GetCustomBorderFromIndex(spAnswers, index);
            tb.YourIntegerProperty = state;

            if (tb.YourIntegerProperty == 1)
            {
                for (int i = 0; i < spAnswers.Children.Count; i++)
                {
                    if (tb != spAnswers.Children[i])
                    {
                        ((CustomBorder)spAnswers.Children[i]).YourIntegerProperty = 0;
                    }
                }
            }
            UpdateBackgrounds();
        }

        public virtual void OnSomethingChanged(int buttonIndex, int state)
        {
            SomethingChangedEvent?.Invoke(this, buttonIndex, state);
        }

        private int GetCustomBorderIndex(CustomBorder toggleButton)
        {
            for (int i = 0; i < spAnswers.Children.Count; i++)
            {
                if (toggleButton == spAnswers.Children[i])
                {
                    return i;
                }
            }

            return -1;
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    return typedChild;
                }
                var result = FindVisualChild<T>(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private object GetCustomBorderFromIndex(StackPanel itemsControl, int buttonIndex)
        {
            if (itemsControl != null && buttonIndex >= 0 && buttonIndex < itemsControl.Children.Count)
            {
                return (CustomBorder)itemsControl.Children[buttonIndex];
            }

            return null;
        }

        internal void Configure(string v)
        {
            if (v=="Vertical")
            {
                Grid.SetColumnSpan(spAnswers, 2);
                Grid.SetColumnSpan(spQuestion, 2);
                Grid.SetRow(spAnswers, 1);
                spAnswers.HorizontalAlignment = HorizontalAlignment.Center;
                spQuestion.HorizontalAlignment = HorizontalAlignment.Center;
                HeightBorders = 100;
            }

            if (v=="Horizontal")
            {
                Grid.SetColumnSpan(spAnswers, 1);
                Grid.SetColumnSpan(spQuestion, 1);
                Grid.SetColumn(spAnswers, 1);
                Grid.SetRow(spAnswers, 0);
                spAnswers.HorizontalAlignment = HorizontalAlignment.Right;
                spQuestion.HorizontalAlignment = HorizontalAlignment.Left;
                grMain.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Star);
                HeightBorders = 60;
            }
        }
    }
}