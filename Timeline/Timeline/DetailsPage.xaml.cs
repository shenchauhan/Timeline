using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.UserActivities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Shell;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Timeline
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailsPage : Page, INotifyPropertyChanged
    {
        private UserActivityChannel _userActivityChannel;
        private UserActivity _userActivity;
        private UserActivitySession _userActivitySession;

        private GalleryItem _galleryItem;

        public DetailsPage()
        {
            this.InitializeComponent();
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is GalleryItem galleryItem)
            {
                GalleryItem = galleryItem;
            }

            if (Frame.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                SystemNavigationManager.GetForCurrentView().BackRequested += DetailsPage_BackRequested;
            }


            _userActivityChannel = UserActivityChannel.GetDefault();
            _userActivity = await _userActivityChannel.GetOrCreateUserActivityAsync("LikePhoto");
        }

        private async void SymbolIcon_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Fetch the adative card JSON
            var adaptiveCard = File.ReadAllText($@"{Package.Current.InstalledLocation.Path}\AdaptiveCard.json");

            // Replace the content.
            adaptiveCard = adaptiveCard.Replace("{{backgroundImage}}", _galleryItem.PublicUrl);
            adaptiveCard = adaptiveCard.Replace("{{name}}", _galleryItem.Name);

            // Create the protocol, so when the clicks the Adaptive Card on the Timeline, it will directly launch to the correct image.
            _userActivity.ActivationUri = new Uri($"my-timeline://details?{_galleryItem.ImageSource.Replace("ms-appx:///Assets/Images/", "")}");

            // Set the display text to the User Activity(e.g. Pike Place market.)
            _userActivity.VisualElements.DisplayText = _galleryItem.Name;

            // Assign the Adaptive Card to the user activity. 
            _userActivity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(adaptiveCard);

            // Save the details user activity.
            await _userActivity.SaveAsync();

            // Dispose of the session and create a new one ready for the next user activity.
            _userActivitySession?.Dispose();
            _userActivitySession = _userActivity.CreateSession();

            await new MessageDialog("Added to timeline").ShowAsync();

        }

        private void DetailsPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                Frame.Navigate(typeof(MainPage));
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= DetailsPage_BackRequested;
        }

        public GalleryItem GalleryItem
        {
            get
            {
                return _galleryItem;
            }
            set
            {
                _galleryItem = value;
                RaisePropertyChanged();
            }
        }

        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
