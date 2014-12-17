using AeroGear.OAuth2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IFileOpenPickerContinuable
    {
        public static MainPage Current;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            Current = this;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                ViewMode = PickerViewMode.Thumbnail
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.PickSingleFileAndContinue();
        }

        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Count > 0)
            {
                var file = args.Files[0];

                var config = new Config()
                {
                    baseURL = new Uri("https://accounts.google.com/"),
                    authzEndpoint = "o/oauth2/auth",
                    redirectURL = "com.aerogear.oauth.test:/oauth2Callback",
                    accessTokenEndpoint = "o/oauth2/token",
                    refreshTokenEndpoint = "o/oauth2/token",
                    revokeTokenEndpoint = "rest/revoke",
                    clientId = "517285908032-8m6kbdccps1tpsnsrb5281sglvb2qo9g.apps.googleusercontent.com",
                    scopes = new List<string>(new string[] { "https://www.googleapis.com/auth/drive" }),
                    accountId = "google"
                };

                var module = AccountManager.AddAccount(config);

                var request = AuthzWebRequest.Create("https://www.googleapis.com/upload/drive/v2/files", module);

                using (var postStream = await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, request))
                {
                    using (var stream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        Stream s = stream.AsStreamForRead();
                        s.CopyTo(postStream);
                    }
                }
            }
            else
            {
                await new MessageDialog("no file to upload").ShowAsync();
            }
        }
    }
}
