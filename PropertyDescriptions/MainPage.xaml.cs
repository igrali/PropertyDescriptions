using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Lumia.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PropertyDescriptions
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SwapChainPanelRenderer renderer;
        private EffectViewModel viewModel;

        public MainPage()
        {
            this.InitializeComponent();

            this.viewModel = new EffectViewModel();

            this.ImageSwapChainPanel.Loaded += ImageSwapChainPanel_Loaded;
        }

        private void ImageSwapChainPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ImageSwapChainPanel.ActualHeight > 0 && this.ImageSwapChainPanel.ActualWidth > 0)
            {
                if (this.renderer == null)
                {
                    this.renderer = new SwapChainPanelRenderer(this.viewModel.blur, this.ImageSwapChainPanel);
                }
            }

            this.ImageSwapChainPanel.SizeChanged += async (s, args) =>
            {
                await this.renderer.RenderAsync();
            };
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

                this.viewModel.blur.Source = new RandomAccessStreamImageSource(fileStream);

                await this.renderer.RenderAsync();
            }
        }

        private void DefaultButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.EffectRangeSlider.Value = this.viewModel.DefaultValue;
        }

        private async void EffectRangeSlider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (this.renderer != null)
            {
                this.viewModel.blur.KernelSize = (int)e.NewValue;

                await this.renderer.RenderAsync();
            }
        }
    }
}
