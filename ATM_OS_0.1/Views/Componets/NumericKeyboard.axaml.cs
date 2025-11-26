using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace ATM_OS
{
    public partial class NumericKeyboard : UserControl
    {
        private string _currentValue = "0";
        private int maxLegth = 6;
        private TextBlock _displayTextBlock;
        private bool pinMode = false;
        
        public event Action<string> OnValueChanged;
        public event Action<string> OnValueConfirmed;
        public event Action OnClearPressed;
        public event Action OnBackPressed;

        public NumericKeyboard()
        {
            InitializeComponent();
            InitializeDisplay();
            
            // Добавляем обработку клавиатуры
            this.Focusable = true;
            this.AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
            this.AttachedToVisualTree += (s, e) => this.Focus();
        }
        
        private void InitializeDisplay()
        {
            _displayTextBlock = this.FindControl<TextBlock>("DisplayText");
            UpdateDisplay();
        }

        public void enablePinMode()
        {
            pinMode = true;
            _currentValue = "";
            UpdateDisplay();
        }
        public string CurrentValue => _currentValue;

        public void Reset()
        {
            _currentValue = pinMode ? "" : "0";
            UpdateDisplay();
            this.Focus(); // Возвращаем фокус после сброса
        }

        public void Clean()
        {
            _currentValue = pinMode ? "" : "0";
            UpdateDisplay();
        }

        public void SetMaxLength(int _maxLenght)
        {
            maxLegth = _maxLenght;
        }

        private void UpdateDisplay()
        {
            if (_displayTextBlock != null && !pinMode)
            {
                _displayTextBlock.Text = _currentValue;
            }
            else
            {
                _displayTextBlock.Text = new string('*', _currentValue.Length);
            }
            OnValueChanged?.Invoke(_currentValue);
        }

        // Обработка ввода с клавиатуры
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                char digit = (char)('0' + (e.Key - Key.D0));
                AddDigit(digit);
                e.Handled = true;
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                char digit = (char)('0' + (e.Key - Key.NumPad0));
                AddDigit(digit);
                e.Handled = true;
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Back:
                        RemoveLastDigit();
                        e.Handled = true;
                        break;
                
                    case Key.Enter:
                        OnValueConfirmed?.Invoke(_currentValue);
                        e.Handled = true;
                        break;
                }
            }
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var digit = button.Content.ToString()[0];
            AddDigit(digit);
        }

        private void AddDigit(char digit)
        {
            if (_currentValue.Length >= maxLegth) return;
            
            if (_currentValue == "0" && !pinMode)
                _currentValue = digit.ToString();
            else
                _currentValue += digit;
            
            UpdateDisplay();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _currentValue = pinMode ? "" : "0";
            
            UpdateDisplay();
            OnClearPressed?.Invoke();
        }
        
        private void RemoveLastDigit()
        {
            if (_currentValue.Length > 1)
                _currentValue = _currentValue.Substring(0, _currentValue.Length - 1);
            else
            {
                _currentValue = pinMode ? "" : "0";
            }

            UpdateDisplay();
            OnBackPressed?.Invoke();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            OnValueConfirmed?.Invoke(_currentValue);
        }
    }
}