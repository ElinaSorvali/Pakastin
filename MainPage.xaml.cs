using System.IO;
using Microsoft.Maui.Controls;

namespace Pakastin
{
    public partial class MainPage : ContentPage
    {
        string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "pakastin.txt");
        string text = "";

        public MainPage()
        {
            InitializeComponent();
            LataaMerkinnat();
        }

        private void LataaMerkinnat()
        {
            if (File.Exists(fileName))
            {
                text = File.ReadAllText(fileName);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var merkinnat = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var merkinta in merkinnat)
                    {
                        LisaaTuote(merkinta.Trim()); // Trim remove any extra spaces/new lines
                    }
                }
                else
                {
                    outputLabel.Text = "Pakastin on vielä tyhjä.";
                }
            }
            else
            {
                outputLabel.Text = "Pakastin on vielä tyhjä.";
            }
        }

        private void lisaaTuote_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(inputKentta.Text))
            {
                // Muodosta päivämäärä halutussa muodossa
                string paivamaara = DateTime.Now.ToString("dd.MM.yyyy");

                // Lisää päivämäärä tuotteen eteen
                string uusiTuote = $" {paivamaara}: {inputKentta.Text}";
                LisaaTuote(uusiTuote);
                File.AppendAllText(fileName, uusiTuote + Environment.NewLine);
                outputLabel.Text = "";
                inputKentta.Text = "";
            }
            else
            {
                outputLabel1.Text = "Et voi lisätä tyhjää merkintää. Kirjoita jotain!";
            }
        }


        private void LisaaTuote(string tuote)
        {
            var poistaNappi = new Button
            {
                Text = "X",
                BackgroundColor = Colors.LightBlue,
                TextColor = Colors.Red,
                FontSize = 14,
                Padding = new Thickness(3),
                WidthRequest = 30,
                HeightRequest = 30
            };

            var stack = new StackLayout { Orientation = StackOrientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

            poistaNappi.Clicked += (s, e) => PoistaTuote(tuote, stack);
            stack.Children.Add(poistaNappi);
            var label = new Label { Text = tuote, FontSize = 18 };
            stack.Children.Add(label);

            tuoteLista.Children.Add(stack);
        }

        private void PoistaTuote(string tuote, StackLayout stack)
        {
            // Lue kaikki rivit tiedostosta
            var lines = File.ReadAllLines(fileName).ToList();

            // Poista tuote listasta
            lines.RemoveAll(line => line.Equals(tuote)); // Poistetaan kaikki rivit, jotka vastaavat tuotetta

            // Kirjoita takaisin päivitetyt rivit tiedostoon
            File.WriteAllLines(fileName, lines); // Päivitetään tiedosto

            // Poista myös rivi käyttöliittymästä
            tuoteLista.Children.Remove(stack);
        }

        private async void delete_Clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Vahvista poisto", "Haluatko varmasti tyhjentää kaikki tiedot?", "Kyllä", "Peruuta");

            if (result)
            {
                File.WriteAllText(fileName, "");
                tuoteLista.Children.Clear();
                outputLabel.Text = "Pakastimen sisältö tyhjennetty.";
            }
        }
    }
}
