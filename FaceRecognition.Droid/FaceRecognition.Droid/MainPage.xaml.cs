using System;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace FaceRecognition.Droid
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            CameraButton.Clicked += OnCameraButtonClicked;
        }

        private async void OnCameraButtonClicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Faces",
                PhotoSize = PhotoSize.Medium
            });

            if (file == null)
                return;

            PhotoImage.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });
        }
    }
}
