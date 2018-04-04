using Xamarin.Forms;
using System.Reflection;
    
namespace mdOrganizer.UI
{
    public class IconButton : ViewCell
    {
        private Image iconImage;
        /*public static readonly BindableProperty IconProperty = BindableProperty.Create("Icon", typeof(string), typeof(IconButton), null);
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set
            {
                SetValue(IconProperty, value);
                updateControls();
            }
        }

        private void updateControls()
        {
            iconImage.Source = ImageSource.FromResource(Icon);
        }*/

        public IconButton()
        {
            iconImage= new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                Aspect = Aspect.AspectFit
            };

            iconImage.SetBinding(Image.SourceProperty, "Icon");

            Label textLabel = new Label()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                LineBreakMode = LineBreakMode.NoWrap,
                FontSize = Styles.FontSize,
                TextColor = Styles.MenuColor

            };

            textLabel.SetBinding(Label.TextProperty, "Text");

            Grid gridLayout = new Grid()
            {
                Padding = Styles.ButtonPadding,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ColumnDefinitions = {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { }
                },
                RowDefinitions = {
                    new RowDefinition {Height = Styles.MenuHeight },
                },
                ColumnSpacing = (!Styles.FixedGap) ? Styles.FontSize : Styles.IconGap
            };
            gridLayout.Children.Add(iconImage, 0, 0);
            gridLayout.Children.Add(textLabel, 1, 0);

            View = gridLayout;

//            updateControls();
        }
        /*protected override void OnBindingContextChanged()
        {
            updateControls();
            base.OnBindingContextChanged();
        }*/
    }
}