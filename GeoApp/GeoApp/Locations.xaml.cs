using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
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
    public sealed partial class Locations : Page
    {
        public Locations()
        {
            this.InitializeComponent();
            //call method to check for locations saved in the file
            checkFileContent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
                SystemNavigationManager.GetForCurrentView().BackRequested += Locations_BackRequested;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void Locations_BackRequested(object sender, BackRequestedEventArgs e)
        {
            //handle the back request and if able to return to a previous page then allow it
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private async void checkFileContent()
        {
            //check if the textblock has any text if not clear button should not be shown
            if (txtBsavedLocations.Text.ToString() == "")
            {
                clearLocations.Visibility = Visibility.Collapsed;
            }
            //get a link to current folder and read from locations.txt file
            StorageFolder locationsFolder = ApplicationData.Current.LocalFolder;
            
            StorageFile locations;
            try
            {
                locations = await locationsFolder.GetFileAsync("locations.txt");
            }
            catch (Exception E)
            {
                string message = E.Message;
                return;
            }
            //read from the file
            string fileText = await Windows.Storage.FileIO.ReadTextAsync(locations);
            //output file contents to textblock
            txtBsavedLocations.Text = txtBsavedLocations.Text + fileText;
            //if the textblock has text now show the clear button
            if(txtBsavedLocations.Text.ToString() != "")
            {
                clearLocations.Visibility = Visibility.Visible;
            }
            
        }

        private async void clearLocations_Click(object sender, RoutedEventArgs e)
        {
            //get a link to the Current folder and file
            StorageFolder locationsFolder = ApplicationData.Current.LocalFolder;

            StorageFile locations;
            try
            {
                locations = await locationsFolder.GetFileAsync("locations.txt");
            }
            catch (Exception E)
            {
                string message = E.Message;
                return;
            }
            //now put an empty string into the file to overwrite its contents to an empty file
            string empty = "";
            await Windows.Storage.FileIO.WriteTextAsync(locations, empty);
            //get the textblock text from resouces file and collapse clear button
            txtBsavedLocations.Text = (Localization.Get("locationsCleared"));
            clearLocations.Visibility = Visibility.Collapsed;
        }
    }
}
