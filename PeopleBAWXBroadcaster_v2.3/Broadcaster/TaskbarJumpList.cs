using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Broadcaster
{
    public class TaskbarJumpList
    {
        private JumpList _jumpList;
        private JumpListCustomCategory _currentCategory = new JumpListCustomCategory("Recent");

        private JumpList JumpList
        {
            get
            {
                //TODO: Task 6--Using Taskbar Jump Lists, steps 8-12
                if (_jumpList == null)
                {
                    _jumpList = JumpList.CreateJumpList();
                    _jumpList.KnownCategoryToDisplay = JumpListKnownCategoryType.Recent;
                    _jumpList.Refresh();
                    _jumpList.AddCustomCategories(_currentCategory);
                }
                return _jumpList;
            }
        }


        public void AddTask()
        {
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

        public void AddRecent(string path)
        {
            OnEnsureRegistration();
            
            //JumpList.AddToRecent(path);
            JumpListLink jli = new JumpListLink(System.Environment.CurrentDirectory.ToString(), path);
            _currentCategory.AddJumpListItems(jli);

            JumpList.Refresh();  
        }

        private void OnEnsureRegistration()
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
                    ".xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error registering file type association:" + Environment.NewLine +
                    ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
