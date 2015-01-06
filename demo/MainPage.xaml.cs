using AeroGear.OAuth2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IFileOpenPickerContinuable, IWebAuthenticationContinuable
    {
        public static MainPage Current;
        public StorageFile file { get; set; }

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
            if (file == null)
            {
                var openPicker = new FileOpenPicker
                {
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                    ViewMode = PickerViewMode.Thumbnail
                };
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.PickSingleFileAndContinue();
                button.Content = "Upload";
            }
            else
            {
                var config = await GoogleConfig.Create(
                    "517285908032-12332132132131321312.apps.googleusercontent.com",
                    new List<string>(new string[] { "https://www.googleapis.com/auth/drive" }),
                    "google"
                );

                //var config = await KeycloakConfig.Create("shoot-third-party", "https://localhost:8443", "shoot-realm");
                //var config = FacebookConfig.Create("1654557457742519", "9cab3cb953d3194908f44f1764b5b921", 
                //    new List<string>(new string[] { "photo_upload, publish_actions" }), "facebook");

                var module = await AccountManager.AddAccount(config);
                if (await module.RequestAccessAndContinue())
                {
                    Upload(module);
                }
            }
        }

        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Count > 0)
            {
                file = args.Files[0];
            }
            else
            {
                button.Content = "Take Picture";
                await new MessageDialog("no file to upload").ShowAsync();
            }
        }

        public async void Upload(OAuth2Module module)
        {
            using (var client = new HttpClient())
            using (var content = new MultipartFormDataContent())
            {
                button.IsEnabled = false;
                progress.IsActive = true;

                client.DefaultRequestHeaders.Authorization = module.AuthenticationHeaderValue();

                var fileContent = new StreamContent((await file.OpenAsync(FileAccessMode.Read)).AsStreamForRead());
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"image\"",
                    FileName = "\"" + file.Name + "\""
                };
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");

                content.Add(fileContent);
                HttpResponseMessage response = await client.PostAsync(new Uri("https://www.googleapis.com/upload/drive/v2/files"), content);
                string responseString = await response.Content.ReadAsStringAsync();

                Debug.WriteLine(responseString);
                await new MessageDialog("uploaded file " + (response.StatusCode != HttpStatusCode.OK ? "un" : "") + "successful").ShowAsync();
            }

            file = null;
            button.Content = "Take Picture";
            button.IsEnabled = true;
            progress.IsActive = false;
        }

        async void IWebAuthenticationContinuable.ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
            Upload(await AccountManager.ParseContinuationEvent(args));
        }
    }
}
