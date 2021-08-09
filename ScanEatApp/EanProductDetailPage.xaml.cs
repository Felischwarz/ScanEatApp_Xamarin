﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ScanEatApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EanProductDetailPage : ContentPage
    {
        public EanProductDetailPage(EanProduct eanProduct)
        {
            InitializeComponent();
            BindingContext = new EanProductDetailPageViewModel(eanProduct);
        }
    }
}