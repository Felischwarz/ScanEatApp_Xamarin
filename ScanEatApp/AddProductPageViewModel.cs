using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace ScanEatApp
{
    public class AddProductPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Command SaveCmd { get; }
        public Command ClearCmd { get; }

        private string _productToAdd;
        public string ProductToAdd
        {
            get => _productToAdd;
            set
            {
                if(_productToAdd == value) { return; }
                _productToAdd = value;

                //var name of what has been changed
                OnPropertyChanged(nameof(ProductToAdd));
            }
        }

        public AddProductPageViewModel()
        {
            SaveCmd = new Command(() =>
            {
                EanProduct product;

                string entered = ProductToAdd;
                if (entered != null)
                {
                    entered = entered.Split(new[] { Environment.NewLine }, StringSplitOptions.None)[0];
                    if (isEANCode(entered))
                    {
                        product = new EanProduct(entered);
                    }
                    else
                    {
                        product = new EanProduct(entered, true);
                    }

                    MyFridgePageViewModel.Instance.FridgeList.Add(product);
                }
                
                ProductToAdd = "";
            });

            ClearCmd = new Command(() =>
            {
                ProductToAdd = "";
            });
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool isEANCode(string code)
        {
            if(code == null)
            {
                return false;
            }
            else if(code.Length == 8 || code.Length == 13)
            {

                bool isNum = Int64.TryParse(code, out Int64 num);
                if(isNum)
                {
                    return true;
                }   
            }
            return false;
        }
    }
}
