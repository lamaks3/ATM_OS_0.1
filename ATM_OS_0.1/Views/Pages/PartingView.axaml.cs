using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ATM_OS
{
    public partial class PartingView : UserControl
    {
        public PartingView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}