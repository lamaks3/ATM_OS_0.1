using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace ATM_OS
{
    public partial class ContinueOperationView : UserControl
    {
        // События для навигации
        public event Action OnBackToMainMenu; // Изменено: было OnBackToStartMenu
        public event Action OnShowPartingView;

        private TextBlock _resultMessage;
        private TextBlock _resultDetails;

        public ContinueOperationView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            
            // Находим элементы после загрузки XAML
            _resultMessage = this.FindControl<TextBlock>("ResultMessage");
            _resultDetails = this.FindControl<TextBlock>("ResultDetails");
        }

        // Метод для инициализации с данными о выполненной операции
        public void Initialize(string operationType, int amount, bool success,string currency)
        {
            if (_resultMessage != null && _resultDetails != null)
            {
                if (success)
                {
                    _resultMessage.Text = "Successful";
                    
                    if (operationType == "Deposit")
                    {
                        _resultDetails.Text = $"{amount:N0} {currency} deposited successfully";
                    }
                    else if (operationType == "Withdraw")
                    {
                        _resultDetails.Text = $"{amount:N0} {currency} was withdrawn successfully";
                    }
                    else if (operationType == "Receipt")
                    {
                        _resultDetails.Text = "Receipt printed successfully";
                    }
                    else
                    {
                        _resultDetails.Text = "Operation completed successfully";
                    }
                }
                else
                {
                    _resultMessage.Text = "Unsuccessful";
                    
                    if (operationType == "Deposit")
                    {
                        _resultDetails.Text = "Deposit failed. Please try again.";
                    }
                    else if (operationType == "Withdraw")
                    {
                        _resultDetails.Text = "Withdrawal failed. Please check your balance.";
                    }
                    else
                    {
                        _resultDetails.Text = "Operation failed. Please try again.";
                    }
                }
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackToMainMenu?.Invoke(); 
        }

        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            OnShowPartingView?.Invoke();
        }
    }
}