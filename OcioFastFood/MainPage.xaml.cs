using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Services.Store;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Services.Store.Engagement;
using Windows.UI.ViewManagement;
using Windows.System;

namespace OcioFastFood
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string PHONE_NUMBER = "+39 0125 280747";

        public MainPage()
        {
            this.InitializeComponent();

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Windows.UI.Colors.Black;
            titleBar.ForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonBackgroundColor = Windows.UI.Colors.Black;
            titleBar.ButtonHoverForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonHoverBackgroundColor = Windows.UI.Colors.DarkSlateGray;
            titleBar.ButtonPressedForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonPressedBackgroundColor = Windows.UI.Colors.Gray;

            titleBar.InactiveForegroundColor = Windows.UI.Colors.Gray;
            titleBar.InactiveBackgroundColor = Windows.UI.Colors.DarkGray;
            titleBar.ButtonInactiveForegroundColor = Windows.UI.Colors.LightGray;
            titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.LightSlateGray;

            Browser.Source = new Uri("https://www.ociofastfood.it");

            SizeBrowser();

            RegisterPushNotifications();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SizeBrowser();
        }

        private void SizeBrowser()
        {
            Browser.Width = this.ActualWidth;
            Browser.Height = (this.ActualHeight == 0 || this.ActualHeight < 51) ? 100 : this.ActualHeight - 50;
        }

        private async void RegisterPushNotifications()
        {
            StoreServicesEngagementManager engagementManager = StoreServicesEngagementManager.GetDefault();
            await engagementManager.RegisterNotificationChannelAsync();
        }

        public async Task<bool> ShowRatingReviewDialog()
        {
            StoreSendRequestResult result = await StoreRequestHelper.SendRequestAsync(
                StoreContext.GetDefault(), 16, String.Empty);

            if (result.ExtendedError == null)
            {
                return true;   
            }
            return false;
        }

        async private void AddContact()
        {
            ContactStore store = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);
            var lists = await store.FindContactListsAsync();
            ContactList list = lists.FirstOrDefault((x) => x.DisplayName == "myPrivateList");
            if (list == null)
            {
                list = await store.CreateContactListAsync("myPrivateList");
                list.OtherAppReadAccess = ContactListOtherAppReadAccess.Full;
                list.OtherAppWriteAccess = ContactListOtherAppWriteAccess.SystemOnly;
                await list.SaveAsync();

            }

            Contact contact = new Contact();
            contact.Name = "Ocio fast food";
            contact.Websites.Add(new ContactWebsite() { Description = "Sito web", RawValue = "https://www.facebook.com/ocioivrea/",  Uri = new Uri("https://www.facebook.com/ocioivrea/") });
            contact.Addresses.Add(new ContactAddress() { StreetAddress = "Corso Nigra 67", Description = "Indirizzo", Country = "Italia", Kind = ContactAddressKind.Work, Locality = "Ivrea", PostalCode = "10015", Region = "TO" });

            var appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var assets = await appInstalledFolder.GetFolderAsync("Assets");
            var imageFile = await assets.GetFileAsync("StoreLogo.backup.png");
            contact.SourceDisplayPicture = imageFile;

            contact.Phones.Add(new ContactPhone() { Kind = ContactPhoneKind.Work, Number = PHONE_NUMBER });
            await list.SaveContactAsync(contact);

            var dialog = new MessageDialog("Contatto inserito. Grazie!");
            await dialog.ShowAsync();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddContact();
        }

        async private void Rate_Click(object sender, RoutedEventArgs e)
        {
            //bool result = await ShowRatingReviewDialog();
            bool result = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9PNSNTC5VKKH"));
        }

        private async void NavigateTo_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("ms-drive-to:?destination.latitude=45.46205&destination.longitude=7.875009&destination.name=Ocio fast food");
            var launcherOptions = new LauncherOptions();
            launcherOptions.TargetApplicationPackageFamilyName = "Microsoft.WindowsMaps_8wekyb3d8bbwe";
            await Launcher.LaunchUriAsync(uri, launcherOptions);
        }
    }
}
