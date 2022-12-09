using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MixMatch2.Resources.Helpers;
using Range = MixMatch2.Resources.Helpers.Range;

namespace MixMatch2.Shared.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ColumnDefinitionCollection HomeGridColumnDefinitions { get; set; } =
            new ColumnDefinitionCollection
            {
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
            };

        public RowDefinitionCollection HomeGridRowDefinitions { get; set; }
            = new RowDefinitionCollection
            {
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(1, GridUnitType.Star)),
            };


        public HomeViewModel()
        {
            
        }

    }
}