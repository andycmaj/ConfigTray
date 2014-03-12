using System;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Xml.XPath;

namespace ConfigTray
{
    public partial class XPathBuilder : Form
    {
        private XDocument _xDoc;
        private BackgroundWorker _worker;
        private static int _index;

        public string SelectedXPath { get; private set; }
        public string SelectedNamespace { get; private set; }

        public XPathBuilder()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (string.IsNullOrEmpty(SelectedXPath))
            {
                //e.Cancel = true;
            }
        }

        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            DialogResult result = XmlDocumentOFD.ShowDialog();
            if (result == DialogResult.OK)
            {
                LoadPanel.Visible = true;
                _index = 0;
                _worker = new BackgroundWorker {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = false
                };
                _worker.ProgressChanged += OnWorkerProgressChanged;
                _worker.RunWorkerCompleted += OnWorkerCompleted;
                _worker.DoWork += DoWork;
                _worker.RunWorkerAsync(XmlDocumentOFD.FileName);
            }
        }

        void OnWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void OnWorkerCompleted(object s, RunWorkerCompletedEventArgs args)
        {
            if (args.Error != null)
            {
                throw args.Error;
            }

            TreeNode root = args.Result as TreeNode;
            XDocTree.Nodes.Clear();
            XDocTree.Nodes.Add(root);

            LoadPanel.Visible = false;
            XmlFile.Text = Path.GetFileName(XmlDocumentOFD.FileName);
        }

        private void DoWork(object s, DoWorkEventArgs args)
        {
            string filePath = args.Argument as string;
            _xDoc = XDocument.Load(filePath);

            //Get default namespace.
            XPathNavigator nav = _xDoc.CreateNavigator();
            nav.MoveToFollowing(XPathNodeType.Element);
            IDictionary<string, string> namespaces = nav.GetNamespacesInScope(XmlNamespaceScope.All);
            var nsEntry = namespaces.SingleOrDefault(entry => string.IsNullOrEmpty(entry.Key) && !string.IsNullOrEmpty(entry.Value));
            SelectedNamespace = nsEntry.Value;

            int totalElements = _xDoc.Descendants().Count();
            TreeNode rootTreeNode = new TreeNode();
            TransposeNodes(_xDoc.Root, rootTreeNode, totalElements);
            args.Result = rootTreeNode;
        }

        private void TransposeNodes(XElement xElement, TreeNode treeNode, int totalElements)
        {
            GetAttributes(xElement, treeNode);
            treeNode.Text = FormatName(xElement);
            treeNode.Tag = xElement.GetAbsoluteXPath();

            float percentComplete = (_index++ / (float)totalElements) * 100;
            _worker.ReportProgress((int)percentComplete);

            foreach (XElement child in xElement.Elements())
            {
                TreeNode childTreeNode = new TreeNode();
                treeNode.Nodes.Add(childTreeNode);
                TransposeNodes(child, childTreeNode, totalElements);
            }
        }

        private static string FormatName(XElement node)
        {
            string fullName = "<" + node.Name;

            if (node.Nodes().Count() == 1 && node.Nodes().Single().NodeType == XmlNodeType.Text)
                fullName += string.Format(">{0}</{1}>", node.Value, node.Name);
            else
                fullName += ">";

            return fullName;
        }

        private static void GetAttributes(XElement node, TreeNode treeNode)
        {
            if (node.Attributes() != null)
            {
                IEnumerable<XAttribute> atts = node.Attributes();
                ListViewItem[] attributes = new ListViewItem[atts.Count()];

                int i = 0;
                foreach (XAttribute att in atts)
                {
                    ListViewItem item = new ListViewItem(new string[] { att.Name.ToString(), att.Value });
                    attributes[i++] = item;

                    TreeNode attributeNode = new TreeNode(string.Format("[{0}] = {1}", att.Name.ToString(), att.Value)) {
                        Tag = att.GetAbsoluteXPath()
                    };
                    treeNode.Nodes.Add(attributeNode);
                }

                treeNode.Tag = attributes;
            }
        }

        private void XDocTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode clicked = e.Node;
            XPathView.Text = (string)clicked.Tag;
        }

        private void Go_Click(object sender, EventArgs e)
        {
            //TODO: Create cancel option. and validate xpath exists.
            SelectedXPath = string.Format("{0}|{1}", XPathView.Text, SelectedNamespace);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
