using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.Devices.Geolocation;
using Windows.Storage;


namespace GeoApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void navSavedLocations_Click(object sender, RoutedEventArgs e)
        {
            //navigation on button click
            Frame.Navigate(typeof(Locations));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //collapse the back button on this main page.
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;

            base.OnNavigatedTo(e);
        }

        private void navNewLocation_Click(object sender, RoutedEventArgs e)
        {
            //navigation on button click
            Frame.Navigate(typeof(NewLocation));
        }
    }
}
