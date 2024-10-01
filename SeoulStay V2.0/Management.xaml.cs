using SeoulStay_V2._0.EntityData;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace SeoulStay_V2._0
{
    /// <summary>
    /// Interaction logic for Management.xaml
    /// </summary>
    public partial class Management : Window
    {
        private List<Items> _allItems;
        private List<Items> _allItems2;
        private Entities _context;

        public Management()
        {
            InitializeComponent();
            _context = new Entities();
            LoadData();
        }

        private void LoadData()
        {
           
            long useridd = Properties.Settings.Default.UserID;

            _allItems = _context.Items.Include("Areas").Include("ItemTypes").ToList();
            _allItems2 = _context.Items.Include("Areas").Include("ItemTypes").ToList().
            Where(x => x.UserID == Properties.Settings.Default.UserID).ToList();
            dataGridTraveller.ItemsSource = _allItems;
            ownerDataGrid.ItemsSource = _allItems2;
            UpdateItemCountText(dataGridTraveller.Items.Count);
        }

       

        private void UpdateItemCountText(int count)
        {
            itemNum.Text = $"{count} items found";
        }
        private void TabItemOwnerManager_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            UpdateItemCountText(ownerDataGrid.Items.Count);
        }

        private void TabItemTraveller_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            UpdateItemCountText(dataGridTraveller.Items.Count);
        }


        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searching = searchBox.Text.ToLower();
            var filteredItem = _allItems.
            Where(item => item.Title.ToLower().Contains(searching) ||
            item.Areas.Name.ToLower().Contains(searching) ||
            item.ItemTypes.Name.ToLower().Contains(searching)
            
            ).ToList();

            dataGridTraveller.ItemsSource = filteredItem;
            UpdateItemCountText(filteredItem.Count);
        }



        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.KeepMeSignedIn = false;
            Properties.Settings.Default.username = string.Empty;
            Properties.Settings.Default.password = string.Empty;
            Properties.Settings.Default.Save();
            var welcomeWindow = new MainWindow();
            welcomeWindow.Show();
            this.Close();
        }

        private void exitBtn_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

     

        private void addListingBtn_Click(object sender, RoutedEventArgs e)
        {
            addListing addListing = new addListing(false);
            addListing.Show();
            this.Close();
        }


        //uirenetin narse mynau
        private void editDetailBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.DataContext as Items;
            if (item != null)
            {
                addListing addListing = new addListing(true, item);
                addListing.Show();
                this.Close();
            }
            
        }

        private void travellerTab_MouseEnter(object sender, MouseEventArgs e)
        {
            UpdateItemCountText(dataGridTraveller.Items.Count);
        }

        private void ownerTab_MouseEnter(object sender, MouseEventArgs e)
        {
            UpdateItemCountText(ownerDataGrid.Items.Count);
        }
    }
}
