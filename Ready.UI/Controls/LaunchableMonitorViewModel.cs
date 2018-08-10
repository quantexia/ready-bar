using Ready.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ready.UI.Controls
{
    public class LaunchableMonitorViewModel : INotifyPropertyChanged
    {
        private LaunchableMonitor lmon;
        
        public LaunchableMonitorViewModel(LaunchableMonitor lmon)
        {
            this.lmon = lmon;
            LaunchCommand = new RelayCommand(o => lmon.NextAvailable.Reveal(), o => lmon.HasAvailable);
            PeriodicTaskFactory.Start(() => Check(), 1000 * 1);
        }

        public Launchable Launchable { get { return lmon.Target; } }

        private void Check()
        {
            //System.Diagnostics.Debug.WriteLine("Do");
            Notify("Launchable");
            Notify("Image");
            Notify("HasAvailable");
            Notify("CountAvailable");
            Notify("NextPid");
        }

        public bool HasAvailable { get { return lmon.HasAvailable; } }
        public int CountAvailable {  get { return lmon.CountAvailable; } }
        public int NextPid { get { return lmon.NextAvailable == null ? 0 : lmon.NextAvailable.Process.Id; } }
        public ImageSource Image { get { return lmon.NextAvailable == null ? createdef() : lmon.NextAvailable.Image; } } 

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand LaunchCommand { get; private set; }

        private ImageSource createdef()
        {
            int width = 128;
            int height = width;
            int stride = width / 8;
            byte[] pixels = new byte[height * stride];

            // Try creating a new image with a custom palette.
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Colors.Red);
            colors.Add(System.Windows.Media.Colors.Blue);
            colors.Add(System.Windows.Media.Colors.Green);
            BitmapPalette myPalette = new BitmapPalette(colors);

            // Creates a new empty image with the pre-defined palette
            BitmapSource image = BitmapSource.Create(
                                                     width, height,
                                                     96, 96,
                                                     PixelFormats.Indexed1,
                                                     myPalette,
                                                     pixels,
                                                     stride);

            return image;
        }
    }
}
