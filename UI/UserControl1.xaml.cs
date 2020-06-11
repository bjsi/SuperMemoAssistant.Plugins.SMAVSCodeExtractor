using Highlight;
using Highlight.Engines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SMACodeExtracts.UI
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1
    {
        public string Comment { get; set; }
        public double Priority { get; set; }
        public bool Confirmed { get; set; } = false;

        public UserControl1(string selectedText, ProjectInfo info)
        {
            if (string.IsNullOrEmpty(selectedText))
                return;

            InitializeComponent();
            FileInfo.Text = $"File: \"{Path.GetFileName(info.file)}\" Project: \"{info.project}\" Language: {info.language}";
            PrioritySlider.Value = 30;
            var highlighter = new Highlighter(new HtmlEngine());
            var html = highlighter.Highlight("C#", selectedText);
            // Preserve Whitespace
            html = $"<pre>{html}</pre>";
            SelectedCodeBrowser.NavigateToString(html);
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            Priority = PrioritySlider.Value;
            Comment = ExtractComment.Text;
            Confirmed = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PrioritySlider_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.PageDown)
            {
                PrioritySlider.Value += 10;
                e.Handled = true;
            }
            else if (e.Key == Key.PageUp)
            {
                PrioritySlider.Value -= 10;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
