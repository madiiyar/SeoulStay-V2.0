using SeoulStay_V2._0.EntityData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace SeoulStay_V2._0
{
    /// <summary>
    /// Interaction logic for addListing.xaml
    /// </summary>
    /// 

    
    public partial class addListing : Window
    {
        private readonly bool _isEditMode;
        private readonly SeoulStay_V2._0.EntityData.Items _itemToEdit;

        public ObservableCollection<AmenityItem> Amenities { get; set; }
        public ObservableCollection<DistanceItem> Distances { get; set; }

        

        public addListing(bool isEditMode, SeoulStay_V2._0.EntityData.Items item = null)
        {
            InitializeComponent();

            _isEditMode = isEditMode;
            _itemToEdit = item;
           
            Load_Amenities();
            Load_Areas();
            LoadItemTypes();
            LoadDistances();


            if (_isEditMode && _itemToEdit != null)
            {
                this.Title = "Seoul Stay - Edit Listing";
                finishBtn.Content = "Close";
                InitializeEditItems();
                nextBtn.Visibility = Visibility.Hidden;
            }
            else
            {
                this.Title = "Seoul Stay - Add Listing";
                cancelBtn.Visibility = Visibility.Visible;
                finishBtn.Content = "Finish";
                nextBtn.Visibility = Visibility.Visible;
                finishBtn.IsEnabled = false;
            }
        }


        private void InitializeEditItems()
        {
            titleField.Text = _itemToEdit.Title;
            capacityField.Text = _itemToEdit.Capacity.ToString();
            bedNumField.Text = _itemToEdit.NumberOfBeds.ToString();
            bedroomNumField.Text  = _itemToEdit.NumberOfBedrooms.ToString();
            bathroomNumField.Text = _itemToEdit.NumberOfBathrooms.ToString();
            appAddressField.Text = _itemToEdit.ApproximateAddress.ToString();   
            exactAddressField.Text = _itemToEdit.ExactAddress.ToString();
            descriptionField.Text = _itemToEdit.Description.ToString();
            rulesField.Text = _itemToEdit.HostRules.ToString();
            minNights.Text = _itemToEdit.MinimumNights.ToString();
            maxField.Text = _itemToEdit.MaximumNights.ToString();
            typeComboBox.SelectedValue = _itemToEdit.ItemTypeID;
            areaComboBox.SelectedValue = _itemToEdit .AreaID;
        }

        private void Load_Areas()
        {
            using (var context = new Entities())
            {
                var areas = context.Areas.Select(a => new 
                {
                    a.ID,
                    a.Name
                }).ToList();

                areaComboBox.ItemsSource = areas;
                areaComboBox.DisplayMemberPath = "Name";
                areaComboBox.SelectedValuePath = "ID";
            }
        }
        private void Load_Amenities()
        {
            using (var context = new Entities())
            {
                var allAmenities = context.Amenities.Select(a => new AmenityItem
                {
                    ID = (int)a.ID,
                    Name = a.Name,
                    IsSelected = false 
                }).ToList();

                if (_isEditMode)
                {
                    var selectedAmenityIds = context.ItemAmenities.
                        Where(a => a.ItemID == _itemToEdit.ID).
                        Select(a => a.AmenityID).ToList();

                    foreach (var amenity in allAmenities)
                    {
                        if(selectedAmenityIds.Contains(amenity.ID))
                        {
                            amenity.IsSelected = true;
                        }
                    }
                }
                Amenities = new ObservableCollection<AmenityItem>(allAmenities);
                amenityDataGrid.ItemsSource = Amenities;
            }
        }

        private void LoadItemTypes()
        {
            using (var context = new Entities())
            {
                var itemTypes = context.ItemTypes.ToList();
                typeComboBox.ItemsSource = itemTypes;
                typeComboBox.DisplayMemberPath = "Name";
                typeComboBox.SelectedValuePath = "ID";
            }
        }

        private void LoadDistances()
        {
            using (var context = new Entities())
            {
                var allDistances  = context.Attractions.
                    Select(a => new DistanceItem
                    {
                        AttractionID =           (int)a.ID    ,       
                        AttractionName =            a.Name ,
                        AreaName =             a.Areas.Name ,
                        Distance =             0,
                        OnFoot =               0,
                        ByCar =                0
                    }).ToList();

                if (_isEditMode && _itemToEdit != null)
                {
                    var existingDistances = context.ItemAttractions.
                        Where(a => a.ItemID == _itemToEdit.ID).
                        Select(a => new DistanceItem
                        {
                          AttractionID =     (int)a.AttractionID,
                          AttractionName =   a.Attractions.Name,
                          AreaName =         a.Attractions.Areas.Name,
                          Distance =         (int)a.Distance,
                          OnFoot =           (int)a.DurationOnFoot,
                          ByCar = (int)a.DurationByCar
                        }).ToList();

                    foreach (var distance in allDistances)
                    {
                        var existingDistance = existingDistances.FirstOrDefault(a => a.AttractionID
                        == distance.AttractionID);

                        if(existingDistance != null)
                        {
                            distance.Distance = existingDistance.Distance;
                            distance.OnFoot = existingDistance.OnFoot;
                            distance.ByCar = existingDistance.ByCar;
                        }
                    }
                }

                Distances = new ObservableCollection<DistanceItem>(allDistances);
                distanceDataGrid.ItemsSource = Distances;
                
            }
        }

        private void finishBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode)
            {
                using (var context = new Entities())
                {
                    var itemToUpdate = context.Items.FirstOrDefault(a => a.ID == _itemToEdit.ID);
                    if(itemToUpdate != null)
                    {
                        itemToUpdate.Title = titleField.Text;
                        itemToUpdate.Capacity = int.Parse(capacityField.Text);
                        itemToUpdate.NumberOfBeds = int.Parse(bedNumField.Text);
                        itemToUpdate.NumberOfBedrooms = int.Parse(bedroomNumField.Text);
                        itemToUpdate.NumberOfBathrooms = int.Parse(bathroomNumField.Text);
                        itemToUpdate.ApproximateAddress = appAddressField.Text;
                        itemToUpdate.ExactAddress = exactAddressField.Text;
                        itemToUpdate.HostRules = rulesField.Text;
                        itemToUpdate.MinimumNights = int.Parse(minNights.Text);
                        itemToUpdate.MaximumNights = int.Parse(maxField.Text);
                        itemToUpdate.ItemTypeID = (long)typeComboBox.SelectedValue;
                        itemToUpdate.AreaID = (long)areaComboBox.SelectedValue;


                        var existingAmenities = context.ItemAmenities.Where(a => a.ItemID == itemToUpdate.ID)
                            .ToList();
                        context.ItemAmenities.RemoveRange(existingAmenities);

                        foreach ( var amen in Amenities.Where(a=> a.IsSelected))
                        {
                            context.ItemAmenities.Add(new ItemAmenities
                            {
                                ItemID = itemToUpdate.ID,
                                AmenityID = amen.ID,
                            });
                        }

                        var existingDistances = context.ItemAttractions.Where(a => a.ItemID == itemToUpdate.ID)
                            .ToList();
                        context.ItemAttractions.RemoveRange(existingDistances);

                        foreach (var distance in Distances)
                        {
                            context.ItemAttractions.Add(new ItemAttractions
                            {
                                ItemID = itemToUpdate.ID,
                                AttractionID = distance.AttractionID,
                                Distance = distance.Distance,
                                DurationByCar = distance.ByCar,
                                DurationOnFoot = distance.OnFoot
                            });
                        }


                        context.SaveChanges();
                        MessageBox.Show("Updated", "Your item information updated succesfully", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                using(var context = new Entities())
                {
                    var newItem = new SeoulStay_V2._0.EntityData.Items
                    {
                        Title = titleField.Text,
                        Capacity = int.Parse(capacityField.Text),
                        NumberOfBathrooms = int.Parse(capacityField.Text),
                        NumberOfBedrooms = int.Parse(capacityField.Text),
                        NumberOfBeds = int.Parse(capacityField.Text),
                        ApproximateAddress = appAddressField.Text,
                        ExactAddress = exactAddressField.Text,
                        Description = descriptionField.Text,
                        HostRules = rulesField.Text,
                        MinimumNights = int.Parse(minNights.Text),
                        MaximumNights = int.Parse(maxField.Text),
                        ItemTypeID = (long)typeComboBox.SelectedValue,
                        AreaID = (long)areaComboBox.SelectedValue,
                        UserID = Properties.Settings.Default.UserID,
                        GUID = new Guid()
                    };

                    context.Items.Add(newItem);
                    context.SaveChanges();


                    foreach (var distance in Distances)
                    {
                        context.ItemAttractions.Add(new ItemAttractions
                        {
                            ItemID = newItem.ID,
                            AttractionID = distance.AttractionID,
                            Distance = distance.Distance,
                            DurationOnFoot = distance.OnFoot,
                            DurationByCar = distance.ByCar
                        });


                        context.SaveChanges();
                        MessageBox.Show("Added successfully");
                    }
                }
            }

            Management management = new Management();   
            management.Show();
            this.Close();
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            if(ListingTab.SelectedIndex == 0)
            {
                if (string.IsNullOrWhiteSpace(titleField.Text) ||
                   string.IsNullOrWhiteSpace(descriptionField.Text) ||
                    string.IsNullOrWhiteSpace(capacityField.Text) ||
                    string.IsNullOrWhiteSpace(maxField.Text) ||
                    string.IsNullOrWhiteSpace(bedNumField.Text) ||
                    string.IsNullOrWhiteSpace(bedroomNumField.Text) ||
                    string.IsNullOrWhiteSpace(bathroomNumField.Text) ||
                    string.IsNullOrWhiteSpace(appAddressField.Text) ||
                    string.IsNullOrWhiteSpace(exactAddressField.Text) ||
                    string.IsNullOrWhiteSpace(rulesField.Text) ||
                    string.IsNullOrWhiteSpace(maxField.Text) ||
                    typeComboBox.SelectedItem == null ||
                    areaComboBox.SelectedItem == null
                    )
                {
                    MessageBox.Show("Error", "Please fill all the blanks", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; 
                }

                else if (capacityField.Text == "0" || bedNumField.Text == "0" || 
                    bedroomNumField.Text == "0" || bathroomNumField.Text == "0" ||
                    minNights.Text == "0" || maxField.Text == "0"
                    )
                {
                    MessageBox.Show("Error", "Capacity,  Beds, Bedrooms, Bathrooms, minimum nights and maximum nights can not be 0", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                } 
                else if(int.Parse(minNights.Text) > int.Parse(maxField.Text))
                {
                    MessageBox.Show("Error", "Maximum Nights needs to be equal or more than minimum nights", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                } else
                {
                    ListingTab.SelectedIndex++;
                }
            }
            else if(ListingTab.SelectedIndex == 1)
            {
                if(!Amenities.Any(a => a.IsSelected))
                {
                    MessageBox.Show("Error", "Please select at least one amenity.", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                } 
                else
                {
                    ListingTab.SelectedIndex = 2;
                    nextBtn.Visibility = Visibility.Hidden;
                    cancelBtn.Visibility = Visibility.Hidden;
                    finishBtn.IsEnabled = true;
                }
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Management management = new Management();
            management.Show();
            this.Close();
        }
    }
}
