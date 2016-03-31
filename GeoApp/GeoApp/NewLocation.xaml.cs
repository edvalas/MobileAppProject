using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace GeoApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewLocation : Page
    {
        //geolocator obj
        Geolocator geolocator;

        public NewLocation()
        {
            this.InitializeComponent();
            //set up the geolocator when navigated to this page
            setupGeoLocation();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
                SystemNavigationManager.GetForCurrentView().BackRequested += NewLocations_BackRequested;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void NewLocations_BackRequested(object sender, BackRequestedEventArgs e)
        {
            //handle the back request and if there is a page to go back to allow it
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private async void setupGeoLocation()
        {
            // ask for permission 
            var accessStatus = await Geolocator.RequestAccessAsync();
            //check for geolocator if access is allowed or denied
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    {
                        await new MessageDialog(Localization.Get("setupGeo")).ShowAsync();

                        geolocator = new Geolocator();
                        geolocator.DesiredAccuracy = PositionAccuracy.High;
                        geolocator.StatusChanged += MyGeo_StatusChanged;

                        break;
                    }
                case GeolocationAccessStatus.Denied:
                    {
                        await new MessageDialog(Localization.Get("setupGeoDenied")).ShowAsync();
                        break;
                    }
                default:
                    {
                        await new MessageDialog(Localization.Get("setupGeoDefault")).ShowAsync();
                        break;
                    }
            }
        }

        private async void MyGeo_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            // use the dispatcher with lambda fuction to update the UI thread.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // code to run in method to update UI
                switch (args.Status)
                {
                    case PositionStatus.Ready:
                        tblStatusUpdates.Text = (Localization.Get("lsReady"));
                        break;
                    case PositionStatus.Disabled:
                        tblStatusUpdates.Text = (Localization.Get("lsDisabled")); 
                        break;
                    case PositionStatus.NoData:
                        tblStatusUpdates.Text = (Localization.Get("lsNoData"));
                        break;
                    case PositionStatus.Initializing:
                        tblStatusUpdates.Text = (Localization.Get("lsInit"));
                        break;
                    default:
                        tblStatusUpdates.Text = (Localization.Get("lsDef"));
                        break;
                }

            });

        }

        private void display(Geoposition pos)
        {
            //outputs all the text to the page
            txtBLocation.Text = "Accuracy in meters: " + pos.Coordinate.Accuracy.ToString() + System.Environment.NewLine +
                        "Latitude: " + pos.Coordinate.Latitude.ToString() + System.Environment.NewLine +
                        "Longitude: " + pos.Coordinate.Longitude.ToString() + System.Environment.NewLine +
                        "Time Stamp: " + pos.Coordinate.Timestamp.ToString();
            getDetail(pos);
        }

        private async void getDetail(Geoposition pos)
        {
            //this method uses latitude and longitude to reverse geocode a verbal location like Ireland, galway rather than numbers
            BasicGeoposition location = new BasicGeoposition();
            location.Latitude = pos.Coordinate.Latitude;
            location.Longitude = pos.Coordinate.Longitude;
            Geopoint pointToReverseGeocode = new Geopoint(location);

            // Reverse geocode the specified geographic location.
            MapLocationFinderResult result =
                  await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);

            // If the query returns results, display the name of the town
            // contained in the address of the first result.
            if (result.Status == MapLocationFinderStatus.Success)
            {
                //output what I want from the result query like town,street,country..
                txtBLocation.Text += "\nCountry: " + result.Locations[0].Address.Country.ToString() +
                    "\nTown: " + result.Locations[0].Address.Town.ToString() +
                    "\nStreet: " + result.Locations[0].Address.Street.ToString();
            }
        }

        private async void getLocation_Click(object sender, RoutedEventArgs e)
        {
            //save new location into pos using geolocator
            Geoposition pos = await geolocator.GetGeopositionAsync();

            await new MessageDialog(Localization.Get("getPos")).ShowAsync();
            position.Text = (Localization.Get("currentLocation"));
            //calls other method to display the details of current location
            display(pos);
        }

        private async void saveLocation_Click(object sender, RoutedEventArgs e)
        {
            //check if there is text in the textblock before you save empty string to a file
            if (txtBLocation.Text.ToString() == "")
            {
                //display error saying no location found
                await new MessageDialog(Localization.Get("noLocation")).ShowAsync();
            }
            else
            {
                //get a link to current folder
                StorageFolder locationsFolder = ApplicationData.Current.LocalFolder;

                StorageFile locations;
                //string to contain current file contents
                string fileText = "";

                try
                {
                    //get the file and its contents to a string
                    locations = await locationsFolder.GetFileAsync("locations.txt");
                    fileText = await Windows.Storage.FileIO.ReadTextAsync(locations);
                }
                catch (Exception E)
                {
                    //if its not there create a new file
                    string message = E.Message;
                    locations = await locationsFolder.CreateFileAsync("locations.txt");
                }
                //string to add on to the file contents
                string output;
                // which is equal to the text of the found location
                output = txtBLocation.Text.ToString();
                //write back to the file its original contents plus the output(new location details)
                await Windows.Storage.FileIO.WriteTextAsync(locations, fileText + output + System.Environment.NewLine + System.Environment.NewLine);
                //msg dialog
                await new MessageDialog(Localization.Get("locationSaved")).ShowAsync();
            }
        }
    }
}
