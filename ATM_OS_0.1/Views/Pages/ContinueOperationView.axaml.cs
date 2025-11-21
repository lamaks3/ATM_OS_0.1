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
            
            _resultMessage = this.FindControl<TextBlock>("ResultMessage");
            _resultDetails = this.FindControl<TextBlock>("ResultDetails");
        }
        
        public void Initialize(string operationType, int amount,string currency)
        {
            if (_resultMessage != null && _resultDetails != null)
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