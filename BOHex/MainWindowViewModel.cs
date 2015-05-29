namespace BOHex
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly MapViewModel _mapViewModel;

        public MainWindowViewModel(MapViewModel mapViewModel)
        {
            _mapViewModel = mapViewModel;
        }

        public MapViewModel MapViewModel
        {
            get { return _mapViewModel; }
        }
    }
}