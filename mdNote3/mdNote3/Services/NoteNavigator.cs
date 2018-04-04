using System;
using System.Collections.Generic;
using System.Text;

namespace mdOrganizer.Services
{
    public class NoteNavigator
    {
        public static string FileName { get; set; }
        public static string FilePath { get; set; }
        public static Markdown.Document Document { get; private set; } = new Markdown.Document();


        //Create empty document
        public static void Reset()
        {
            CurrentPath.Clear();
            Document.Clear();
            FileName = "untitled.md";
            FilePath = null;
        }

        public static List<int> CurrentPath { get; private set; } = new List<int>();
        public static Markdown.Item CurrentNote
        {
            get
            {
                if (Document == null) return null;
                Markdown.Item result = Document.Root;
                foreach (int i in CurrentPath)
                {
                    result = result.Childs[i];
                }
                return result;
            }
        }

        public static void BuildPathToNote(Markdown.Item newCurrentNote)
        {
            CurrentPath.Clear();
            while (newCurrentNote.Parent != null)
            {
                int i = newCurrentNote.Parent.Childs.IndexOf(newCurrentNote);
                if (i >= 0)
                    CurrentPath.Insert(0, i);
                else break;
                newCurrentNote = newCurrentNote.Parent;
            }
        }
        public static void GoBack(int stepsCount = 1)
        {
            for (int i = 0; i < stepsCount; i++)
            {
                CurrentPath.RemoveAt(NoteNavigator.CurrentPath.Count - 1);
                if (NoteNavigator.CurrentPath.Count == 0) break;
            }
        }
        public static bool CanGoBack
        {
            get { return CurrentPath.Count > 0; }
        }

        public static string NoteHtmlForm
        {
            get
            {
                if (CurrentNote == null) return "";
                return HtmlGenerator.Get(CurrentNote);
            }
        }
        public static string NoteTitle
        {
            get
            {
                return (CurrentPath.Count > 0) ? CurrentNote.Title : FileName;
            }
        }
    }
}
