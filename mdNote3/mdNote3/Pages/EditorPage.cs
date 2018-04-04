using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mdOrganizer.Services;
using Xamarin.Forms;

namespace mdOrganizer.Pages
{
    public class EditorPage : ContentPage
    {
        private static EditorPage _editor = null;
        public static EditorPage Editor
        {
            get
            {
                if (_editor == null) _editor = new EditorPage();
                return _editor;
            }
        }

        private Picker noteCategory;
        private Entry noteTitle;
        private Editor noteContent;

        private Markdown.Item editingNote;

        private List<Markdown.Item> parents;

        private Action onClose = null;

        public string UnsortedHeader = null;
        private Markdown.Item unsortedCategory = null;

        private void fillChilds(Markdown.Item startingPoint)
        {
            if (startingPoint == editingNote) return;
            if (UnsortedHeader != null)
            {
                if (startingPoint.Title.Equals(UnsortedHeader))
                    unsortedCategory = startingPoint;
            }
            parents.Add(startingPoint);

            foreach (Markdown.Item item in startingPoint.Childs)
            {
                fillChilds(item);
            }
        }

        public void ScanParents()
        {
            noteCategory.ItemsSource = null;
            parents.Clear();
            unsortedCategory = null;
            fillChilds(NoteNavigator.Document.Root);

            if ((unsortedCategory == null) && (UnsortedHeader != null))
            {
                unsortedCategory = NoteNavigator.Document.AddContent(NoteNavigator.Document.Root, UnsortedHeader, "");
                parents.Add(unsortedCategory);
            }
            noteCategory.ItemsSource = parents;
        }

        public EditorPage()
        {
            parents = new List<Markdown.Item>();

            noteCategory = new Picker()
            {
                Title = Strings.Loc.EditorPage_ParentCategory,
                Margin = new Thickness(5, 5, 5, 5),
                ItemsSource = parents,
                ItemDisplayBinding = new Binding("RawTitle")
            };
            noteTitle = new Entry()
            {
                Placeholder = Strings.Loc.EditorPage_TitleLabel,
                Margin = new Thickness(5, 5, 5, 5)
            };
            noteContent = new Editor()
            {
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            Content = new StackLayout
            {
                Children = {
                    noteCategory,
                    noteTitle,
                    new ScrollView() {
                        Margin = new Thickness(5, 5, 5, 5),
                        Content =  noteContent,
                        VerticalOptions = LayoutOptions.FillAndExpand
                    }
                }
            };

            ToolbarItem okBtn = new ToolbarItem()
            {
                Icon = "ok30.png",
                Text = Strings.Loc.EditorPage_SaveBtn,
                Order = ToolbarItemOrder.Primary,
                Command = new Command(new Action(SaveNote))
            };

            ToolbarItems.Add(okBtn);
            Title = Strings.Loc.EditorPage_EditTitle;
        }

        public void AddNote(Markdown.Item parent, Action onCloseAction, string defaultCaption = "", string defaultContent = "", string unsortedHeader = null)
        {
            onClose = onCloseAction;
            editingNote = new Markdown.Item();
            editingNote.Content = defaultContent;
            editingNote.Title = defaultCaption;
            UnsortedHeader = unsortedHeader;
            ScanParents();
            if (parent == null)
            {
                if (unsortedHeader != null)
                    parent = unsortedCategory;
                else
                    parent = NoteNavigator.Document.Root;
            }
            noteCategory.SelectedItem = parent;
            noteTitle.Text = editingNote.Title;
            noteContent.Text = editingNote.Content;

            noteTitle.IsVisible = true;
            noteCategory.IsVisible = true;
            Title = Strings.Loc.EditorPage_AddTitle;
        }

        public void EditNote(Markdown.Item note, Action onCloseAction)
        {
            onClose = onCloseAction;
            editingNote = note;
            ScanParents();

            noteCategory.SelectedItem = editingNote.Parent;
            noteTitle.Text = editingNote.Title;
            noteContent.Text = editingNote.Content;

            noteTitle.IsVisible = editingNote.Parent != null;
            noteCategory.IsVisible = editingNote.Parent != null;

            Title = Strings.Loc.EditorPage_EditTitle;
        }

        public async void SaveNote()
        {
            //TODO Validation
            if ((editingNote.Parent != null) || (editingNote != NoteNavigator.Document.Root))
            {
                if (editingNote.Parent != noteCategory.SelectedItem)
                    NoteNavigator.Document.ChangeParent(editingNote, (Markdown.Item)noteCategory.SelectedItem);
                editingNote.Title = noteTitle.Text;
            }
            NoteNavigator.Document.UpdateContent(editingNote, noteContent.Text);
            NoteNavigator.BuildPathToNote(editingNote);

            await DeviceServices.SaveDocumentAsync();

            await Navigation.PopAsync(true);
            onClose();
        }

        protected override void OnDisappearing()
        {
            editingNote = null;
            parents.Clear();
            base.OnDisappearing();
            onClose();
        }
    }
}