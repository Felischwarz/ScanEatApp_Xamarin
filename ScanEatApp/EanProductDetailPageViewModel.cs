using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Text;

namespace ScanEatApp
{
    public class EanProductDetailPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private EanProduct product;
        public EanProduct Product
        {
            get => product;
            set
            {
                if (value != product)
                {
                    product = value;
                    OnPropertyChanged(nameof(Product));
                }
            }
        }

        private ObservableCollection<string> product_Properties = new ObservableCollection<string>();
        public ObservableCollection<string> Product_Properties
        {
            get => product_Properties;
            set
            {
                if (value != product_Properties)
                {
                    product_Properties = value;
                    OnPropertyChanged(nameof(Product_Properties));
                }
            }
        }

        public EanProductDetailPageViewModel(EanProduct eanProduct)
        {     
            Product_Properties = new ObservableCollection<string>();

            Product = eanProduct;

            readEanProductProperties(eanProduct);
            
        }

        private void readEanProductProperties(EanProduct eanProduct)
        {
            if(eanProduct.detailname != null)
            {
                Product_Properties.Add("Detailname: " + eanProduct.detailname);
            }

            if (eanProduct.vendor != null)
            {
                Product_Properties.Add("Manufacturer: " + eanProduct.vendor);
            }

            if (eanProduct.maincat != null)
            {
                Product_Properties.Add("Main category: " + eanProduct.maincat);
            }

            if (eanProduct.subcat != null)
            {
                Product_Properties.Add("Sub category: " + eanProduct.subcat);
            }

            if (eanProduct.content_properties != null)
            {
                string content_properties_text = "";
                foreach(string content_property in eanProduct.content_properties)
                {            
                    if(eanProduct.content_properties.IndexOf(content_property) == eanProduct.content_properties.Count - 1)
                    {
                        content_properties_text += "\n" + "-" + content_property;
                    }

                    else
                    {
                        content_properties_text += "\n" + "-" + content_property;
                    }
                }

                Product_Properties.Add("About the ingredients: " + content_properties_text);
            }

            if (eanProduct.pack_properties != null)
            {
                string pack_properties_text = "";
                foreach (string pack_property in eanProduct.pack_properties)
                {
                    if (eanProduct.pack_properties.IndexOf(pack_property) == eanProduct.pack_properties.Count - 1)
                    {
                        pack_properties_text += "\n" + "-" + pack_property;
                    }

                    else
                    {
                        pack_properties_text += "\n" + "-" + pack_property;
                    }
                }

                Product_Properties.Add("About the packaging: " + pack_properties_text);
            }

            if (eanProduct.descr != null)
            {
                Product_Properties.Add("Description: " + eanProduct.descr);
            }

            if (eanProduct.origin != null)
            {
                Product_Properties.Add("Origin: " + eanProduct.origin);
            }

            if (eanProduct.validated != null)
            {
                Product_Properties.Add("validated: " + eanProduct.validated);
            }

            if (eanProduct.timeStamp != null)
            {
                Product_Properties.Add("added to fridge at " + eanProduct.timeStamp.ToString());
            }
        }
        
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
