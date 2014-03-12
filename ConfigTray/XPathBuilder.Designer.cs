namespace ConfigTray
{
    partial class XPathBuilder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XPathBuilder));
            this.XDocTree = new System.Windows.Forms.TreeView();
            this.XmlDocumentOFD = new System.Windows.Forms.OpenFileDialog();
            this.XPathView = new System.Windows.Forms.TextBox();
            this.OpenFileButton = new System.Windows.Forms.Button();
            this.Go = new System.Windows.Forms.Button();
            this.XmlFile = new System.Windows.Forms.Label();
            this.LoadPanel = new ConfigTray.Controls.TransparentPanel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.LoadPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // XDocTree
            // 
            this.XDocTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.XDocTree.Location = new System.Drawing.Point(12, 67);
            this.XDocTree.Name = "XDocTree";
            this.XDocTree.Size = new System.Drawing.Size(483, 489);
            this.XDocTree.TabIndex = 0;
            this.XDocTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.XDocTree_NodeMouseClick);
            // 
            // XmlDocumentOFD
            // 
            this.XmlDocumentOFD.DefaultExt = "xml";
            this.XmlDocumentOFD.Filter = "Xml Files|*.xml|Config Files|*.config|All Files|*.*";
            // 
            // XPathView
            // 
            this.XPathView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.XPathView.Location = new System.Drawing.Point(12, 41);
            this.XPathView.Name = "XPathView";
            this.XPathView.Size = new System.Drawing.Size(483, 20);
            this.XPathView.TabIndex = 1;
            // 
            // OpenFileButton
            // 
            this.OpenFileButton.Location = new System.Drawing.Point(12, 12);
            this.OpenFileButton.Name = "OpenFileButton";
            this.OpenFileButton.Size = new System.Drawing.Size(87, 23);
            this.OpenFileButton.TabIndex = 2;
            this.OpenFileButton.Text = "Open Xml File";
            this.OpenFileButton.UseVisualStyleBackColor = true;
            this.OpenFileButton.Click += new System.EventHandler(this.OpenFileButton_Click);
            // 
            // Go
            // 
            this.Go.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Go.Location = new System.Drawing.Point(420, 12);
            this.Go.Name = "Go";
            this.Go.Size = new System.Drawing.Size(75, 23);
            this.Go.TabIndex = 3;
            this.Go.Text = "Use XPath";
            this.Go.UseVisualStyleBackColor = true;
            this.Go.Click += new System.EventHandler(this.Go_Click);
            // 
            // XmlFile
            // 
            this.XmlFile.AutoSize = true;
            this.XmlFile.Location = new System.Drawing.Point(105, 17);
            this.XmlFile.Name = "XmlFile";
            this.XmlFile.Size = new System.Drawing.Size(10, 13);
            this.XmlFile.TabIndex = 4;
            this.XmlFile.Text = " ";
            // 
            // LoadPanel
            // 
            this.LoadPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(172)))), ((int)(((byte)(172)))), ((int)(((byte)(250)))));
            this.LoadPanel.Controls.Add(this.progressBar1);
            this.LoadPanel.Location = new System.Drawing.Point(12, 67);
            this.LoadPanel.Name = "LoadPanel";
            this.LoadPanel.Size = new System.Drawing.Size(483, 489);
            this.LoadPanel.TabIndex = 5;
            this.LoadPanel.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(3, 202);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(477, 54);
            this.progressBar1.Step = 20;
            this.progressBar1.TabIndex = 0;
            // 
            // XPathBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 568);
            this.Controls.Add(this.LoadPanel);
            this.Controls.Add(this.XmlFile);
            this.Controls.Add(this.Go);
            this.Controls.Add(this.OpenFileButton);
            this.Controls.Add(this.XPathView);
            this.Controls.Add(this.XDocTree);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "XPathBuilder";
            this.Text = "XPathBuilder";
            this.LoadPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView XDocTree;
        private System.Windows.Forms.OpenFileDialog XmlDocumentOFD;
        private System.Windows.Forms.TextBox XPathView;
        private System.Windows.Forms.Button OpenFileButton;
        private System.Windows.Forms.Button Go;
        private System.Windows.Forms.Label XmlFile;
        private Controls.TransparentPanel LoadPanel;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}