using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Windows.Forms;
using ConfigTray.Win32Api;

namespace ConfigTray
{
    public static class Extensions
    {
        /// <summary>
        /// Get the absolute XPath to a given XElement, including the namespace.
        /// (e.g. "/a:people/b:person[6]/c:name[1]/d:last[1]").
        /// </summary>
        public static string GetAbsoluteXPath(this XObject node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("element");
            }

            Func<XElement, string> relativeXPath = e => {
                int index = e.IndexPosition();

                var currentNamespace = e.Name.Namespace;

                string name;
                if (currentNamespace == null)
                {
                    name = e.Name.LocalName;
                }
                else
                {
                    string namespacePrefix = e.GetPrefixOfNamespace(currentNamespace) ?? "ns";
                    name = string.IsNullOrEmpty(namespacePrefix)
                        ? e.Name.LocalName
                        : String.Format("{0}:{1}", namespacePrefix, e.Name.LocalName);
                }

                // If the element is the root, no index is required
                return (index == -1) ? "/" + name : string.Format
                (
                    "/{0}[{1}]",
                    name,
                    index.ToString()
                );
            };

            IEnumerable<string> ancestors = null;
            if (node.NodeType == XmlNodeType.Attribute)
            {
                XAttribute att = (XAttribute)node;
                string attributeSuffix = "/@" + att.Name;
                ancestors = from e in att.Parent.Ancestors()
                            select relativeXPath(e);
                return string.Concat(ancestors.Reverse().ToArray()) +
                   relativeXPath(att.Parent) + attributeSuffix;
            }
            else
            {
                var element = (XElement)node;
                ancestors = from e in element.Ancestors()
                            select relativeXPath(e);
                return string.Concat(ancestors.Reverse().ToArray()) +
                   relativeXPath(element);
            }


        }

        /// <summary>
        /// Get the index of the given XElement relative to its
        /// siblings with identical names. If the given element is
        /// the root, -1 is returned.
        /// </summary>
        /// <param name="element">
        /// The element to get the index of.
        /// </param>
        public static int IndexPosition(this XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (element.Parent == null)
            {
                return -1;
            }

            int i = 1; // Indexes for nodes start at 1, not 0

            foreach (var sibling in element.Parent.Elements(element.Name))
            {
                if (sibling == element)
                {
                    return i;
                }

                i++;
            }

            throw new InvalidOperationException
                ("element has been removed from its parent.");
        }

        public static void RemoveIcon(this NotifyIcon icon)
        {
            NotifyIconData data = NotifyIconData.CreateDefault(icon.Icon.Handle);
            Win32Api.Win32Api.Shell_NotifyIcon(Win32Api.NotifyCommand.Delete, ref data);
        }
    }
}
