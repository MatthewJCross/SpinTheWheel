using SpinTheWheel.ViewModels;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpinTheWheel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Random _random = new();
        private bool _isSpinning;
        private SoundPlayer _tickPlayer;
        private double _lastTickAngle;
        public MainWindow()
        {
            InitializeComponent();
            this.StateChanged += MainWindow_StateChanged;

            var stream = Application.GetResourceStream(new Uri("Resources/tick.wav", UriKind.Relative)).Stream;

            _tickPlayer = new SoundPlayer(stream);
            _tickPlayer.Load();
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                MaximizeButton.Visibility = Visibility.Visible;
                RestoreButton.Visibility = Visibility.Collapsed;
            }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Spin_Click(object sender, RoutedEventArgs e)
        {
            if (_isSpinning) 
                return;

            var vm = (MainViewModel)DataContext;
            int count = vm.Entries.Count;
            if (count == 0) 
                return;

            _isSpinning = true;

            double slice = 360.0 / count;

            // Pick winner
            int targetIndex = _random.Next(count);

            // Centre of slice
            double targetSliceCenter = targetIndex * slice + slice / 2;

            // Normalize current angle
            double currentAngle = WheelRotate.Angle % 360;

            // We want that slice centre to end at 0° (pointer at top)
            double targetAngle = 360 - targetSliceCenter - 90;

            // Extra spins for drama
            double spins = 360 * 5;

            double finalAngle = WheelRotate.Angle + spins + (targetAngle - currentAngle);

            _lastTickAngle = WheelRotate.Angle;

            var animation = new DoubleAnimation
            {
                To = finalAngle,
                Duration = TimeSpan.FromSeconds(5),
                DecelerationRatio = 0.9
            };

            animation.CurrentTimeInvalidated += (_, __) =>
            {
                double current = WheelRotate.Angle;
                double diff = Math.Abs(current - _lastTickAngle);

                if (diff >= slice)
                {
                    PlayTick();
                    _lastTickAngle = current;
                }
            };

            animation.Completed += (_, __) =>
            {
                vm.Winner = $"Winner: {vm.Entries[targetIndex].Text}";
                _isSpinning = false;
            };

            WheelRotate.BeginAnimation(RotateTransform.AngleProperty, animation);
        }

        private void PlayTick()
        {
            _tickPlayer.Play();
        }
    }
}