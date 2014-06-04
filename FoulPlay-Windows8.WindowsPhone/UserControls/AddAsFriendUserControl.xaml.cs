using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace FoulPlay_Windows8.UserControls
{
    public sealed partial class AddAsFriendUserControl : UserControl
    {
        public AddAsFriendUserControl()
        {
            InitializeComponent();
        }

        public void OpenPopup()
        {
            ParentPopup.IsOpen = true;
        }

        public void ClosePopup()
        {
            ParentPopup.IsOpen = false;
        }

        public void SetOffset()
        {
            ParentPopup.HorizontalOffset = (Window.Current.Bounds.Width - 400)/2;
            ParentPopup.VerticalOffset = (Window.Current.Bounds.Height - 500)/2;
        }
    }
}