using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Lumia.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PropertyDescriptions
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private BitmapImage bitmap;
        private WriteableBitmapRenderer renderer;
        private WriteableBitmap resultBitmap;
        private EffectViewModel viewModel;

        public MainPage()
        {
            this.InitializeComponent();

            this.viewModel = new EffectViewModel();
        }

        private async void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            await PickImageAsync();
        }

        private async Task PickImageAsync()
        {
            var filePicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                ViewMode = PickerViewMode.Thumbnail
            };

            filePicker.FileTypeFilter.Clear();
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".png");

            StorageFile file = await filePicker.PickSingleFileAsync();

            if (file != null)
            {
                IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
                this.bitmap = new BitmapImage();
                this.bitmap.SetSource(fileStream);

                this.resultBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight);

                this.LoadedImage.Source = this.bitmap;

                fileStream.Seek(0);

                this.viewModel.blur.Source = new RandomAccessStreamImageSource(fileStream);
            }
        }

        private void DefaultButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.EffectRangeSlider.Value = this.viewModel.DefaultValue;
        }

        private async void EffectRangeSlider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            // only if bitmap is loaded
            if (this.bitmap != null)
            {
                this.viewModel.blur.KernelSize = (int)e.NewValue;

                if (this.renderer == null)
                {
                    this.renderer = new WriteableBitmapRenderer(this.viewModel.blur, this.resultBitmap);
                }

                await this.renderer.RenderAsync();

                this.resultBitmap.Invalidate();

                this.LoadedImage.Source = this.resultBitmap;
            }
        }
    }
}
