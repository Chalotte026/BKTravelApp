using BkTravelApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Path = System.IO.Path;
namespace BkTravelApp.Views;

public partial class EditTravel : ContentPage
{
    Data.Generate gen = new Data.Generate();
    Data.BktravelDB DB = new Data.BktravelDB();
    bool IsPicker = false;
    byte[] imageByte = null;
    public EditTravel()
	{
		InitializeComponent();
        if(pkEditType.ItemsSource == null)
            pkEditType.ItemsSource = gen.TravelTypes;
    }

    public EditTravel(Models.Travel pTravel) : this()
    {
        this.BindingContext = pTravel;
        pkEditType.ItemsSource = gen.TravelTypes;
        if (!string.IsNullOrEmpty(pTravel.Type) && pTravel!=null)
        {
            var typeList = (ObservableCollection<TravelType>)pkEditType.ItemsSource;
            var typeSelect = typeList.Where(x => x.TypeName == pTravel.Type)
                .FirstOrDefault();

            pkEditType.SelectedItem = typeSelect;
        }
    }

    async void delteTravel_Clicked(object sender, EventArgs e)
    {

        bool confirmed = await this.DisplayAlert("Confirmation", "Are you sure you want to delete this travel?", "Yes", "No");

        if (confirmed)
        {
            var locate = (Travel)BindingContext;
            await DB.DeleteTravelAsync(locate);
            await DisplayAlert("Success", "Delete location success.", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            // ไม่ทำการลบข้อมูล
        }
    }

    async void saveEdit_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(editName.Text))
        {
            await DisplayAlert("Please Enter all location info", "Please enter location info before browsing directions.", "Enter");
        }
        else if (EditShowImage.Source == null)
        {
            await DisplayAlert("Please Add an Image", "Please add an image before saving.", "OK");
        }
        else
        {
            HasImageFolder();
            string imageGet = Path.Combine(FileSystem.AppDataDirectory, "images");
            string runID = Guid.NewGuid().ToString() + ".png";
            string picture = Path.Combine(imageGet, runID);
            var adds = editName.Text;

            var locate = (Travel)BindingContext;

            locate.Name = adds;
            locate.address = EditShowLoca.Text;
            locate.Describtion = EditShowDesc.Text;
            locate.longitude = EditShowLong.Text;
            locate.latitude = EditShowLa.Text;
            locate.ImageLocation = picture;
            /*
            if (imageByte != null)
            {
                await SaveImage(Path.Combine(imageGet, runID), imageByte);
            }
            else if (EditShowImage.Source is FileImageSource fileImageSource)
            {
                // Get the original image path from the binding context
                string originalImagePath = (BindingContext as Travel)?.ImageLocation;

                if (!string.IsNullOrEmpty(originalImagePath))
                {
                    // Read the original image file and convert it to byte array
                    byte[] originalImageBytes = File.ReadAllBytes(originalImagePath);

                    // Save the original image bytes
                    await SaveImage(Path.Combine(imageGet, runID), originalImageBytes);
                }
            }*/
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

    async void updateImage_Clicked(object sender, EventArgs e)
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
            EditShowImage.Source = null;
            EditShowImage.Source = ImageSource.FromStream(() => new MemoryStream(imageByte));
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

    async void getLocation_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(editName.Text))
        {
            await DisplayAlert("Please Enter Name Location", "Please enter a location Name before browsing directions.", "Enter");
        }
        else
        {
            var address = editName.Text;
            var locations = await Geocoding.Default.GetLocationsAsync(address);
            var location = locations?.FirstOrDefault();
            if (location != null)
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {

                    EditShowLoca.Text = $"{placemark.SubThoroughfare} {placemark.Thoroughfare}, {placemark.Locality}, {placemark.AdminArea} {placemark.PostalCode}, {placemark.CountryName}";
                    EditShowLa.Text = location.Latitude.ToString();
                    EditShowLong.Text = location.Longitude.ToString();
                }
            }
        }
    }
}