using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace mdOrganizer.UI
{
    public class IconMenuTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SeparatorTemplate { get; set; }
        public DataTemplate CommandTemplate { get; set; }
        public DataTemplate CustomTemplate { get; set; }

        public IconMenuTemplateSelector()
        {
            CommandTemplate = new DataTemplate(()=> {
                IconButton btn = new IconButton();
//                btn.SetBinding(IconButton.IconProperty, "Icon");
                return btn;
            });

            SeparatorTemplate = new DataTemplate(() =>
            {
                return new ViewCell
                {
                    View = new BoxView
                    {
                        HeightRequest = Styles.SeparatorHeight,
                        BackgroundColor = Styles.SeparatorColor,
                        Margin = Styles.SeparatorMargin
                    }
                };
            });

        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item != null)
            {
                switch (((IconMenuItem)item).Kind)
                {
                    case IconMenuItemKind.Command:
                        return CommandTemplate;
                    case IconMenuItemKind.Separator:
                        return SeparatorTemplate;
                    case IconMenuItemKind.Custom:
                        return CustomTemplate;
                }
            }
            return null;
        }
    }
}
