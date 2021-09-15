using FileScanner.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace FileScanner.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string selectedFolder;
        private ObservableCollection<string> folderItems = new ObservableCollection<string>();
         
        public DelegateCommand<string> OpenFolderCommand { get; private set; }
        public DelegateCommand<string> ScanFolderCommand { get; private set; }

        public ObservableCollection<string> FolderItems { 
            get => folderItems;
            set { 
                folderItems = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFolder
        {
            get => selectedFolder;
            set
            {
                selectedFolder = value;
                OnPropertyChanged();
                ScanFolderCommand.RaiseCanExecuteChanged();
            }
        }

        public MainViewModel()
        {
            OpenFolderCommand = new DelegateCommand<string>(OpenFolder);
            ScanFolderCommand = new DelegateCommand<string>(ScanFolder, CanExecuteScanFolder);
        }

        private bool CanExecuteScanFolder(string obj)
        {
            return !string.IsNullOrEmpty(SelectedFolder);
        }

        private void OpenFolder(string obj)
        {
            try
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        SelectedFolder = fbd.SelectedPath;
                    }
                }
            } catch (IOException e)
            {
                System.Windows.MessageBox.Show("Il y a une erreur dans l'ouverture du dossier: " + e);
            }
        }

        private async void ScanFolder(string dir)
        {
            _ = Task.Run(() =>
            {
                 
                FolderItems = new ObservableCollection<string>(GetDirs(dir));
                foreach (var item in Directory.EnumerateFiles(dir, "*"))
                {
                    FolderItems.Add(item);
                }
  
            });
        }

        IEnumerable<string> GetDirs(string dir)
        {
            foreach (var d in Directory.EnumerateDirectories(dir, "*"))
            {
                IEnumerable<string> files;
                try
                {
                    files = Directory.EnumerateFiles(d, "*");
                } catch(Exception e)
                {
                    continue;
                }
                foreach (var file in files)
                {
                    yield return file;
                }
                var temp = GetDirs(d);
                foreach (var dd in temp)
                {
                    yield return dd;
                }
                yield return d;
            }
        }
    }
}
