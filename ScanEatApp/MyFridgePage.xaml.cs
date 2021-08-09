using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ScanEatApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyFridgePage : ContentPage
    { 
        public MyFridgePage()
        {

            InitializeComponent();
        }

        private void OnClickedAddProduct(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddProductPage());
        }
    }
}