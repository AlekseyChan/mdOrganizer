using System;
using System.Collections.Generic;
using System.Text;

namespace mdOrganizer.Markdown
{
    public class Item
    {
        public string RawTitle
        {
            get
            {
                string title = "";
                if (Level > 0)
                {
                    for (int i = 0; i < Level; i++)
                        title += "#";
                    title += " ";
                }
                else
                    title = "Оглавление";
                return title + Title;
            }
        }

        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string HtmlForm { get; set; } = "";
        public int Level { get; set; } = 0;

        public bool IsMarked { get; set; } = false;

        public List<Item> Childs { get; set; } = new List<Item>();
        public Item Parent { get; set; } = null;

        public void MarkWithChilds(bool isMarked)
        {
            IsMarked = isMarked;
            foreach (Item child in Childs)
                child.MarkWithChilds(isMarked);
        }

        public void MarkWithParents(bool isMarked)
        {
            IsMarked = isMarked;
            if (Parent!=null)
                Parent.MarkWithParents(isMarked);
        }

        public override string ToString()
        {
            StringBuilder contentBuilder = new StringBuilder();
            if (Level > 0)
            {
                contentBuilder.AppendLine(RawTitle);
            }
            if (!String.IsNullOrEmpty(Content))
            {
                contentBuilder.AppendLine(Content);
            }

            foreach (Item block in Childs)
            {
                block.Level = Level + 1;
                contentBuilder.Append(block.ToString());
            }
            return contentBuilder.ToString();
        }
    }
}
