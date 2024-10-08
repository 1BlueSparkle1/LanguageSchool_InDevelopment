﻿using Microsoft.Win32;
using SchoolLanguage.Components;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace SchoolLanguage.MyPages
{
    /// <summary>
    /// Логика взаимодействия для AddEditServicePage.xaml
    /// </summary>
    public partial class AddEditServicePage : Page
    {
        private Service service;
        public AddEditServicePage(Service _service)
        {
            InitializeComponent();
            App.servicePage = this;
            service = _service;
            this.DataContext = service;
            RefreshPhoto();
            
        }
        
        private void EditImageBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "*.png|*.png|*.jpg|*.jpg|*.jpeg|*.jpeg"
            };
            if (openFile.ShowDialog().GetValueOrDefault())
            {
                service.MainImage = File.ReadAllBytes(openFile.FileName);
                MainImage.Source = new BitmapImage(new Uri(openFile.FileName));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (service.ID == 0)
            {
                if (App.db.Service.Any(x => x.Title == service.Title))
                {
                    errors.AppendLine("Такая услуга уже существует!");
                }
                else
                {
                    App.db.Service.Add(service);
                }

            }
            if(service.DurationInSeconds > 14400)
            {
                errors.AppendLine("Длительность не может превышать 4 часов");
            }
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
            }
            else
            {
                App.db.SaveChanges();
                MessageBox.Show("Сохранено");
                navigation.NextPage(new PageComponent("Список услуг", new ServiceListPage()));
            }

            
        }

        private void CostTb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(char.IsDigit(e.Text[0])))
            {
                e.Handled = true;
            }
        }

        private void AddImageBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "*.png|*.png|*.jpg|*.jpg|*.jpeg|*.jpeg"
            };
            if (openFile.ShowDialog().GetValueOrDefault())
            {
                App.db.ServicePhoto.Add(new ServicePhoto
                {
                    ServiceID = service.ID,
                    PhotoByte = File.ReadAllBytes(openFile.FileName)
                });
                App.db.SaveChanges();
                RefreshPhoto();
            }
        }
        public void RefreshPhoto()
        {
            PhotoWp.Children.Clear();
            foreach (var photo in service.ServicePhoto)
            {
                PhotoWp.Children.Add(new PhotoUserControl(photo));
            }
            BitmapImage bitmapImage = new BitmapImage();
            if (service.MainImage != null)
            {
                MemoryStream byteStream = new MemoryStream(service.MainImage);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = byteStream;
                bitmapImage.EndInit();
                MainImage.Source = bitmapImage;
            }
            else
                bitmapImage = new BitmapImage(new Uri(@"\Resources\Безымянный.png", UriKind.Relative));

            
        }
    }
}
