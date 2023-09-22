using System;
using System.Windows;
using System.Windows.Controls;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MainPage.xaml
    /// </summary>
    public partial class MessageFrameS : Page
    {
        public MessageFrameS()
        {
            InitializeComponent();
        }


        public MessageFrameS(string message, string header, Enum_MessageType typeMessage, bool ImTeacher) : this()
        {
            tbHeader.Text = header;
            string[] lineParsed = message.Split(new string[1] { "\\n" }, StringSplitOptions.None);
            tbMessage.Text = "";
            foreach (string line in lineParsed)
            {
                string line2 = line.Replace('&', '\u2022');
                tbMessage.Text = tbMessage.Text + line2 + "\r\n";
            }
            switch (typeMessage)
            {
                case Enum_MessageType.CASE_DESCRIPTION:
                    tbMessage.FontSize = ImTeacher ? 20 : 30;
                    tbMessage.TextAlignment = TextAlignment.Justify;
                    break;
                case Enum_MessageType.SCORE:
                    tbMessage.FontSize = ImTeacher ? 40 : 60;
                    tbMessage.TextAlignment = TextAlignment.Justify;
                    break;
                case Enum_MessageType.OTHER_INFORMATION:
                    tbMessage.FontSize = ImTeacher ? 40 : 60;
                    tbMessage.TextAlignment = TextAlignment.Justify;
                    break;
                default:
                    break;
            }
            if (ImTeacher)
            {
                grMain.RowDefinitions[0].Height = new GridLength(100, GridUnitType.Pixel);
                tbHeader.FontSize = 40;
                tbHeader.Margin = new Thickness(20);
                scMessage.Margin = new Thickness(40);
            }

            //tbMessage.Text = message;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {

            AppControl.Instance.AdvancePartialAdvanceStep();
        }
    }
}
