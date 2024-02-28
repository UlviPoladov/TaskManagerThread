using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace WpfApp13
{
    public partial class MainWindow : Window
    {
        private List<string> blacklist;
        private Thread blacklistThread;
        private Thread taskManagerThread;

        public MainWindow()
        {
            InitializeComponent();

            
            Closing += MainWindow_Closing;

            blacklist = new List<string>();

            blacklistThread = new Thread(BlacklistThreadFunction);
            blacklistThread.Start();

            taskManagerThread = new Thread(TaskManagerThreadFunction);
            taskManagerThread.Start();
        }


        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            blacklistThread.Abort();
            taskManagerThread.Abort();
        }

        private void TaskManagerThreadFunction()
        {
            try
            {
                while (true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        UpdateTaskManagerData(null, null);
                    });

                    Thread.Sleep(1000); 
                }
            }
            catch (ThreadAbortException)
            {
               
            }
        }

        private void UpdateTaskManagerData(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcesses();
            datagrid.Items.Clear();

            foreach (Process process in processes)
            {
                string processName = process.ProcessName;

                datagrid.Items.Add(new
                {
                    ProcessName = processName,
                });
            }
        }

        private void endproccesbtn_Click(object sender, RoutedEventArgs e)
        {
            if (datagrid.SelectedItem != null)
            {
                dynamic selectedItem = datagrid.SelectedItem;
                string processName = selectedItem.ProcessName;

                Process[] processes = Process.GetProcessesByName(processName);
                foreach (Process process in processes)
                {
                    process.Kill();
                }

                UpdateTaskManagerData(null, null);
            }
        }

        private void blacklistbtn_Click(object sender, RoutedEventArgs e)
        {
            string appName = blacklistxtbox.Text;
            blacklist.Add(appName);

            blacklistlistbox.ItemsSource = null;
            blacklistlistbox.ItemsSource = blacklist;
        }

        private void BlacklistThreadFunction()
        {
            try
            {
                while (true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        Process[] processes = Process.GetProcesses();

                        foreach (Process process in processes)
                        {
                            foreach (string blacklistedAppName in blacklist)
                            {
                                if (process.ProcessName.Equals(blacklistedAppName, StringComparison.OrdinalIgnoreCase))
                                {
                                    process.Kill();
                                }
                            }
                        }
                    });

                    Thread.Sleep(2000); 
                }
            }
            catch (ThreadAbortException)
            {
                
            }
        }
    }
}
