using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BkTravelApp.Data
{
    class Generate
    {
        public ObservableCollection<Models.TravelType> TravelTypes { get; set; }
        public ObservableCollection<Models.Travel> travels { get; set; }

        public Generate() 
        { 
            TravelTypes= new ObservableCollection<Models.TravelType>();
            travels= new ObservableCollection<Models.Travel>();
            Type();
        }

        public void Type()
        {
            TravelTypes.Add(new Models.TravelType { TypeName = "All" });
            TravelTypes.Add(new Models.TravelType { TypeName = "Cafe" });
            TravelTypes.Add(new Models.TravelType { TypeName = "Club" });
            TravelTypes.Add(new Models.TravelType { TypeName = "Mall" });
            TravelTypes.Add(new Models.TravelType { TypeName = "Park" });
            TravelTypes.Add(new Models.TravelType { TypeName = "Restuarant" });
            TravelTypes.Add(new Models.TravelType { TypeName = "School" });
            TravelTypes.Add(new Models.TravelType { TypeName = "Shop" });
            TravelTypes.Add(new Models.TravelType { TypeName = "Temple" });
        }

    }
}
