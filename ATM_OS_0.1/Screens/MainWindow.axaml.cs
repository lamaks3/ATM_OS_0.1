using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using Avalonia.Styling;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace ATM_OS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartPulseAnimation();
            InitializeNfcListener();
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

            // Применяем анимацию к изображению NFC
            var nfcImage = this.FindControl<Image>("NfcImage");
            if (nfcImage != null)
            {
                animation.RunAsync(nfcImage);
            }
        }

        private void InitializeNfcListener()
        {
            // Запускаем проверку карт в фоновом режиме
            Task.Run(async () => await CheckForCards());
        }

        private async Task CheckForCards()
        {
            while (true)
            {
                string cardUID = NfcScannerService.GetCardUID();
                
                if (!string.IsNullOrEmpty(cardUID))
                {
                    // Проверяем существование карты в базе
                    var repository = new CardHolderRepository();
                    if (repository.CardExists(cardUID))
                    {
                        // Карта распознана и существует - переключаем на окно PIN
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            ShowPinWindow(cardUID);
                        });
                    }
                    
                    // Сбрасываем UID для следующего сканирования
                    NfcScannerService.SetCardUID(string.Empty);
                }
                
                await Task.Delay(500); // Проверяем каждые 500ms
            }
        }

        private void ShowPinWindow(string cardUID)
        {
            var pinWindow = new PinWindow(cardUID);
            pinWindow.Show();
            this.Close();
        }
    }
}