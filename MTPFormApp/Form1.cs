using System;
using System.Linq;
using System.Windows.Forms;
using PortableDevices;

namespace MTPFormApp
{
    public partial class Form1 : Form
    {
        private PortableDevice Device;
        private PortableDeviceFolder Root;
        private PortableDeviceFolder CurFolder;

        public Form1()
        {
            InitializeComponent();

            SetupList();

            SetupDevice();

            this.Root = GetRootFolder();
            ShowFolder(this.Root);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
        }

        private void Form1_Deactived(object sender, EventArgs e)
        {
            
        }

        private void ReloadBtn_Clicked(object sender, EventArgs e)
        {

        }

        private void ListVIew_DoubleClicked(object sender, EventArgs e)
        {
            int selected = this.ListView.SelectedItems[0].Index;
            PortableDeviceObject selectedObj = this.CurFolder.Files[selected];
            if (selectedObj is PortableDeviceFolder)
            {
                var folder = GetFolderContents((PortableDeviceFolder)selectedObj);
                ShowFolder(folder);
            }
            else if(selectedObj is PortableDeviceFile)
            {
                //メッセージボックスを表示する
                DialogResult result = MessageBox.Show("ファイルをダウンロードしますか？",
                    "確認",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                //何が選択されたか調べる
                if (result == DialogResult.OK)
                {
                    //「はい」が選択された時
                    string filePath = GetSaveFile(selectedObj.Name);
                    if (filePath.Length > 0)
                    {
                        this.Device.Connect();
                        this.Device.Prepare();

                        this.Device.DownloadFile((PortableDeviceFile)selectedObj, filePath);

                        this.Device.Disconnect();

                        MessageBox.Show("ファイルのダウンロードを完了しました。");
                    }
                }
            }
        }

        private void UpdBtn_Clicked(object sender, EventArgs e)
        {
            string filePath = SelectFile();
            if (filePath.Length > 0)
            {
                this.Device.Connect();
                this.Device.Prepare();

                this.Device.TransferContentToDevice(filePath, this.CurFolder);

                this.Device.Disconnect();

                MessageBox.Show("ファイルのアップロードを完了しました。");
            }
        }

        private string SelectFile()
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = "";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            ofd.InitialDirectory = @"C:\";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            ofd.Filter = "すべてのファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            //2番目の「すべてのファイル」が選択されているようにする
            ofd.FilterIndex = 1;
            //タイトルを設定する
            ofd.Title = "アップロードするファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //存在しないファイルの名前が指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckFileExists = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckPathExists = true;

            //ダイアログを表示する
            string selectedFilePath = "";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する
                selectedFilePath = ofd.FileName;
            }
            return selectedFilePath;
        }

        private string GetSaveFile(string filename)
        {//SaveFileDialogクラスのインスタンスを作成
            SaveFileDialog sfd = new SaveFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            sfd.FileName = filename;
            //はじめに表示されるフォルダを指定する
            sfd.InitialDirectory = @"C:\";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            string[] arr = filename.Split(new string[] { "." }, StringSplitOptions.None);
            string filter = "すべてのファイル(*.*)|*.*";
            if (arr.Length >= 2)
            {
                string type = arr[arr.Length - 1];
                filter = type + "ファイル(*." + type + ")|*." + type + "|" + filter;
            }
            sfd.Filter = filter;
            //[ファイルの種類]ではじめに選択されるものを指定する
            sfd.FilterIndex = 1;
            //タイトルを設定する
            sfd.Title = "保存先のファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            sfd.RestoreDirectory = true;
            //既に存在するファイル名を指定したとき警告する
            //デフォルトでTrueなので指定する必要はない
            sfd.OverwritePrompt = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            sfd.CheckPathExists = true;

            //ダイアログを表示する
            string saveFilePath = "";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき、選択されたファイルパスを格納する
                saveFilePath = sfd.FileName;
            }
            return saveFilePath;
        }

        private void ShowFolder(PortableDeviceFolder folder)
        {
            // カレントディレクトリ保存
            this.CurFolder = folder;
            // 行アイテム削除
            while(this.ListView.Items.Count > 0)
            {
                this.ListView.Items.RemoveAt(0);
            }
            // 行アイテムに情報追加
            foreach (PortableDeviceObject obj in folder.Files)
            {
                ListViewItem item = this.ListView.Items.Add(obj.Name);
                item.Selected = true;
                item.Focused = true;
                item.SubItems.Add((obj is PortableDeviceFolder) ? "フォルダ" : "ファイル");
                item.SubItems.Add("" + obj.Size);
            }
        }

        private PortableDeviceFolder GetFolderContents(PortableDeviceFolder folder)
        {
            this.Device.Connect();
            this.Device.Prepare();

            var contents = this.Device.GetContents(folder, false);

            this.Device.Disconnect();

            return contents;
        }

        private PortableDeviceFolder GetRootFolder()
        {
            this.Device.Connect();
            this.Device.Prepare();

            PortableDeviceFolder root = this.Device.GetRootContent(false);

            this.Device.Disconnect();

            return root;
        }

        private void SetupDevice()
        {
            var devices = new PortableDeviceCollection();
            devices.Refresh();

            this.Device = devices.First();
        }

        private void SetupList()
        {
            ListView.View = View.Details;
            
            // Add a column with width 110 and left alignment.
            ListView.Columns.Add("File name", 110, HorizontalAlignment.Left);

            // Add a column with width 110 and left alignment.
            ListView.Columns.Add("File type", 110, HorizontalAlignment.Left);

            // Add a column with width 110 and left alignment.
            ListView.Columns.Add("File size", 110, HorizontalAlignment.Left);
        }
    }
}
