using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace ATM_OS
{
    public partial class ContinueOperationView : UserControl
    {
        public event Action OnBackToMainMenu;
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
        
        public void Initialize(HomeView.OperationType operationType, int amount,string currency)
        {
            if (_resultMessage != null && _resultDetails != null)
            {
                _resultMessage.Text = "Successful";
                
                if (HomeView.OperationType.Deposit == operationType)
                {
                    _resultDetails.Text = $"{amount} {currency} deposited successfully";
                }
                else if (HomeView.OperationType.Withdraw == operationType)
                {
                    _resultDetails.Text = $"{amount} {currency} was withdrawn successfully";
                }
                else if (HomeView.OperationType.PinChange == operationType)
                {
                    _resultDetails.Text = "Pin changed successfully";
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