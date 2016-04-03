using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
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

namespace BLELedToggle
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static Guid LedService =>        new Guid("19B10000-E8F2-537E-4F6C-D104768A1214");
        public static Guid LedCharacteristic => new Guid("19B10001-E8F2-537E-4F6C-D104768A1214");

        private GattCharacteristic characteristic;
        private DeviceInformationCollection devices;
        private GattDeviceService service;
        private DeviceInformation device;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.button.IsEnabled = false;
            this.devices = await DeviceInformation.FindAllAsync(
                GattDeviceService.GetDeviceSelectorFromUuid(LedService),
                new string[] { "System.Devices.ContainerId" });

            if (devices.Count == 0)
            {
                this.notice.Text = "Could find paired LED device";
            }
            else
            {
                this.device = devices[0];
                this.service = await GattDeviceService.FromIdAsync(device.Id);

                this.characteristic = service.GetCharacteristics(LedCharacteristic)[0];
                this.button.IsEnabled = true;

                this.notice.Text = "Connected";
            }
        }

        private async void buttonClick(object sender, RoutedEventArgs e)
        {
            this.button.IsEnabled = false;
            byte val = this.button.IsChecked == true ? (byte)0x01 : (byte)0x00;
            var buffer = new[] { val }.AsBuffer();
            await this.characteristic.WriteValueAsync(buffer);
            this.button.IsEnabled = true;
        }
    }
}
