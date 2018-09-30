using System;
using System.Collections.Generic;
using System.Linq;
using CognitiveServices.Services;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace CognitiveServices
{
    public partial class MainPage : ContentPage
    {
        MediaFile _file;
        FaceDetection _faceDetection;

        public MainPage()
        {
            InitializeComponent();

            _faceDetection = new FaceDetection(this);

            CameraButton.Clicked += OnCameraButtonClicked;
            AnalyzeButton.Clicked += OnAnalyzeButtonClicked;
        }

        private async void OnCameraButtonClicked(object sender, EventArgs e)
        {
            AnalyzeButton.IsEnabled = false;

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            _file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Faces",
                PhotoSize = PhotoSize.Medium,
                SaveToAlbum = true
            });

            if (_file == null)
                return;

            PhotoImage.Source = ImageSource.FromStream(() =>
            {
                var stream = _file.GetStream();
                return stream;
            });

            AnalyzeButton.IsEnabled = true;
        }

        private async void OnAnalyzeButtonClicked(object sender, EventArgs e)
        {
            if (_file == null) return;
            IList<DetectedFace> faces = await _faceDetection.MakeAnalysisRequest(_file);
            DetectedFace face = faces.FirstOrDefault();
            if (face == null)
            {
                await DisplayAlert("Face Analysis", "No Faces Found", "OK");
            }
            string smiling = face.FaceAttributes.Smile >= 0.75 ? "smiling" : "not smiling";            var analysis = $"{face.FaceAttributes.Age} year old {face.FaceAttributes.Gender} who is {smiling}.";
            await DisplayAlert("Face Analysis", analysis, "OK");
        }
    }
}
