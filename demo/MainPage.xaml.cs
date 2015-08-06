using AeroGear.OAuth2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public StorageFile file { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
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
                file = await openPicker.PickSingleFileAsync();
                button.Content = "Upload";
            }
            else
            {
                var config = await GoogleConfig.Create(
                    "517285908032-tjn5607ris2msdmfm2mdic00m0phsgmg.apps.googleusercontent.com",
                    new List<string>(new string[] { "https://www.googleapis.com/auth/drive" }),
                    "google"
                );

                //var config = await KeycloakConfig.Create("shoot-third-party", "https://localhost:8443", "shoot-realm");
                //var config = FacebookConfig.Create("853024668116416", "561448404a97917b704dcc0e215e9cff",
                //    new List<string>(new string[] { "publish_actions" }), "facebook");

                var module = await AccountManager.AddAccount(config);
                await module.RequestAccess();

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

                    string responseString = null;
                    HttpResponseMessage response = await client.PostAsync(new Uri("https://www.googleapis.com/upload/drive/v2/files"), content);
                    responseString = await response.Content.ReadAsStringAsync();

                    Debug.WriteLine(responseString);
                    await new MessageDialog("uploaded file " + (response.StatusCode != HttpStatusCode.OK ? "un" : "") + "successful").ShowAsync();
                }

                file = null;
                button.Content = "Take Picture";
                button.IsEnabled = true;
                progress.IsActive = false;
            }
        }
    }
}
