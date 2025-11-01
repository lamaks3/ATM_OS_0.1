using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using Avalonia.Markup.Xaml;

namespace ATM_OS
{
    public partial class StartView : UserControl
    {
        public StartView()
        {
            InitializeComponent();
        }

        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}