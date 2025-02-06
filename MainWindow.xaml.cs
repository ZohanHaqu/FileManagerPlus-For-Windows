using System;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace FileManagerPlus
{
    public partial class MainWindow : Window
    {
        private string currentDirectory;

        public MainWindow()
        {
            InitializeComponent();

            // Disable resizing of the window
            this.ResizeMode = ResizeMode.NoResize;

            // Disable maximizing the window
            this.WindowState = WindowState.Normal;
            this.MaxHeight = this.MinHeight = this.Height;
            this.MaxWidth = this.MinWidth = this.Width;

            // Display files in the Main Storage by default
            DisplayFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)); // Default directory
        }

        // Button click events to display different file contents
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string selectedOption = (sender as Button).Content.ToString();

            switch (selectedOption)
            {
                case "Main Storage":
                    DisplayFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                    break;
                case "Downloads":
                    DisplayFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads");
                    break;
                case "Storage Analysis":
                    ShowStorageInfo();
                    break;
                case "Images":
                    DisplayFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
                    break;
                case "Audio":
                    DisplayFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
                    break;
                case "Videos":
                    DisplayFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
                    break;
                case "Documents":
                    DisplayFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                    break;
                case "Apps":
                    DisplayFiles(@"C:\Program Files");
                    break;
                case "New Files":
                    DisplayFiles(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                    break;
                default:
                    break;
            }
        }

        // Function to display files and folders in the ListView
        private void DisplayFiles(string path)
        {
            if (Directory.Exists(path))
            {
                currentDirectory = path;
                DirectoryInfo di = new DirectoryInfo(path);
                var files = di.GetFileSystemInfos();

                FileListView.Items.Clear();
                foreach (var file in files)
                {
                    var fileDetails = new
                    {
                        Name = file.Name,
                        Type = file is FileInfo f ? f.Extension : "Folder",
                        Size = file is FileInfo fi ? FormatSize(fi.Length) : "" // Format size for files
                    };

                    FileListView.Items.Add(fileDetails);
                }
            }
        }

        // Function to show storage information (Total space and free space)
        private void ShowStorageInfo()
        {
            DriveInfo drive = new DriveInfo("C");
            StorageInfoText.Text = $"Total: {drive.TotalSize / (1024 * 1024 * 1024)} GB\nFree: {drive.AvailableFreeSpace / (1024 * 1024 * 1024)} GB";
        }

        // Helper function to format file size in human-readable format
        private string FormatSize(long size)
        {
            if (size >= 1024 * 1024 * 1024)
                return (size / (1024 * 1024 * 1024)) + " GB";
            else if (size >= 1024 * 1024)
                return (size / (1024 * 1024)) + " MB";
            else if (size >= 1024)
                return (size / 1024) + " KB";
            else
                return size + " Bytes";
        }

        // Handle double-click to navigate into directories or open files
        private void FileListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FileListView.SelectedItem != null)
            {
                dynamic selectedItem = FileListView.SelectedItem;
                string selectedName = selectedItem.Name;

                string newPath = Path.Combine(currentDirectory, selectedName);

                // If it's a directory, navigate into it
                if (Directory.Exists(newPath))
                {
                    DisplayFiles(newPath); // If it's a folder, open it
                }
                // If it's a file, open it with the default application
                else if (File.Exists(newPath))
                {
                    try
                    {
                        Process.Start(newPath); // Opens the file with its default associated application
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening file: {ex.Message}");
                    }
                }
            }
        }
    }
}


