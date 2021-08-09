using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using Xamarin.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace ScanEatApp
{
    public class MyFridgePageViewModel : INotifyPropertyChanged
    {
        public static MyFridgePageViewModel Instance;

        public event PropertyChangedEventHandler PropertyChanged;

        string FridgeSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FridgeSave.txt");

        private ObservableCollection<EanProduct> _fridgeList;
        public ObservableCollection<EanProduct> FridgeList
        {
            get
            {
                return _fridgeList;
            }
            set
            {
                if (_fridgeList == value) { return; }

                _fridgeList = value;

                //var name of what has been changed
                OnPropertyChanged(nameof(FridgeList));
            }
        }


        private EanProduct selectedProduct;
        public EanProduct SelectedProduct
        {
            get
            {
                return selectedProduct;
            }

            set
            {
                if (value != selectedProduct)
                {
                    selectedProduct = value;
                    OnPropertyChanged(nameof(SelectedProduct));    
                }
            }
        }

        public Command SelectionChangedCommand { get; }

        public MyFridgePageViewModel()
        {
            if (Instance != null) { return; }
            Instance = this;

            FridgeList = new ObservableCollection<EanProduct>();
            /*
            FridgeList.Add(new EanProduct("test1", true));
            FridgeList.Add(new EanProduct("test3", true));
            SaveFridgeProducts();
            
            FridgeList = LoadFridgeProducts();*/


            SelectionChangedCommand = new Command(() => 
            {
                if(SelectedProduct != null)
                {
                    Application.Current.MainPage.Navigation.PushAsync(new EanProductDetailPage(SelectedProduct));
                    SelectedProduct = null;
                }   
            });
        }

        public void SaveFridgeProducts()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(FridgeSavePath, FileMode.Create, FileAccess.Write);
           
            formatter.Serialize(stream, FridgeList);

            stream.Close();                 
        }

        public ObservableCollection<EanProduct> LoadFridgeProducts()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(FridgeSavePath, FileMode.Open, FileAccess.Read);

            ObservableCollection<EanProduct> updated_FridgeList = (ObservableCollection<EanProduct>)formatter.Deserialize(stream);

            return updated_FridgeList;
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }   
    }
}
