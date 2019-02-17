using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System.Linq;
using System.Drawing;
using QuickVersion.Properties;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace QuickVersion {

    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    [COMServerAssociation(AssociationType.Directory)]
    public class ContextMenu : SharpContextMenu
    {
        private ContextMenuStrip menu = new ContextMenuStrip();

        private List<string> selectedVersionableFiles = new List<string>();

        protected override bool CanShowMenu() {
           
            if (SelectedItemPaths.Count() < 1) {
                return false;
            }

            selectedVersionableFiles.Clear();

            foreach (string selectedItemPath in SelectedItemPaths) {
                FileAttributes attr = File.GetAttributes(selectedItemPath);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory) {
                    continue;
                }
                selectedVersionableFiles.Add(selectedItemPath);
            }

            if (selectedVersionableFiles.Count < 1) {
                return false;
            }

            return true;
        }

        private void UpdateMenu() {
            menu.Dispose();
            menu = CreateMenu();
        }

        protected override ContextMenuStrip CreateMenu() {
            menu.Items.Clear();
            this.createSubMenus();
            return menu;
        }

        protected void createSubMenus() {

            var mainMenu = new ToolStripMenuItem {
                Text = "Create Version",
                //Image = GetBitmap("Green")
            };

            mainMenu.Click += (sender, args) => createVersion();

            menu.Items.Clear();
            menu.Items.Add(mainMenu);
        }

        private void createVersion() {
            foreach (string file in selectedVersionableFiles) {
                createVersion(file);
            }
        }

        private void createVersion(string file) {

            int latestVersion = getLatestVersion(file);

            string fnm = Path.GetFileNameWithoutExtension(file);
            string ext = Path.GetExtension(file);
            string dir = Path.GetDirectoryName(file);
            string cpy = null;

            if (latestVersion == 0) {
                cpy = Path.Combine(dir, fnm + ".1" + ext);
            } else {
                FilenameAndVersion fileVersionned = new FilenameAndVersion(file);
                cpy = Path.Combine(dir, fileVersionned.filename + '.' + (latestVersion + 1) + ext);
            }

            File.Copy(file, cpy, true);
        }

        private int getLatestVersion(string file) {

            int version = 0;

            FilenameAndVersion original = new FilenameAndVersion(Path.GetFileNameWithoutExtension(file), 0);

            string dir = Path.GetDirectoryName(file);
            foreach (string f in Directory.GetFiles(dir)) {

                FilenameAndVersion current = new FilenameAndVersion(f);
                
                if (original.filename != current.filename) {
                    continue;
                } 

                if (current.version > version) {
                    version = current.version;
                }   
            }

            return version;
        }

        public static Bitmap GetBitmap(string color) {
            if (Extensions.Dpi > 0.96f * 250 - 1) {
                return (Bitmap)Resources.ResourceManager.GetObject(color + "40", Resources.Culture);
            }
            if (Extensions.Dpi > 0.96f * 225 - 1) {
                return (Bitmap)Resources.ResourceManager.GetObject(color + "36", Resources.Culture);
            }
            if (Extensions.Dpi > 0.96f * 200 - 1) {
                return (Bitmap)Resources.ResourceManager.GetObject(color + "32", Resources.Culture);
            }
            if (Extensions.Dpi > 0.96f * 175 - 1) {
                return (Bitmap)Resources.ResourceManager.GetObject(color + "28", Resources.Culture);
            }
            if (Extensions.Dpi > 0.96f * 150 - 1) {
                return (Bitmap)Resources.ResourceManager.GetObject(color + "24", Resources.Culture);
            }
            if (Extensions.Dpi > 0.96f * 125 - 1) {
                return (Bitmap)Resources.ResourceManager.GetObject(color + "20", Resources.Culture);
            }
            return (Bitmap)Resources.ResourceManager.GetObject(color + "16", Resources.Culture);
        }
    }

    public struct FilenameAndVersion {

        public string filename;
        public int version;

        const string SPLIT_VERSION = @".(?=\d+$)";

        public FilenameAndVersion(string file) {
            version = 0;
            string[] split = Regex.Split(Path.GetFileNameWithoutExtension(file), SPLIT_VERSION);
            filename = split[0];
            if (split.Length > 1) {
                if (int.TryParse(split[1], out int cver)) {
                    version = cver;
                }
            }
        }

        public FilenameAndVersion(string filename, int version) {
            this.filename = filename;
            this.version = version;
        }

        public override string ToString() {
            return $"<filename:{filename}, version:{version}>";
        }
    }
}