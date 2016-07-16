using Microsoft.IoT.Lightning.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using Windows.Devices.Spi;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DemoPCF8575Lighting
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private I2cDevice m_i2cPCF8575;
        private DispatcherTimer m_timer;

        public MainPage()
        {
            this.InitializeComponent();
            setup();
        }

        private async Task setup()
        {
            try
            {
                if (LightningProvider.IsLightningEnabled)
                {
                    LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();
                }
                var gpioController = await GpioController.GetDefaultAsync();
                var i2cController = await I2cController.GetDefaultAsync();
                var spiController = await SpiController.GetDefaultAsync();

                var i2cSettings = new I2cConnectionSettings(0x20); //no additional bits
                i2cSettings.BusSpeed = I2cBusSpeed.FastMode;
                m_i2cPCF8575 = i2cController.GetDevice(i2cSettings);
                m_i2cPCF8575.Write(new byte[] { 0x00, 0x00 });//All pins as input; no interrupt
                m_timer = new DispatcherTimer();
                m_timer.Interval = TimeSpan.FromMilliseconds(500);
                m_timer.Tick += M_timer_Tick;
                m_timer.Start();
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            
        }

        private void M_timer_Tick(object sender, object e)
        {
            byte[] arr = new byte[2];
            m_i2cPCF8575.Read(arr);
            Debug.WriteLine($"{arr[0]},{arr[1]}");
        }
    }
}
