using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using System.Reflection;

namespace OilSelector
{
    public partial class MainPage : ContentPage
    {

        #region Private Member Variables
        private double m_dblLow = 0;
        private double m_dblHigh = 0;
        private int m_numBulbs = 0;
        private int m_addedOilsCount = 0;
        private int m_navCount = 0;
        private int m_internetTimeout = 0;
        private bool m_doneLoadingOils = false;
        private List<ViscosityItem> oils;
        private List<ViscosityItem> oilsToCart;

        //Picker pkInstruments;
        //Picker pkTemperatures;
        //Entry txtLowRange;
        //Entry txtHighRange;
        //Button btnFindOils;
        //Button btnPurchaseOilsOnline;
        //TableView tblOils;
        //Label lblContact;
        #endregion

        #region Constructor
        public MainPage()
        {
            InitializeComponent();

            oilsToCart = new List<ViscosityItem>();

            // Create the delegate that invokes methods for the timer.
            Device.StartTimer(TimeSpan.FromSeconds(2), () => {
                                                                DisplayMain();
                                                                return false;
                                                             });
            imgSpinner.RotateTo(2160, 5000);
        }
        #endregion

        #region CallBacks
        async void DisplayMain()
        {/*
            var stackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Vertical,
                Spacing = 10
            };

            var lblInst = new Label { Text = "Instrument" };
            pkInstruments = new Picker { HorizontalOptions = LayoutOptions.FillAndExpand };
            pkInstruments.SelectedIndexChanged += PkInstruments_SelectedIndexChanged;

            var lblTemp = new Label { Text = "Temperature (°C)" };
            pkTemperatures = new Picker();
            pkTemperatures.SelectedIndexChanged += PkTemperatures_SelectedIndexChanged;

            var lblLowRange = new Label { Text = "Low Range (cSt)" };
            txtLowRange = new Entry();
            txtLowRange.Unfocused += TxtLowRange_Unfocused;

            var lblHighRange = new Label { Text = "High Range (cSt)" };
            txtHighRange = new Entry();
            txtHighRange.Unfocused += TxtHighRange_Unfocused;

            btnFindOils = new Button
            {
                Text = "Find Oils",
                IsEnabled = false
            };
            btnFindOils.Clicked += BtnFindOils_Clicked;

            tblOils = new TableView
            {
                Intent = TableIntent.Data,
                IsVisible = false,
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowHeight = 50
            };

            lblContact = new Label
            {
                IsVisible = false,
                LineBreakMode = LineBreakMode.WordWrap,
                HorizontalOptions = LayoutOptions.Center,
                Text = "Call us at (814)-353-8000 or visit our website to order your oil standards today."
            };

            //btnPurchaseOilsOnline = new Button
            //{
            //    Text = "Purchase Oils Online",
            //    IsVisible = false
            //};
            //btnPurchaseOilsOnline.Clicked += BtnPurchaseOilsOnline_Clicked;

            stackLayout.Children.Add(lblInst);
            stackLayout.Children.Add(pkInstruments);
            stackLayout.Children.Add(lblTemp);
            stackLayout.Children.Add(pkTemperatures);
            stackLayout.Children.Add(lblLowRange);
            stackLayout.Children.Add(txtLowRange);
            stackLayout.Children.Add(lblHighRange);
            stackLayout.Children.Add(txtHighRange);
            stackLayout.Children.Add(btnFindOils);
            stackLayout.Children.Add(tblOils);
            stackLayout.Children.Add(lblContact);

            scrollView.Content = stackLayout;
            */    

            // Perform slide-in animation of main screen, start at the right hand side of the screen
            int startingPoint = (int)Math.Truncate(relLayout.Width + 1);
            scrollView.TranslateTo(startingPoint, 0, 0);
            await stkTop.TranslateTo(startingPoint, 0, 0);

            pkInstruments.Items.Add("CAV 4.2");
            pkInstruments.Items.Add("MiniAV");
            pkInstruments.Items.Add("MiniAV-LT");
            pkInstruments.Items.Add("CAV2100");
            pkInstruments.Items.Add("CAV2200");

            pkTemperatures.Items.Add("-40.00");
            pkTemperatures.Items.Add("-30.00");
            pkTemperatures.Items.Add("-20.00");
            pkTemperatures.Items.Add("20.00");
            pkTemperatures.Items.Add("25.00");
            pkTemperatures.Items.Add("30.00");
            pkTemperatures.Items.Add("37.78");
            pkTemperatures.Items.Add("40.00");
            pkTemperatures.Items.Add("50.00");
            pkTemperatures.Items.Add("60.00");
            pkTemperatures.Items.Add("65.00");
            pkTemperatures.Items.Add("70.00");
            pkTemperatures.Items.Add("75.00");
            pkTemperatures.Items.Add("80.00");
            pkTemperatures.Items.Add("90.00");
            pkTemperatures.Items.Add("98.89");
            pkTemperatures.Items.Add("100.00");
            pkTemperatures.Items.Add("115.56");
            pkTemperatures.Items.Add("135.00");
            pkTemperatures.Items.Add("150.00");

            pkInstruments.SelectedIndex = 0;
            pkTemperatures.SelectedIndex = 7;
       
            // Load the standard names/temps/viscs from file
            if (!GetOilsFromFile())
                await DisplayAlert("Alert", "Error : Unable to load oil viscosities from file.", "OK");


            //imgLogoWelcome.IsVisible = false;
            scrollView.IsVisible = true;
            stkTop.IsVisible = true;
            imgSpinner.IsVisible = false;

            // Perform the slide to 0, 0 animation
            scrollView.TranslateTo(0, 0, 400);
            await stkTop.TranslateTo(0, 0, 400);
        }
        
        bool CheckForInternet()
        {
            if (m_doneLoadingOils)
                return false;

            if (m_internetTimeout > 90)
            {
                stkOverlay.IsVisible = false;
                imgSpinner.IsVisible = false;
                stkWebNav.IsVisible = false;
                webView.IsVisible = false;
                DisplayAlert("Alert", "Could not connect to the internet.", "OK");
                return false;
            }
            else
            {
                m_internetTimeout++;
                return true;
            }
        }
        #endregion

        #region Events
        private async void BackWeb_Button_Clicked(object sender, EventArgs e)
        {
            // Check to see if there is anywhere to go back to
            if (m_navCount > 0)
            {
                m_navCount = m_navCount - 2;    // Subtract one on the count, and another because one gets added upon navigated
                webView.GoBack();
            }
            else
            { // If not, leave the webWiew

                // Perform slide-in animation of main screen, start at the right hand side of the screen
                int startingPoint = (int)Math.Truncate(relLayout.Width + 1);
                stkTop.TranslateTo(startingPoint, 0, 0);
                await scrollView.TranslateTo(startingPoint, 0, 0);

                webView.IsVisible = false;
                stkWebNav.IsVisible = false;

                stkTop.TranslateTo(0, 0, 400);
                await scrollView.TranslateTo(0, 0, 400);
            }
        }

        private void BtnPurchaseOilsOnline_Clicked(object sender, EventArgs e)
        {
            m_addedOilsCount = 0;
            if (oilsToCart.Count > 0)
            {
                m_internetTimeout = 0;
                m_doneLoadingOils = false;
                Device.StartTimer(TimeSpan.FromSeconds(1), () => CheckForInternet());
                imgSpinner.IsVisible = true;
                stkOverlay.IsVisible = true;
                imgSpinner.RotateTo(51840, 120000);
                webView.Source = @"http://cannoninstrument.com/en/Checkout/EmptyBasket";
            }
            else
                DisplayAlert("Alert", "No oils selected to purcahse", "OK");
        }

        private void BtnFindOils_Clicked(object sender, EventArgs e)
        {
            // if we have valid numeric values for high and low we can allow user to find oils
            if (m_dblHigh != 0 && m_dblLow != 0)
            {
                if (m_dblHigh > m_dblLow)
                {
                    FindOils();
                }
                else
                    DisplayAlert("Alert", "High Range must be greater than Low Range.", "OK");
            }
            else
                DisplayAlert("Alert", "High Range and Low Range must be valid numeric values.", "OK");
        }

        private void PkInstruments_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearOils();
        }

        private void PkTemperatures_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearOils();
        }

        private void Sc_OnChanged(object sender, ToggledEventArgs e)
        {
            SwitchCell s = (SwitchCell)sender;
            int pos = s.Text.IndexOf(' ');
            string stdName = s.Text.Substring(0, pos);

            if (e.Value)
            {
                int index = oils.FindIndex(x => x.Standard == stdName);
                if (index != -1)
                    oilsToCart.Add(oils[index]);
            }
            else
            {
                int index = oilsToCart.FindIndex(x => x.Standard == stdName);
                if (index != -1)
                    oilsToCart.RemoveAt(index);
            }
        }

        private void TxtHighRange_Unfocused(object sender, FocusEventArgs e)
        {
            ClearOils();

            if (!Double.TryParse(txtHighRange.Text, out m_dblHigh))     // Ensure tube range value is numeric
            {
                txtHighRange.Text = "";
                DisplayAlert("Alert", "High Range must be a valid numeric value.", "OK");
                txtHighRange.Focus();
            }
            else
            {
                if (m_dblHigh <= 0)                                     // Ensure numeric value is greater than 0 
                {
                    m_dblHigh = 0;
                    txtHighRange.Text = "";
                    DisplayAlert("Alert", "High Range a valid numeric number greater than 0.", "OK");
                    txtHighRange.Focus();
                }
            }

            // if we have valid numeric values for high and low we can allow user to find oils
            if (m_dblHigh != 0 && m_dblLow != 0)
                btnFindOils.IsEnabled = true;
        }

        private void TxtLowRange_Unfocused(object sender, FocusEventArgs e)
        {
            ClearOils();
            
            if (!Double.TryParse(txtLowRange.Text, out m_dblLow))        // Ensure tube range value is numeric
            {
                txtLowRange.Text = "";
                DisplayAlert("Alert", "Low Range must be a valid numeric value.", "OK");
                txtLowRange.Focus();
            }
            else
            {
                if (m_dblLow <= 0)                                      // Ensure numeric value is greater than 0
                {
                    m_dblLow = 0;
                    txtLowRange.Text = "";
                    DisplayAlert("Alert", "Low Range a valid numeric number greater than 0.", "OK");
                    txtLowRange.Focus();
                }
                else
                {
                    txtHighRange.Focus();
                }
            }

            // if we have valid numeric values for high and low we can allow user to find oils
            if (m_dblHigh != 0 && m_dblLow != 0)
                btnFindOils.IsEnabled = true;
        }

        private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (m_addedOilsCount < oilsToCart.Count)
                webView.Source = @"http://cannoninstrument.com/basket/addByQuickshop?stockCode=" + oilsToCart[m_addedOilsCount].StockCode + @".004&Quantity=1";
            else if (m_addedOilsCount == oilsToCart.Count)
                webView.Source = @"http://cannoninstrument.com/en/Checkout";
            else if (m_addedOilsCount == oilsToCart.Count + 1)
            {
                // Perform slide-in animation of main screen, start at the right hand side of the screen
                int startingPoint = (int)Math.Truncate(relLayout.Width + 1);
                stkWebNav.TranslateTo(startingPoint, 0, 0);
                await webView.TranslateTo(startingPoint, 0, 0);

                m_doneLoadingOils = true;
                m_navCount = 0;
                imgSpinner.IsVisible = false;
                stkOverlay.IsVisible = false;
                stkWebNav.IsVisible = true;
                webView.IsVisible = true;

                stkWebNav.TranslateTo(0, 0, 400);
                await webView.TranslateTo(0, 0, 400);
            }
            else
                m_navCount++;

            m_addedOilsCount++;
        }
        #endregion

        private void FindOils()
        {
            // Clear any old data from the oil table and oils to add to cart
            oilsToCart.Clear();
            int count = tblOils.Root.Count;
            for (int i = 0; i < count; i++)
                tblOils.Root.Remove(tblOils.Root[0]);
            tblOils.IsVisible = true;

            // Determine if it is a 10 fold or 100 fold range tube
            // 10 fold = 1 bulb tube, 100 fold = 2 bulb tube
            double dblBulb1High = m_dblHigh;
            if (m_dblHigh / m_dblLow > 10)
            {
                dblBulb1High = m_dblLow * 10;
                m_numBulbs = 2;
            }
            else
                m_numBulbs = 1;

            int oilsInRange = 0;
            bool bulb1HasOils = false;
            bool bulb2HasOils = false;

            foreach (ViscosityItem vi in oils)
            {
                double.TryParse(pkTemperatures.Items[pkTemperatures.SelectedIndex], out double temperature);

                SwitchCell sc = new SwitchCell();

                // Add oils in the bottom 10 fold range of the tube into Bulb 2 oils
                // If it is only a 1 bulb tube, then this becomes bulb 1
                if (vi.C == temperature && vi.Viscosity >= m_dblLow && vi.Viscosity <= dblBulb1High)
                {
                    // Add the table section for Bulb 2 oils if it doesn't already exist
                    if (!bulb2HasOils)
                    {
                        if (m_numBulbs == 2)
                            tblOils.Root.Insert(0, new TableSection("Bulb 2 Oils"));
                        else
                            tblOils.Root.Insert(0, new TableSection("Bulb 1 Oils"));
                        bulb2HasOils = true;
                    }


                    // Find the index of the table section for Bulb 1 oils
                    int index = -1;
                    for (int i = 0; i < tblOils.Root.Count; i++)
                    {
                        TableSection ts = tblOils.Root[i];
                        if ((ts.Title == "Bulb 2 Oils" && m_numBulbs == 2) || (ts.Title == "Bulb 1 Oils" && m_numBulbs == 1))
                            index = i;
                    }

                    // Add the switch cell to the table section for Bulb 1 oils unless it could not be found.
                    if (index == -1)
                    {
                        if (m_numBulbs == 2)
                            DisplayAlert("Alert", "Could not find table section for bulb 2 oils.", "OK");
                        else
                            DisplayAlert("Alert", "Could not find table section for bulb 1 oils.", "OK");
                    }
                    else
                    {
                        /*
                        Label lblViscName = new Label
                        {
                            Text = vi.Standard,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.CenterAndExpand
                        };

                        Label lblVisc = new Label
                        {
                            Text = vi.Viscosity.ToString(),
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.CenterAndExpand
                        };

                        Grid g = new Grid { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
                        g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        g.Children.Add(lblViscName, 0, 0);
                        g.Children.Add(lblVisc, 1, 0);
                        */
                        sc.Text = vi.Standard + " - " + vi.Viscosity + " cSt";
                        sc.On = true;
                        sc.OnChanged += Sc_OnChanged;
                        tblOils.Root[index].Add(sc);
                        //tblOils.Root[index].Add(new ViewCell { View = g });
                        oilsToCart.Add(vi);
                    }

                    oilsInRange = oilsInRange + 1;
                }


                // Add the oils for the top 10 fold range of the tube into Bulb 1 oils
                if (m_numBulbs == 2 && vi.C == temperature && vi.Viscosity > dblBulb1High && vi.Viscosity <= m_dblHigh)
                {
                    // Add the table section for Bulb 1 oils if it doesn't already exist
                    if (!bulb1HasOils)
                    {
                        if (tblOils.Root.Count == 0)
                            tblOils.Root.Insert(0, new TableSection("Bulb 1 Oils"));
                        else
                            tblOils.Root.Insert(1, new TableSection("Bulb 1 Oils"));
                        bulb1HasOils = true;
                    }

                    // Find the index of the table section for Bulb 1 oils
                    int index = -1;
                    for (int i = 0; i < tblOils.Root.Count; i++)
                    {
                        TableSection ts = tblOils.Root[i];
                        if (ts.Title == "Bulb 1 Oils")
                            index = i;
                    }


                    // Add the switch cell to the table section for Bulb 1 oils unless it could not be found.
                    if (index == -1)
                        DisplayAlert("Alert", "Could not find table section for bulb 1 oils.", "OK");
                    else
                    {
                        /*
                        Label lblViscName = new Label
                        {
                            Text = vi.Standard,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.CenterAndExpand
                        };

                        Label lblVisc = new Label
                        {
                            Text = vi.Viscosity.ToString(),
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.CenterAndExpand
                        };

                        Grid g = new Grid { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
                        g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        g.Children.Add(lblViscName, 0, 0);
                        g.Children.Add(lblVisc, 1, 0);
                        */
                        sc.Text = vi.Standard + " - " + vi.Viscosity + " cSt";
                        sc.On = true;
                        sc.OnChanged += Sc_OnChanged;
                        tblOils.Root[index].Add(sc);
                        //tblOils.Root[index].Add(new ViewCell { View = g});
                        oilsToCart.Add(vi);
                    }

                    oilsInRange = oilsInRange + 1;
                }
            
            }

            if (oilsInRange > 0)
            {
                tblOils.HeightRequest = (oilsInRange + 2) * 55;
                tblOils.IsVisible = true;
                lblContact.IsVisible = true;
                btnPurchaseOilsOnline.IsVisible = true;
            }
            else
                DisplayAlert("Alert", "Could not find oils in the specified tube range at the given temperature.", "OK");
        }

        private void ClearOils()
        {
            int count = tblOils.Root.Count;
            for(int i = 0; i < count; i++)
                tblOils.Root.Remove(tblOils.Root[0]);

            tblOils.IsVisible = false;
            lblContact.IsVisible = false;
            btnPurchaseOilsOnline.IsVisible = false;
        }

        private bool GetOilsFromFile()
        {
            try
            {
#if __IOS__
                var resourcePrefix = "OilSelector.iOS.";
#endif
#if __ANDROID__
                var resourcePrefix = "OilSelector.Droid.";
#endif
#if WINDOWS_PHONE
                var resourcePrefix = "OilSelector.WinPhone.";
#endif

                oils = new List<ViscosityItem>();
                var assembly = typeof(ViscosityItem).GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream(resourcePrefix + "oils.csv");
                using (var reader = new System.IO.StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        string oil = reader.ReadLine();
                        string[] data = oil.Split(',');
                        int id = 0;
                        double viscosity, c, f;
                        string standard;
                        string stockCode;

                        int.TryParse(data[0], out id);
                        double.TryParse(data[1], out viscosity);
                        double.TryParse(data[2], out c);
                        double.TryParse(data[3], out f);
                        standard = data[4];
                        stockCode = data[5];

                        ViscosityItem vi = new ViscosityItem();
                        vi.ID = id;
                        vi.Viscosity = viscosity;
                        vi.C = c;
                        vi.F = f;
                        vi.Standard = standard;
                        vi.StockCode = stockCode;
                        oils.Add(vi);
                    }
                }
                oils.Sort((x, y) => x.Viscosity.CompareTo(y.Viscosity));
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public int GetInstrumentSelection()
        {
            return pkInstruments.SelectedIndex;
        }

        public int GetTemperatureSelection()
        {
            return pkTemperatures.SelectedIndex;
        }
    }
}
