using AeroGear.OAuth2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
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
                    "517285908032-11moj33qbn01m7sem6g7gmfco2tp252v.apps.googleusercontent.com",
                    new List<string>(new string[] { "https://www.googleapis.com/auth/drive" }),
                    "google"
                );

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
            HttpContent stringContent = new StringContent(file.Name);

            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                client.DefaultRequestHeaders.Authorization = module.AuthenticationHeaderValue();

                formData.Add(new StreamContent((await file.OpenAsync(FileAccessMode.Read)).AsStreamForRead()), file.Name);

                button.IsEnabled = false;
                progress.IsActive = true;

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri("https://www.googleapis.com/upload/drive/v2/files"));
                requestMessage.Content = formData;
                HttpResponseMessage responseObject = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
                Debug.WriteLine(responseObject);
                await new MessageDialog("uploaded file " + (responseObject.StatusCode != HttpStatusCode.OK ? "un" : "") + "successful").ShowAsync();
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
