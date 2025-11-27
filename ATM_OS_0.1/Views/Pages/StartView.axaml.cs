using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ATM_OS
{
    public partial class StartView : UserControl
    {
        public event Action ExitAppRequested;
        public StartView()
        {
            InitializeComponent();
        }

        public void ExitAppButton_OnClick(object sender, RoutedEventArgs e)
        {
            ExitAppRequested?.Invoke();
        }
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}