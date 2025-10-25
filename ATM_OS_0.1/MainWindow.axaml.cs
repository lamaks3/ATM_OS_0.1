using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using Avalonia.Styling;

namespace ATM_OS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartPulseAnimation();
        }

        private void StartPulseAnimation()
        {
            var animation = new Animation
            {
                Duration = TimeSpan.FromSeconds(2),
                IterationCount = IterationCount.Infinite,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0.0),
                        Setters =
                        {
                            new Setter { Property = ScaleTransform.ScaleXProperty, Value = 1.0 },
                            new Setter { Property = ScaleTransform.ScaleYProperty, Value = 1.0 },
                            new Setter { Property = OpacityProperty, Value = 0.7 }
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(0.5),
                        Setters =
                        {
                            new Setter { Property = ScaleTransform.ScaleXProperty, Value = 1.3 },
                            new Setter { Property = ScaleTransform.ScaleYProperty, Value = 1.3 },
                            new Setter { Property = OpacityProperty, Value = 0.3 }
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1.0),
                        Setters =
                        {
                            new Setter { Property = ScaleTransform.ScaleXProperty, Value = 1.0 },
                            new Setter { Property = ScaleTransform.ScaleYProperty, Value = 1.0 },
                            new Setter { Property = OpacityProperty, Value = 0.7 }
                        }
                    }
                }
            };
        }
    }
}