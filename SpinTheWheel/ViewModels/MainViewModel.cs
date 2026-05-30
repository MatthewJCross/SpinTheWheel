using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using SpinTheWheel.Models;
using SpinTheWheel.Utils;

namespace SpinTheWheel.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly Random _random = new();

        public ObservableCollection<WheelEntry> Entries { get; } = new();

        private double _rotationAngle;
        public double RotationAngle
        {
            get => _rotationAngle;
            set { _rotationAngle = value; OnPropertyChanged(); }
        }

        private string _winner;
        public string Winner
        {
            get => _winner;
            set { _winner = value; OnPropertyChanged(); }
        }

        private int _winnerIndex;
        public int WinnerIndex => _winnerIndex;

        public ICommand SpinCommand { get; }

        public MainViewModel()
        {
            SpinCommand = new RelayCommand(Spin);

            // Default entries
            Entries.Add(new WheelEntry { Text = "Matt", Color = Brushes.Purple });
            Entries.Add(new WheelEntry { Text = "Jamie", Color = Brushes.MediumSeaGreen });
            Entries.Add(new WheelEntry { Text = "Andy", Color = Brushes.Goldenrod });
            Entries.Add(new WheelEntry { Text = "Chris", Color = Brushes.SteelBlue });
            Entries.Add(new WheelEntry { Text = "Richard", Color = Brushes.Red });
            Entries.Add(new WheelEntry { Text = "Paul", Color = Brushes.Bisque });
            Entries.Add(new WheelEntry { Text = "Calum", Color = Brushes.Orchid });
            Entries.Add(new WheelEntry { Text = "Seth", Color = Brushes.Chocolate });
            Entries.Add(new WheelEntry { Text = "James", Color = Brushes.Yellow });
            Entries.Add(new WheelEntry { Text = "Patrick", Color = Brushes.MistyRose });
            Entries.Add(new WheelEntry { Text = "Jasper", Color = Brushes.Coral });
        }

        private void Spin()
        {
            if (Entries.Count == 0) return;

            _winnerIndex = _random.Next(Entries.Count);
            Winner = $"Winner: {Entries[_winnerIndex].Text}";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
