using System;
using System.Collections.Generic;
using System.Linq;
using CognitiveServices.Services;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace CognitiveServices
{
    public partial class MainPage : ContentPage
    {
        MediaFile _photo;

        public MainPage()
        {
            InitializeComponent();

            TakePhotoButton.Clicked += OnTakePhoto;
            SelectPhotoButton.Clicked += OnSelectPhoto;
            AnalyzeFacesButton.Clicked += OnAnalyzeFaces;
            AnalyzePhotoButton.Clicked += OnAnalyzePhoto;
        }

        private async void OnTakePhoto(object sender, EventArgs e)
        {
            EnableAnayzeButtons(false);

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "Camera is not available.", "OK");
                TakePhotoButton.IsEnabled = false;
                return;
            }

            var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Faces",
                PhotoSize = PhotoSize.Medium,
                SaveToAlbum = true
            });

            SetPhoto(photo);
        }

        private async void OnSelectPhoto(object sender, EventArgs e)
        {
            EnableAnayzeButtons(false);

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("No Gallery", "Access to the gallery is not available.", "OK");
                SelectPhotoButton.IsEnabled = false;
                return;
            }

            var photo = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                PhotoSize = PhotoSize.Medium
            });

            SetPhoto(photo);
        }

        private void SetPhoto(MediaFile photo)
        {
            if (photo == null) return;

            _photo = photo;
            PhotoImage.Source = ImageSource.FromStream(() => _photo.GetStream());
            EnableAnayzeButtons(true);
        }

        private void EnableAllButtons(bool enabled)
        {
            ActivityIndicator.IsRunning = !enabled;
            TakePhotoButton.IsEnabled = enabled;
            SelectPhotoButton.IsEnabled = enabled;
            EnableAnayzeButtons(enabled);
        }

        private void EnableAnayzeButtons(bool enabled)
        {
            AnalyzeFacesButton.IsEnabled = enabled;
            AnalyzePhotoButton.IsEnabled = enabled;
        }

        private async void OnAnalyzeFaces(object sender, EventArgs e)
        {
            if (_photo == null) return;
            EnableAllButtons(false);
            try
            {
                IList<DetectedFace> faces = await FaceDetection.MakeAnalysisRequest(_photo);
                DetectedFace face = faces.FirstOrDefault();
                ActivityIndicator.IsRunning = false;
                if (face == null)
                {
                    await DisplayAlert("Face Analysis", "No Faces Found", "OK");
                    return;
                }
                string smiling = face.FaceAttributes.Smile >= 0.75 ? "smiling" : "not smiling";
                var analysis = $"{face.FaceAttributes.Age} year old {face.FaceAttributes.Gender} who is {smiling}.";
                await DisplayAlert("Face Analysis", analysis, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Analysis Error", ex.Message, "OK");
            }
            finally
            {
                EnableAllButtons(true);
            }
        }

        private async void OnAnalyzePhoto(object sender, EventArgs e)
        {
            if (_photo == null) return;
            EnableAllButtons(false);
            try
            {
                ImageAnalysis analysis = await ComputerVision.MakeAnalysisRequest(_photo);
                ActivityIndicator.IsRunning = false;
                await DisplayAlert("Image Analysis", analysis?.Description.Captions.FirstOrDefault()?.Text, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Analysis Error", ex.Message, "OK");
            }
            finally
            {
                EnableAllButtons(true);
            }
        }
    }
}
