using Microsoft.Maui.Devices.Sensors;
using System;
using Path = System.IO.Path;
namespace BkTravelApp.Views;

public partial class SettingLocation : ContentPage
{
    Data.Generate gen = new Data.Generate();
    Data.BktravelDB DB = new Data.BktravelDB();
    bool IsPicker = false;
    byte[] imageByte = null;
    public SettingLocation()
	{
		InitializeComponent();
        pkAddType.ItemsSource = gen.TravelTypes;
    }
    async void btnPickImage_Clicked(object sender, EventArgs e)
    {
        var img = await FilePicker.Default.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images,
            PickerTitle = "Pick an image Travel Location"
        });

        if (img != null)
        {
            var stream = await img.OpenReadAsync();
            imageByte = StreamToByteArray(stream);
            imgBtn.Source = null;
            imgBtn.Source = ImageSource.FromStream(() => new MemoryStream(imageByte));
            IsPicker = true;
        }
    }
    public byte[] StreamToByteArray(Stream input)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }
    async void btnGetDirec_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameLocate.Text))
        {
            await DisplayAlert("Please Enter Name Location", "Please enter a location Name before browsing directions.", "Enter");
        }
        else
        {
            string ios = "https://maps.apple.com/?q=" + nameLocate.Text.ToString();
            string android = "geo:0,0?q=" + nameLocate.Text.ToString();
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                await Launcher.Default.OpenAsync(ios);
            }
            else if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                await Launcher.Default.OpenAsync(android);
            }
        }
    }

    async void btnGetLocate_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameLocate.Text))
        {
            await DisplayAlert("Please Enter Name Location", "Please enter a location Name before browsing directions.", "Enter");
        }
        else
        {
            var address = nameLocate.Text;
            var locations = await Geocoding.Default.GetLocationsAsync(address);
            var location = locations?.FirstOrDefault();
            if (location != null)
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {

                    entryLocatation.Text = $"{placemark.SubThoroughfare} {placemark.Thoroughfare}, {placemark.Locality}, {placemark.AdminArea} {placemark.PostalCode}, {placemark.CountryName}";
                    entryLatitude.Text = location.Latitude.ToString();
                    entryLongitude.Text = location.Longitude.ToString();
                }
            }
        }
    }

    public async void btnSaveInfo_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameLocate.Text))
        {
            await DisplayAlert("Please Enter all location info", "Please enter location info before browsing directions.", "Enter");
        }
        else if (imgBtn.Source == null)
        {
            await DisplayAlert("Please Add an Image", "Please add an image before saving.", "OK");
        }
        else
        {
            HasImageFolder();
            string imageGet = Path.Combine(FileSystem.AppDataDirectory, "images");
            string runID = Guid.NewGuid().ToString() + ".png";
            string picture = Path.Combine(imageGet, runID);

            var adds = nameLocate.Text;

            var mselected = (Models.TravelType)pkAddType.SelectedItem;
            var locate = (Models.Travel)BindingContext;
            locate.Name = adds;
            locate.address = entryLocatation.Text;
            locate.Describtion = entryDescrib.Text;
            locate.longitude = entryLongitude.Text;
            locate.latitude = entryLatitude.Text;
            locate.ImageLocation = picture;
            locate.Type = mselected.TypeName.ToString();
            await SaveImage(Path.Combine(imageGet, runID), imageByte);
            await DB.SaveTravelAsync(locate);
            await DisplayAlert("Success", "Save your location you like Successful.", "OK");
            await Navigation.PopAsync();
        }
    }
    public async Task SaveImage(string pFilePath, byte[] imageByte)
    {
        using (var file = new FileStream(pFilePath, FileMode.Create))
        {
            await file.WriteAsync(imageByte, 0, imageByte.Length);
        }
    }
    public void HasImageFolder()
    {
        string imageFolderPath = Path.Combine(FileSystem.Current.AppDataDirectory, "images");
        if (!Directory.Exists(imageFolderPath))
        {
            Directory.CreateDirectory(imageFolderPath);
        }
    }
}