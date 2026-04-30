using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BkTravelApp.Models
{
    public class Travel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string address { get; set; }
        public string Describtion { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string ImageLocation { get; set; }
        public string Type { get; set; }

        private string distance;
        public string Distance
        {
            get { return distance; }
            set
            {
                if (distance != value)
                {
                    distance = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
