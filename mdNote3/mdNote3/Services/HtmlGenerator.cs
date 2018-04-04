using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace mdOrganizer.Services
{
    public class HtmlGenerator
    {
        private static string rootTemplate = null;
        private static string noteTemplate = null;
        private static string markedTemplate = null;
        private static string themeFile = null;

        public static string ThemeFile
        {
            get
            {
                if (themeFile == null) themeFile = loadResource("platform.css");
                return themeFile;
            }
        }

        public static string RootTemplate
        {
            get
            {
                if (rootTemplate == null) rootTemplate = loadResource("Templates.BasicHtml.html");
                return rootTemplate;
            }
        }

        public static string NoteTemplate
        {
            get
            {
                if (noteTemplate == null) noteTemplate = loadResource("Templates.NoteHtml.html");
                return noteTemplate;
            }
        }

        public static string MarkedTemplate
        {
            get
            {
                if (markedTemplate == null) markedTemplate = loadResource("Templates.MarkedNote.html");
                return markedTemplate;
            }
        }

        private static string loadResource(string resource)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            System.IO.Stream stream = assembly.GetManifestResourceStream(DeviceServices.BaseResource + "." + resource);
            using (var reader = new System.IO.StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private static string buildTitle(Markdown.Item item)
        {
            if (item == null) return "";
            return (item.Title.Length > 0 ? item.Title : Strings.Loc.NotesIndex);
        }

        private static string buildParentsPath(Markdown.Item startItem, int i)
        {
            if (startItem.Parent != null)
            {
                string parents = buildParentsPath(startItem.Parent, i + 1);
                if (parents.Length > 0) parents += "&nbsp;/&nbsp;";
                return parents + "<a href='parent_" + i.ToString() + "'>" +
                    buildTitle(startItem.Parent) +
                    "</a>";
            }
            return "";
        }

        public static string Get(Markdown.Item item)
        {
            string childStr = "";
            for (int i = 0; i < item.Childs.Count; i++)
            {
                Markdown.Item child = item.Childs[i];
                string childTemplate = child.IsMarked ? MarkedTemplate : NoteTemplate;
                childStr += childTemplate.Replace("{NoteTitle}", child.Title).Replace("{NoteId}", i.ToString()).Replace("{NoteContent}", Settings.PreviewMode ? child.HtmlForm : "");
                
            }

            string pathStr = buildParentsPath(item, 1);

            return RootTemplate.
                Replace("{Theme}", ThemeFile).Replace("{BaseSize}", Settings.BaseFontSize.ToString()).
                Replace("{NoteTitle}", item.Title).
                Replace("{NoteContent}", item.HtmlForm).
                Replace("{NoteChilds}", childStr).
                Replace("{Parent}", pathStr);
        }
    }
}
