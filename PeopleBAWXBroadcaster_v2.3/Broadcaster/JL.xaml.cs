// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Broadcaster
{
    /// <summary>
    /// Interaction logic for JL.xaml
    /// </summary>
    public partial class JL : UserControl
    {
        private JumpList _jumpList;
        private JumpListCustomCategory _currentCategory;
        
        public JL()
        {
            InitializeComponent();
        }

        private void OnOpenFile(object unused1, RoutedEventArgs unused2)
        {
            this.JumpList.KnownCategoryToDisplay = JumpListKnownCategoryType.Recent;
            CommonOpenFileDialog cfd = new CommonOpenFileDialog();
            cfd.ShowDialog();
            //The only purpose of this method is to record the fact that a selection
            //has been made in a common file dialog. If there's a file type association
            //for this application, then the selected file will be added to this
            //application's Recent and Frequent system lists, which can then be used
            //to populate the Recent and/or Frequent jump list destination categories.
        }

        private void OnGetRemovedItems(object unused1, RoutedEventArgs unused2)
        {
            foreach (object item in this.JumpList.RemovedDestinations)
            {
                System.Diagnostics.Debug.WriteLine(item.GetType());
            }
        }

        private void OnEnsureRegistration(object unused1, RoutedEventArgs unused2)
        {
            if (Utilities.IsApplicationRegistered(TaskbarManager.Instance.ApplicationId))
                return;

            try
            {
                Utilities.RegisterFileAssociations(
                    TaskbarManager.Instance.ApplicationId,
                    false,
                    TaskbarManager.Instance.ApplicationId,
                    Assembly.GetExecutingAssembly().Location,
                    ".png");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error registering file type association:" + Environment.NewLine +
                    ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }        

        //The following method is part of the lab. Add a few user tasks, using several
        //instances of the JumpListLink class and the JumpList.AddUserTasks method. Some ideas are
        //notepad.exe, calc.exe, mspaint.exe and other Windows utilities.
        private void OnAddTask(object unused1, RoutedEventArgs unused2)
        {
            //TODO: Task 6--Using Taskbar Jump Lists, steps 1-7
            string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
            IJumpListTask notepadTask = new JumpListLink(
                System.IO.Path.Combine(systemFolder, "notepad.exe"), "Open Notepad")
                {
                    IconReference = new IconReference(Path.Combine(systemFolder, "notepad.exe"), 0)
                };
            IJumpListTask calcTask = new JumpListLink(
                Path.Combine(systemFolder, "calc.exe"), "Open Calculator")
                {
                    IconReference = new IconReference(Path.Combine(systemFolder, "calc.exe"), 0)
                };
            IJumpListTask paintTask = new JumpListLink(
                Path.Combine(systemFolder, "mspaint.exe"), "Open Paint")
            {
                IconReference = new IconReference(Path.Combine(systemFolder, "mspaint.exe"), 0)
            };

            JumpList.AddUserTasks(notepadTask, calcTask, new JumpListSeparator(), paintTask);
            JumpList.Refresh();

        }

        //The following property is part of the lab. If the jump list is not initialized yet,
        //create it using the static JumpList.CreateJumpList method, set the known category
        //and refresh the list.
        private JumpList JumpList
        {
            get
            {
                //TODO: Task 6--Using Taskbar Jump Lists, steps 8-12
                if (_jumpList == null)
                {
                    _jumpList = JumpList.CreateJumpList();
                    _jumpList.KnownCategoryToDisplay = showRecent.IsChecked.Value ?
                        JumpListKnownCategoryType.Recent :
                        JumpListKnownCategoryType.Frequent;
                    _jumpList.Refresh();
                }
                return _jumpList;
            }
        }

        //The following method is part of the lab. When the known category to display changes,
        //change the JumpList.KnownCategoryToDisplay property accordingly and then refresh the
        //jump list.
        private void showRecent_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Task 6--Using Taskbar Jump Lists, steps 13-15
            _jumpList.KnownCategoryToDisplay = showRecent.IsChecked.Value ?
                        JumpListKnownCategoryType.Recent :
                        JumpListKnownCategoryType.Frequent;
            _jumpList.Refresh();
        }

        //The following method is part of the lab. Clear the list of user tasks and 
        //refresh the jump list.
        private void OnClearTasks(object unused1, RoutedEventArgs unused2)
        {
            //TODO: Task 6--Using Taskbar Jump Lists, steps 16-19

            JumpList.ClearAllUserTasks();
            JumpList.Refresh();
        }

        //The following method is part of the lab. Create a new JumpListCustomCategory, store
        //it as the current category and refresh it. Don't forget to enable the sub-controls.
        private void createCategory_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Task 6--Using Taskbar Jump Lists, steps 20-25
            _currentCategory = new JumpListCustomCategory(this.txtCategory.Text);
            JumpList.AddCustomCategories(_currentCategory);
            JumpList.Refresh();

            this.createCategoryItem.IsEnabled = true;
            this.createCategoryLink.IsEnabled = true;
            this.txtCategoryItem.IsEnabled = true;
            this.txtCategoryLink.IsEnabled = true;

        }

        //The following method is part of the lab. Ensure that the filename is valid,
        //and if it is -- create a new item and add it to the current category, and refresh
        //the jump list.
        private void createCategoryItem_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Task 6--Using Taskbar Jump Lists, steps 26-31
            if (!CheckFileName(txtCategoryItem.Text))
                return;
            JumpListItem jli = new JumpListItem(GetTempFileName(txtCategoryItem.Text));
            _currentCategory.AddJumpListItems(jli);
            JumpList.Refresh();
        }

        //The following method is part of the lab. Ensure that the filename is valid,
        //and if it is -- create a new link and add it to the current category, and refresh
        //the jump list.
        private void createCategoryLink_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Task 6--Using Taskbar Jump Lists, steps 32-34
            if (!CheckFileName(txtCategoryItem.Text))
                return;
            JumpListLink jli = new JumpListLink(GetTempFileName(txtCategoryLink.Text), txtCategoryLink.Text);
            _currentCategory.AddJumpListItems(jli);
            JumpList.Refresh();
        }

        private bool CheckFileName(string fileName)
        {
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                MessageBox.Show("Please use only characters that are allowed in file names.",
                    "Invalid character found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private string GetTempFileName(string fileName)
        {
            string path = Path.Combine(System.IO.Path.GetTempPath(), fileName + ".png");
            File.Create(path).Close();  //Ensure the file exists
            return path;
        }
    }
}