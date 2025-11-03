using System.Collections.ObjectModel;

namespace gui;

public partial class MainPage : ContentPage
{
    private ObservableCollection<Produkt> produkty = new ObservableCollection<Produkt>();
    private string sciezkaCSV = Path.Combine(FileSystem.AppDataDirectory, "produkty.csv");

    public MainPage()
    {
        InitializeComponent();
        ProductsListView.ItemsSource = produkty;
        WczytajCSV();
    }

    private void WczytajCSV()
    {
        try
        {
            if (File.Exists(sciezkaCSV))
            {
                produkty.Clear();
                string[] linie = File.ReadAllLines(sciezkaCSV);
                foreach (string linia in linie)
                {
                    string[] czesci = linia.Split(',');
                    if (czesci.Length >= 5)
                    {
                        produkty.Add(new Produkt
                        {
                            Id = int.Parse(czesci[0]),
                            Nazwa = czesci[1],
                            Cena = double.Parse(czesci[2]),
                            Kategoria = czesci[3],
                            Utworzono = czesci[4]
                        });
                    }
                }
            }
        }
        catch (Exception e)
        {
            DisplayAlert("Błąd", "Nie udało się wczytać: " + e.Message, "OK");
        }
    }

    private void ZapiszCSV()
    {
        try
        {
            List<string> linie = new List<string>();
            foreach (var p in produkty)
            {
                linie.Add($"{p.Id},{p.Nazwa},{p.Cena},{p.Kategoria},{p.Utworzono}");
            }
            File.WriteAllLines(sciezkaCSV, linie);
            DisplayAlert("Sukces", "Zapisano do CSV!", "OK");
        }
        catch (Exception e)
        {
            DisplayAlert("Błąd", "Nie udało się zapisać: " + e.Message, "OK");
        }
    }

    private async void OnDodajClicked(object sender, EventArgs e)
    {
        string nazwa = await DisplayPromptAsync("Dodaj produkt", "Podaj nazwę:");
        if (string.IsNullOrWhiteSpace(nazwa)) return;

        string cenaText = await DisplayPromptAsync("Dodaj produkt", "Podaj cenę:", keyboard: Keyboard.Numeric);
        if (!double.TryParse(cenaText, out double cena)) cena = 0;

        string kategoria = await DisplayPromptAsync("Dodaj produkt", "Podaj kategorię:");

        
        int maxId = produkty.Count > 0 ? produkty.Max(p => p.Id) : 0;

        produkty.Add(new Produkt
        {
            Id = maxId + 1,
            Nazwa = nazwa,
            Cena = cena,
            Kategoria = kategoria,
            Utworzono = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        });
    }

    private async void OnEdytujClicked(object sender, EventArgs e)
    {
        var selected = ProductsListView.SelectedItem as Produkt;
        if (selected == null)
        {
            await DisplayAlert("Błąd", "Wybierz produkt!", "OK");
            return;
        }

        string nazwa = await DisplayPromptAsync("Edytuj produkt", "Podaj nazwę:", initialValue: selected.Nazwa);
        if (!string.IsNullOrWhiteSpace(nazwa)) selected.Nazwa = nazwa;

        string cenaText = await DisplayPromptAsync("Edytuj produkt", "Podaj cenę:",
            initialValue: selected.Cena.ToString(), keyboard: Keyboard.Numeric);
        if (double.TryParse(cenaText, out double cena)) selected.Cena = cena;

        string kategoria = await DisplayPromptAsync("Edytuj produkt", "Podaj kategorię:",
            initialValue: selected.Kategoria);
        selected.Kategoria = kategoria;

       
        ProductsListView.ItemsSource = null;
        ProductsListView.ItemsSource = produkty;
    }

    private async void OnUsunClicked(object sender, EventArgs e)
    {
        var selected = ProductsListView.SelectedItem as Produkt;
        if (selected == null)
        {
            await DisplayAlert("Błąd", "Wybierz produkt!", "OK");
            return;
        }

        bool answer = await DisplayAlert("Usuń", $"Usunąć {selected.Nazwa}?", "Tak", "Nie");
        if (answer)
        {
            produkty.Remove(selected);
            await DisplayAlert("Sukces", "Usunięto!", "OK");
        }
    }

    private void OnZapiszClicked(object sender, EventArgs e)
    {
        ZapiszCSV();
    }
}

public class Produkt
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = string.Empty;
    public double Cena { get; set; }
    public string Kategoria { get; set; } = string.Empty;
    public string Utworzono { get; set; } = string.Empty;
}