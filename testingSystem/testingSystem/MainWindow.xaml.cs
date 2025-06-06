﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using testingSystem.View.UserControls;
using testingSystem.ViewModels;

namespace testingSystem
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DataService.UpdateVersrionFile(version);
            this.Closing += MainWindow_Closing;
        }

        private  void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CurrContentControl.Content is TakingTest takingTest)
            {
                var takingTestVM = takingTest.DataContext as TakingTestVM;

                if (takingTestVM != null)
                {
                    var result = MessageBox.Show("Сначала завершите тестирование", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    //if (result == MessageBoxResult.Yes) 
                    //{
                    //    await Task.Run(() => takingTestVM.FinishTest(null));
                    //    DataService.DeleteTempFile(); 
                    //    Application.Current.Shutdown();
                    //}
                }
            }
            else
            {
                DataService.DeleteTempFile();
                Application.Current.Shutdown();
            }
        }
    }
}
