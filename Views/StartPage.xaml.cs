
using BkTravelApp.Models;
using CommunityToolkit.Maui.Core;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;

namespace BkTravelApp.Views;

public partial class StartPage : ContentPage
{
	Data.Generate gen = new Data.Generate();
    Data.BktravelDB DB = new Data.BktravelDB(); 
    public ICommand EditCommand{ get;  set; }
    public ICommand MapCommand { get;  set; }
    CancellationTokenSource cts;
    public ObservableCollection<Models.Travel> Travels { get; set; }
	public StartPage()
	{
		InitializeComponent();
        EditCommand = new Command<Models.Travel>(SelectedEdit);
        pkLocaType.ItemsSource = gen.TravelTypes;
        MapCommand = new Command<Models.Travel>(SelectedMap);
        this.BindingContext = this;
    }

    private async void SelectedEdit(Travel obj)
    {
        //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        //await Navigation.PushAsync(new Views.EditTravel {
        //    BindingContext = obj 
        //});

        await Navigation.PushAsync(new Views.EditTravel(obj));
    }

    private async void SelectedMap(Travel obj)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        string text = obj.Name;
        string ios = "https://maps.apple.com/?q=" + text;
        string android = "geo:0,0?q=" + text;
        if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            await Launcher.Default.OpenAsync(ios);
        }
        else if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            await Launcher.Default.OpenAsync(android);
        }
    }

    async void AddTravel_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Views.SettingLocation { BindingContext = new Models.Travel() });
    }
    public void pkLocaType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (pkLocaType.SelectedIndex > -1)
        {
            var row = (Models.TravelType)pkLocaType.SelectedItem;

            if (row.TypeName == "All") // ตรวจสอบเงื่อนไขสำหรับตัวเลือก "All"
            {
                showLocate.ItemsSource = Travels; // แสดงสถานที่ทั้งหมด
                showType.Text = "All"; // กำหนดข้อความให้กับ Label showType
            }
            else
            {
                showLocate.ItemsSource = Travels.Where(x => x.Type == row.TypeName).ToList(); // แสดงสถานที่ตามประเภทที่เลือก
                showType.Text = row.TypeName; // กำหนดข้อความให้กับ Label showType
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Travels = new ObservableCollection<Models.Travel>(await DB.GetTravelsAsync());
        showLocate.ItemsSource = Travels;
    }

    void showLocate_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Travel selectedItem = e.CurrentSelection[0] as Travel;
    }
    private async Task<Location> GetCurrentLocation()
    {
        Location Mylocate = null;
        try
        {
            var request = new GeolocationRequest(
                GeolocationAccuracy.Medium,
                TimeSpan.FromSeconds(10));
            cts = new CancellationTokenSource();
            Mylocate =
                await Geolocation.GetLocationAsync(request, cts.Token);
        }
        catch (FeatureNotSupportedException fnsEx)
        {
            await DisplayAlert("Faild", fnsEx.Message, "OK");
        }
        catch (PermissionException pEx)
        {
            await DisplayAlert("Faild", "Please give location permission", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Faild", ex.Message, "OK");
        }
        return Mylocate;
    }
    private async void btnNearMe_Clicked(object sender, EventArgs e)
    {
        var currentLocation = await GetCurrentLocation();
        if (currentLocation != null)
        {
            foreach (var travel in Travels)
            {
                double distance = Location.CalculateDistance(
                    new Location(
                        double.Parse(travel.latitude),
                        double.Parse(travel.longitude)
                    ),
                    new Location(
                        currentLocation.Latitude,
                        currentLocation.Longitude
                    ),
                    DistanceUnits.Kilometers
                );
                travel.Distance = $"{distance:N2} Km.";
            }
        }
        Travels = new ObservableCollection<Travel>(Travels.OrderBy(t => double.Parse(t.Distance.Replace(" Km.", ""))));
        showLocate.ItemsSource = Travels;
        showType.Text = "Near me";
    }
}