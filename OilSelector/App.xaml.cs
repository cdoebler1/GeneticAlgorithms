using Xamarin.Forms;

namespace OilSelector
{
	public partial class App : Application
    {
        //static OilDatabase database;
        private int instrSelection = 0;
        private int tempSelection = 0;
        private string strLow = "";
        private string strHigh = "";

        public App ()
		{
			InitializeComponent();

			MainPage = new MainPage();
		}


        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
            // Handle when your app sleeps
            instrSelection = MainPage.FindByName<Picker>("pkInstruments").SelectedIndex;
            tempSelection = MainPage.FindByName<Picker>("pkTemperatures").SelectedIndex;
            strLow = MainPage.FindByName<Entry>("txtLowRange").Text;
            strHigh = MainPage.FindByName<Entry>("txtHighRange").Text;

        }

		protected override void OnResume ()
		{
            // Handle when your app resumes
            MainPage.FindByName<Picker>("pkInstruments").SelectedIndex = instrSelection;
            MainPage.FindByName<Picker>("pkTemperatures").SelectedIndex = tempSelection;
            MainPage.FindByName<Entry>("txtLowRange").Text = strLow;
            MainPage.FindByName<Entry>("txtHighRange").Text = strHigh;
        }
	}
}
