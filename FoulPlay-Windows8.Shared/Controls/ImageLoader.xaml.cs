using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace FoulPlay_Windows8.Controls
{
    public sealed partial class ImageLoader : UserControl
    {
        public static DependencyProperty LoadingContentProperty =
            DependencyProperty.Register("LoadingContent",
                typeof (object),
                typeof (ImageLoader), null);

        public static DependencyProperty FailedContentProperty =
            DependencyProperty.Register("FailedContent",
                typeof (object),
                typeof (ImageLoader), null);

        public static DependencyProperty SourceProperty =
            DependencyProperty.Register("Source",
                typeof (ImageSource),
                typeof (ImageLoader),
                new PropertyMetadata(null, OnSourceChanged));

        public ImageLoader()
        {
            InitializeComponent();
        }

        public object LoadingContent
        {
            get { return (base.GetValue(LoadingContentProperty)); }
            set { base.SetValue(LoadingContentProperty, value); }
        }

        public object FailedContent
        {
            get { return (base.GetValue(FailedContentProperty)); }
            set { base.SetValue(FailedContentProperty, value); }
        }

        public ImageSource Source
        {
            get { return ((ImageSource) base.GetValue(SourceProperty)); }
            set { base.SetValue(SourceProperty, value); }
        }

        private static void OnSourceChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs args)
        {
            var loader = (ImageLoader) sender;
            VisualStateManager.GoToState(loader, "Loading", true);
        }

        private void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Failed", true);
        }

        private void OnImageOpened(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Displaying", true);
        }
    }
}