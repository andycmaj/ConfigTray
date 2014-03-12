using System;
using System.Windows.Forms;
using ConfigTray.Configuration;
using System.Linq;
using System.Drawing;
using NLog;

namespace ConfigTray
{
    public partial class EditConfig : Form
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();
        public ConfigTrayConfiguration ModifiedConfig { get; private set; }
        private ListViewItem _itemUnderDrag;

        public EditConfig(ConfigTrayConfiguration config)
        {
            ModifiedConfig = config;
            InitializeComponent();

            LoadListItems();
        }

        private void LoadListItems()
        {
            foreach (Setting setting in ModifiedConfig.Settings)
            {
                ListViewItem settingItem = new ListViewItem(setting.Name) {
                    Tag = setting
                };
                SettingsList.Items.Add(settingItem);
            }

            foreach (ConfigFile file in ModifiedConfig.Files)
            {
                ListViewItem fileItem = new ListViewItem(file.ToString()) {
                    Tag = file
                };
                FilesList.Items.Add(fileItem);
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #region Setting Menu

        private void AddChoiceSetting_Click(object sender, EventArgs e)
        {
            ChoiceSetting newSetting = new ChoiceSetting {
                Name = "NewChoiceSetting"
            };

            AddSetting(newSetting);
        }

        private void AddToggleSetting_Click(object sender, EventArgs e)
        {
            ToggleSetting newSetting = new ToggleSetting {
                Name = "NewToggleSetting"
            };

            AddSetting(newSetting);
        }

        private void AddLiteralSetting_Click(object sender, EventArgs e)
        {
            LiteralSetting newSetting = new LiteralSetting {
                Name = "NewLiteralSetting"
            };

            AddSetting(newSetting);
        }

        private void AddSetting(Setting newSetting)
        {
            ModifiedConfig.Settings.Add(newSetting);

            ListViewItem settingItem = new ListViewItem(newSetting.Name) {
                Tag = newSetting
            };
            SettingsList.Items.Add(settingItem);

            SettingsList.SelectedIndices.Clear();
            SettingsList.SelectedIndices.Add(SettingsList.Items.Count - 1);
        }

        private void DeleteSetting_Click(object sender, EventArgs e)
        {
            if (SettingsList.SelectedItems.Count == 0)
            {
                return;
            }

            Setting selectedSetting = SettingsList.SelectedItems[0].Tag as Setting;
            if (selectedSetting == null)
            {
                return;
            }

            string confirmText = string.Format("Delete selected setting: {0}", selectedSetting.Name);
            DialogResult result = MessageBox.Show(confirmText, "Confirm delete", MessageBoxButtons.OKCancel);
            if (result == DialogResult.Cancel)
            {
                return;
            }

            //Remove setting from all files that reference it.
            foreach (ConfigFile file in ModifiedConfig.Files)
            {
                Setting settingInFile = file.Settings.SingleOrDefault(setting => setting.Name == selectedSetting.Name);
                if (settingInFile != null)
                {
                    file.Settings.Remove(settingInFile);
                    file.SettingNames.Remove(settingInFile.Name);
                }
            }

            //Remove setting from settings.
            SettingsList.Items.Remove(SettingsList.SelectedItems[0]);
            ModifiedConfig.Settings.Remove(selectedSetting);
        }

        private void AddFile_Click(object sender, EventArgs e)
        {
            ConfigFile newFile = new ConfigFile {
                Name = "NewFile"
            };
            ModifiedConfig.Files.Add(newFile);

            ListViewItem fileItem = new ListViewItem(newFile.Name) {
                Tag = newFile
            };
            FilesList.Items.Add(fileItem);

            FilesList.SelectedIndices.Clear();
            FilesList.SelectedIndices.Add(FilesList.Items.Count - 1);
        }

        private void DeleteFile_Click(object sender, EventArgs e)
        {
            if (FilesList.SelectedItems.Count == 0)
            {
                return;
            }

            ConfigFile selectedFile = FilesList.SelectedItems[0].Tag as ConfigFile;
            if (selectedFile == null)
            {
                return;
            }

            string confirmText = string.Format("Delete selected file: {0}", selectedFile.Name);
            DialogResult result = MessageBox.Show(confirmText, "Confirm delete", MessageBoxButtons.OKCancel);
            if (result == DialogResult.Cancel)
            {
                return;
            }

            //Remove file.
            FilesList.Items.Remove(FilesList.SelectedItems[0]);
            ModifiedConfig.Files.Remove(selectedFile);
        }

        #endregion

        #region Drag and Drop

        private void SettingsList_MouseDown(object sender, MouseEventArgs e)
        {
            Setting selectedSetting;
            ListViewItem selectedItem = SettingsList.GetItemAt(e.X, e.Y);
            if (selectedItem == null)
            {
                return;
            }

            selectedSetting = selectedItem.Tag as Setting;
            if (selectedSetting == null)
            {
                return;
            }

            DoDragDrop(selectedSetting, DragDropEffects.Link);
        }

        private void FilesList_DragEnter(object sender, DragEventArgs e)
        {
            if ((e.AllowedEffect & DragDropEffects.Link) != 0 && e.Data.GetDataPresent(DataFormats.Serializable))
            {
                e.Effect = DragDropEffects.Link;
            }
        }

        private void FilesList_DragDrop(object sender, DragEventArgs e)
        {
            if (_itemUnderDrag == null)
            {
                return;
            }

            ConfigFile targetFile = _itemUnderDrag.Tag as ConfigFile;
            if (targetFile == null)
            {
                return;
            }

            try
            {
                Setting settingToAdd = (Setting)e.Data.GetData(DataFormats.Serializable);

                MessageBox.Show(string.Format("Adding \"{0}\" to \"{1}\"", settingToAdd, targetFile.Name), "Link Settings", MessageBoxButtons.OKCancel);

                targetFile.Settings.Add(settingToAdd);
                targetFile.SettingNames.Add(settingToAdd.Name);
                _itemUnderDrag.ForeColor = Color.ForestGreen;
                _itemUnderDrag.Font = new Font(_itemUnderDrag.Font, FontStyle.Bold);

                ListViewItem item = SettingsList.SelectedItems[0];

                using (Graphics g = SettingsList.CreateGraphics())
                {
                    DrawLink(item, _itemUnderDrag, g);
                }
            }
            catch (Exception ex)
            {
                s_logger.ErrorException("Error linking setting", ex);
                MessageBox.Show(ex.Message, "Error adding setting.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilesList_DragOver(object sender, DragEventArgs e)
        {
            Point p = FilesList.PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y));

            //Set X coordinate for item fetch so that we can target an item even if mouse is not directly over text.
            int x = FilesList.Location.X + 10;
            ListViewItem instantaneousItem = FilesList.GetItemAt(x, p.Y);
            if (instantaneousItem == null || _itemUnderDrag == instantaneousItem)
            {
                return;
            }

            _itemUnderDrag = instantaneousItem;

            using (Graphics g = FilesList.CreateGraphics())
            {
                FilesList.Refresh();
                g.DrawRectangle(Pens.BlueViolet, _itemUnderDrag.Bounds);
            }
        }

        #endregion

        #region PropertyGrid Selected Object State

        private void SettingsList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.IsSelected)
            {
                Setting unselectedSetting = (Setting)e.Item.Tag;
                if (e.Item.Text != unselectedSetting.Name)
                {
                    //Refresh Setting name if changed.
                    e.Item.Text = unselectedSetting.Name;
                }
                return;
            }

            SetSelectedSettingItem(e.Item);
        }

        private void FilesList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.IsSelected)
            {
                string fileString = ((ConfigFile)e.Item.Tag).ToString();
                if (e.Item.Text != fileString)
                {
                    //Refresh File name if changed.
                    e.Item.Text = fileString;
                }
                return;
            }

            SetSelectedFileItem(e.Item);
        }

        private void SettingsList_Enter(object sender, EventArgs e)
        {
            Point p = SettingsList.PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y));
            ListViewItem item = SettingsList.GetItemAt(p.X, p.Y);

            if (SettingsList.SelectedItems.Count != 1)
            {
                return;
            }

            //If entering on already selected item, selectchange handler wont fire, so set here.
            if (item == SettingsList.SelectedItems[0])
            {
                SetSelectedSettingItem(item);
            }
        }

        private void FilesList_Enter(object sender, EventArgs e)
        {
            Point p = FilesList.PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y));
            ListViewItem item = FilesList.GetItemAt(p.X, p.Y);

            if (FilesList.SelectedItems.Count != 1)
            {
                return;
            }

            //If entering on already selected item, selectchange handler wont fire, so set here.
            if (item == FilesList.SelectedItems[0])
            {
                SetSelectedFileItem(item);
            }
        }

        #endregion

        private void SetSelectedSettingItem(ListViewItem item)
        {
            Setting setting = item.Tag as Setting;
            if (setting == null || SettingGrid.SelectedObject == setting)
            {
                return;
            }

            SettingGrid.SelectedObject = setting;

            using (Graphics g = SettingsList.CreateGraphics())
            {
                SettingsList.Refresh();

                //Highlight files that reference this setting.
                foreach (ListViewItem fileItem in FilesList.Items)
                {
                    ConfigFile file = fileItem.Tag as ConfigFile;

                    if (file.SettingNames.Contains(setting.Name))
                    {
                        fileItem.ForeColor = Color.ForestGreen;
                        fileItem.Font = new Font(fileItem.Font, FontStyle.Bold);

                        g.DrawRectangle(Pens.ForestGreen, item.Bounds);
                        DrawLink(item, fileItem, g);
                    }
                    else
                    {
                        fileItem.ForeColor = Color.Black;
                        fileItem.Font = new Font(fileItem.Font, FontStyle.Regular);
                    }
                }
            }
        }

        private void SetSelectedFileItem(ListViewItem item)
        {
            ConfigFile file = item.Tag as ConfigFile;
            if (file == null || SettingGrid.SelectedObject == file)
            {
                return;
            }

            SettingGrid.SelectedObject = file;
        }

        private void DrawLink(ListViewItem settingItem, ListViewItem fileItem, Graphics g)
        {
            float segment1left = settingItem.Bounds.Right;
            float segment1right = settingItem.Position.X + settingItem.Bounds.Width + 102;
            float segment1y = settingItem.Position.Y + (settingItem.Bounds.Height / 2);

            float segment2x = settingItem.Position.X + settingItem.Bounds.Width + 100;
            float segment2yStart = fileItem.Position.Y + (fileItem.Bounds.Height / 2);
            float segment2yEnd = settingItem.Position.Y + (settingItem.Bounds.Height / 2);

            float segment3left = settingItem.Position.X + settingItem.Bounds.Width + 99;
            float segment3right = SettingsList.Left + SettingsList.Width;
            float segment3y = fileItem.Position.Y + (fileItem.Bounds.Height / 2);

            using (Pen pen = new Pen(Color.ForestGreen, 3))
            {
                // 1. Draw horizontal line from selected setting to dX
                g.DrawLine(pen,
                    segment1left,
                    segment1y,
                    segment1right,
                    segment1y);
                // 2. Draw vertical line from yHighestFile to yLowestFile perpendicular to dX and passing through 1.y
                g.DrawLine(pen,
                    segment2x,
                    segment2yStart,
                    segment2x,
                    segment2yEnd);
                // 3. Draw horizontal lines from dX to each file item.
                g.DrawLine(pen,
                    segment3left,
                    segment3y,
                    segment3right,
                    segment3y);
            }
        }

        private void SettingGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.PropertyDescriptor.Name != "ValueXPath")
            {
                return;
            }

            string newValue = (string)e.ChangedItem.Value;
            if (!newValue.Contains('|'))
            {
                return;
            }

            string[] discreteValues = newValue.Split(new char[1] { '|' });
            if (discreteValues.Length != 2)
            {
                throw new ArgumentOutOfRangeException("Wrong amount of values returned by XPath Editor ");
            }

            Setting selectedSetting = SettingGrid.SelectedObject as Setting;
            selectedSetting.ValueXPath = discreteValues[0];
            selectedSetting.XmlNamespace = discreteValues[1];

            SettingGrid.Refresh();
        }
    }
}
