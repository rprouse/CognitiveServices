using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            AnalyzeOcrButton.Clicked += OnAnalyzeOcr;
            AnalyzeTextButton.Clicked += OnAnalyzeText;
        }

        private async void OnTakePhoto(object sender, EventArgs e)
        {
            EnableAnayzeButtons(false);

            try
            {
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
            finally
            {
                EnableAnayzeButtons(_photo != null);
            }
        }

        private async void OnSelectPhoto(object sender, EventArgs e)
        {
            EnableAnayzeButtons(false);

            try
            {
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
            finally
            {
                EnableAnayzeButtons(_photo != null);
            }
        }

        private void SetPhoto(MediaFile photo)
        {
            if (photo == null) return;

            _photo = photo;
            PhotoImage.Source = ImageSource.FromStream(() => _photo.GetStream());
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
            AnalyzeOcrButton.IsEnabled = enabled;
            AnalyzeTextButton.IsEnabled = enabled;
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

        private async void OnAnalyzeOcr(object sender, EventArgs e)
        {
            if (_photo == null) return;
            EnableAllButtons(false);
            try
            {
                OcrResult result = await ComputerVision.MakeOcrRequest(_photo);
                ActivityIndicator.IsRunning = false;
                if ((result?.Regions.Count ?? 0) == 0)
                {
                    await DisplayAlert("OCR Result", "No text found", "OK");
                    return;
                }

                var text = new StringBuilder();
                foreach(var region in result.Regions)
                {
                    foreach(var line in region.Lines)
                    {
                        text.AppendLine( string.Join(" ", line.Words.Select(w => w.Text)));
                    }
                    text.AppendLine();
                }

                await DisplayAlert("OCR Result", text.ToString(), "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("OC Error", ex.Message, "OK");
            }
            finally
            {
                EnableAllButtons(true);
            }
        }

        private async void OnAnalyzeText(object sender, EventArgs e)
        {
            if (_photo == null) return;
            EnableAllButtons(false);
            try
            {
                RecognitionResult result = await ComputerVision.MakeTextRequest(_photo);
                ActivityIndicator.IsRunning = false;
                if (result == null)
                {
                    await DisplayAlert("Text Result", "No text found", "OK");
                    return;
                }

                var text = new StringBuilder();
                foreach (var line in result.Lines)
                {
                    text.AppendLine(string.Join(" ", line.Text));
                }
                await DisplayAlert("Text Result", text.ToString(), "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Text Error", ex.Message, "OK");
            }
            finally
            {
                EnableAllButtons(true);
            }
        }
    }
}
