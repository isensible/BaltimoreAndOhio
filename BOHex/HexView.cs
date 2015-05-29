using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace BOHex
{
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Played", GroupName = "CommonStates")]
    public class HexView : Control
    {
        static HexView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HexView), new FrameworkPropertyMetadata(typeof(HexView)));
        }

        public HexView()
        {
            DataContextChanged += HexView_DataContextChanged;
        }

        public HexViewModel ViewModel
        {
            get { return (HexViewModel)DataContext; }
        }

        public override void OnApplyTemplate()
        {
            var focusPart = GetTemplateChild("FocusPart") as FrameworkElement;
            if (focusPart != null)
            {
                focusPart.MouseEnter += HexCellMouseEnter;
                focusPart.MouseLeave += HexCellMouseLeave;
                focusPart.MouseLeftButtonUp += HexCellMouseClick;
            }

            GoToStateForOcccupied();
        }

        private void HexCellMouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CanPlay())
            {
                PlayCell();
            }
        }

        private void PlayCell()
        {
            ViewModel.PlayCell();
            VisualStateManager.GoToState(this, "Played", true);
        }

        private void HexCellMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (CanPlay())
            {
                ViewModel.LeaveCell();
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        private void HexCellMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (CanPlay())
            {
                ViewModel.EnterCell();
                VisualStateManager.GoToState(this, "MouseOver", true);
            }
        }

        private bool CanPlay()
        {
            return true;
        }

        private void HexView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldValue = e.OldValue as INotifyPropertyChanged;
            if (oldValue != null)
            {
                oldValue.PropertyChanged -= OnViewModelPropertyChanged;
            }

            var newValue = e.NewValue as INotifyPropertyChanged;
            if (newValue != null)
            {
                newValue.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Occupied")
            {
                this.GoToStateForOcccupied();
            }
        }

        private void GoToStateForOcccupied()
        {
            //if (this.ViewModel.Occupied == Occupied.Empty)
            //{
            //    VisualStateManager.GoToState(this, "Normal", true);
            //}
            //else
            //{
            //    VisualStateManager.GoToState(this, "Played", true);
            //}
        }

        
    }
}
