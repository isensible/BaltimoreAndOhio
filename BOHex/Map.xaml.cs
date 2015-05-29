using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BOHex
{
    public partial class Map
    {
        public Map()
        {
            InitializeComponent();
        }

        private void HexBoardSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var viewModel = this.DataContext as MapViewModel;
            if (viewModel != null)
            {
                viewModel.SetActualBoardSize(e.NewSize);
            }
        }

        private void HexBoardDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = this.DataContext as MapViewModel;
            if (viewModel != null)
            {
                viewModel.SetActualBoardSize(new Size(this.ActualWidth, this.ActualHeight));
            }
        }
    }
}
