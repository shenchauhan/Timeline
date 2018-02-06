using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Timeline
{
    public class GalleryItem : INotifyPropertyChanged
    {
        private string _name, _photographer, _imageSource, publicUrl;

        public GalleryItem(string name, string photographer, string imageName, string publicUrl)
        {
            Name = name;
            Photographer = photographer;
            ImageSource = $"ms-appx:///Assets/Images/{imageName}";
            PublicUrl = publicUrl;
        }

        public GalleryItem()
        {
        }
            
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        public string Photographer
        {
            get { return _photographer; }
            set
            {
                _photographer = value;
                RaisePropertyChanged();
            }
        }

        public string ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = $"ms-appx:///Assets/Images/{value}";
                RaisePropertyChanged();
            }
        }

        public string PublicUrl
        {
            get
            {
                return publicUrl;
            }
            set
            {
                publicUrl = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
