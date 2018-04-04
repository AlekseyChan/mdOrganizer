using System;
using System.Collections.Generic;
using System.Text;


namespace mdOrganizer.Markdown
{
    public class Document
    {
        private Markdown.Converter converter = new Markdown.Converter();

        public void UpdateContent(Item item, string value, bool updateHtmlForm = true)
        {
            converter.AutoNewLines = true;
            item.Content = value;
            if (updateHtmlForm)
                item.HtmlForm = converter.Transform(value);
        }

        public Item Root { get; private set; } = null;

        public void Parse(System.IO.TextReader reader)
        {
            Root = new Item();

            StringBuilder contentBuilder = new StringBuilder();

            Item parent = null;
            Item current = Root;

            while (true)
            {
                string rawString = reader.ReadLine();

                if (rawString == null)
                {
                    UpdateContent(current, contentBuilder.ToString());
                    break;
                }

                string line = rawString.Trim();

                int level = 0;
                while (line.StartsWith("#"))
                {
                    line = line.Remove(0, 1);
                    level++;
                }

                if (level > 0)
                {
                    UpdateContent(current, contentBuilder.ToString());
                    contentBuilder.Clear();
                    if (current.Level < level)
                    {
                        //опускаемся на уровень вниз
                        parent = current;
                        level = parent.Level + 1;
                    }
                    else
                    {
                        //на том же уровне еще одного 
                        parent = current.Parent;
                        while ((parent != null) && (parent.Level >= level))
                        {
                            //поднимаемся на уровень вверх
                            parent = parent.Parent;
                        }
                    }

                    current = new Item()
                    {
                        Title = line.TrimStart(),
                        Level = level,
                        Parent = parent
                    };
                    if (parent != null)
                    {
                        parent.Childs.Add(current);
                    }
                }
                else
                {
                    if (contentBuilder.Length > 0)
                        contentBuilder.AppendLine();

                    contentBuilder.Append(rawString);
                }
            }

        }

        private void releaseChilds(Markdown.Item parent)
        {
            foreach (Markdown.Item child in parent.Childs)
            {
                releaseChilds(child);
            }
            parent.Childs.Clear();
        }

        public void Clear()
        {
            if (Root != null)
                releaseChilds(Root);
        }

        public Document()
        {
        }

        public Document(string markdownString)
        {
            Parse(new System.IO.StringReader(markdownString));
        }

        public Document(System.IO.Stream stream)
        {
            Parse(new System.IO.StreamReader(stream));
        }

        public override string ToString()
        {
            return Root.ToString();
        }

        public Item AddContent(Item parent, string title, string content)
        {
            if (parent == null)
                parent = Root;
            Item newBlock = new Item()
            {
                Level = parent.Level + 1,
                Title = title,
                Parent = parent
            };
            UpdateContent(newBlock, content);
            parent.Childs.Add(newBlock);
            return newBlock;
        }

        public void RemoveContent(Item block)
        {
            Item parent = block.Parent;
            parent.Childs.Remove(block);
            block.Parent = null;
        }

        public void ChangeParent(Item item, Item newParent)
        {
            Item parent = item.Parent;
            if (parent != null)
                parent.Childs.Remove(item);
            item.Parent = newParent;
            newParent.Childs.Add(item);
        }

        private void findString(string searchString, Item startItem)
        {
            searchString = searchString.ToLower();
            if (startItem.Title.ToLower().Contains(searchString) || startItem.Content.ToLower().Contains(searchString))
            {
                startItem.MarkWithParents(true);
            }
            else
            {
                startItem.IsMarked = false;
            }
            foreach (Item child in startItem.Childs)
            {
                findString(searchString, child);
            }
        }

        public void FindString(string searchString, Item startItem = null)
        {
            if (Root == null) return;
            if (startItem == null) startItem = Root;
            if (String.IsNullOrEmpty(searchString))
            {
                startItem.MarkWithChilds(false);
                return;
            }
            findString(searchString, startItem);
        }
    }
}
