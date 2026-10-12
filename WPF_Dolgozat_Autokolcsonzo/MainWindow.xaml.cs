using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Dolgozat_Autokolcsonzo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Autókölcsönző - Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInfo(string message)
        {
            MessageBox.Show(message, "Autókölcsönző - Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool ShowConfirm(string message)
        {
            return MessageBoxResult.OK == MessageBox.Show(message, "Autókölcsönző - Megerősítés", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        }

        private bool ValidateString(string text, int min, int max, out string error)
        {
            error = "";

            if (text.Length == 0 && min > 0)
            {
                error = $"A szöveg nem lehet üres!";
                return false;
            }

            if (text.Length < min)
            {
                error = $"A szöveg hossza minimum {min} karakter!";
                return false;
            }

            if (text.Length > max)
            {
                error = $"A szöveg hossza maximum {max} karakter!";
                return false;
            }

            return true;
        }

        private bool ValidateNumber(string numberString, int min, int max, out int number, out string error)
        {
            error = "";
            number = 0;

            try
            {
                number = Convert.ToInt32(numberString);
            }
            catch (FormatException)
            {
                error = "A szám formátuma helytelen!";
                return false;
            }
            catch (OverflowException)
            {
                error = "A szám meghaladja az elfogadott számérték határt!";
                return false;
            }

            if (number < min)
            {
                error = $"A szám minimális elfogadott értéke a {min}.";
                return false;
            }

            if (number > max)
            {
                error = $"A szám maximális elfogadott értéke a {max}.";
                return false;
            }

            return true;
        }

        private int GateChecked(RadioButton button, int value)
        {
            if (button.IsChecked ?? false)
            {
                return value;
            }

            return 0;
        }

        private int GateChecked(CheckBox button, int value)
        {
            if (button.IsChecked ?? false)
            {
                return value;
            }

            return 0;
        }

        private T SelectFromList<T>(T[] items, bool?[] conditions)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (conditions[i] ?? false)
                {
                    return items[i];
                }
            }

            return items[0];
        }

        private List<string> FilterList(string[] items, bool?[] conditions)
        {
            List<string> filtered = new();

            for (int i = 0; i < items.Length; i++)
            {
                if (conditions[i] ?? false)
                {
                    filtered.Add(items[i]);
                }
            }

            return filtered;
        }

        private bool AnyChecked(bool?[] items)
        {
            foreach (bool? item in items)
            {
                if (item ?? false)
                {
                    return true;
                }
            }

            return false;
        }

        private void Purchase_Clicked(object sender, RoutedEventArgs e)
        {
            string error;

            int userAge;
            int rentalLength;

            if (!ValidateString(inputName.Text, 3, int.MaxValue, out error))
            {
                inputName.Focus();
                ShowError($"Név: {error}");
                return;
            }

            if (!ValidateNumber(inputAge.Text, 18, 100, out userAge, out error))
            {
                inputAge.Focus();
                ShowError($"Életkor: {error}");
                return;
            }

            if (!ValidateString(inputLicenseNumber.Text, 6, int.MaxValue, out error))
            {
                inputLicenseNumber.Focus();
                ShowError($"Jogosítvány szám: {error}");
                return;
            }

            if (comboCarModel.SelectedIndex == 0)
            {
                ShowError("Autó: Válasszon egy autómodellt!");
                return;
            }

            if (!ValidateNumber(inputRentalLength.Text, 1, 30, out rentalLength, out error))
            {
                inputRentalLength.Focus();
                ShowError($"Bérlés időtartam: {error}");
                return;
            }

            if (!AnyChecked([radioInsuranceBase.IsChecked,
                             radioInsuranceExtra.IsChecked,
                             radioInsuranceFull.IsChecked]))
            {
                ShowError("Biztosítás: Válasszon egy biztosítási opciót!");
                return;
            }

            if (!AnyChecked([radioPayCash.IsChecked,
                             radioPayCard.IsChecked]))
            {
                ShowError("Fizetési mód: Válasszon egy fizetési módot!");
                return;
            }

            if (!(checkTerms.IsChecked ?? false))
            {
                ShowError("A fizetési feltételek elfogadása kötelező!");
                return;
            }

            (string, int)[] carModelData =
            {
                ("Suzuki Swift", 12_000),
                ("Toyota Corolla", 18_000),
                ("BMW 3", 30_000),
                ("Tesla Model 3", 35_000)
            };

            string[] extraNames =
            {
                "GPS",
                "Gyerekülés",
                "Extra sofőr",
                "Autópálya matrica"
            };

            (string, int)[] insuranceData =
            {
                ("Alap biztosítás", 0),
                ("Extra biztosítás", 3_000),
                ("Teljes biztosítás", 6_000)
            };

            string[] paymentMethodNames =
            {
                "Készpénz",
                "Bankkártya"
            };

            (string, int) selectedCar = carModelData[comboCarModel.SelectedIndex - 1];
            (string, int) selectedInsurance = SelectFromList(insuranceData, [radioInsuranceBase.IsChecked, radioInsuranceExtra.IsChecked, radioInsuranceFull.IsChecked]);

            int dailyCost = selectedCar.Item2 +

                            GateChecked(radioInsuranceBase, 0) +
                            GateChecked(radioInsuranceExtra, 3_000) +
                            GateChecked(radioInsuranceFull, 6_000) +

                            GateChecked(checkGPS, 1_000) +
                            GateChecked(checkSeat, 1_500) +
                            GateChecked(checkDriver, 2_500);

            int finalCost = dailyCost * rentalLength +
                            GateChecked(checkSticker, 5_000);

            List<string> selectedExtras = FilterList(extraNames, [checkGPS.IsChecked, checkSeat.IsChecked, checkDriver.IsChecked, checkSticker.IsChecked]);
            string extraString = selectedExtras.Count > 0 ? string.Join(", ", selectedExtras) : "-";

            string summaryMessage =
                    $"===== Rendelés Összefoglalő =====\n\n" +
                    $"Ügyfél:\n" +
                    $"  Név: {inputName.Text}\n" +
                    $"  Életkor: {inputAge.Text}\n" +
                    $"  Jogosítvány szám: {inputLicenseNumber.Text}\n\n" +
                    $"Bérlés:\n" +
                    $"  Autómodell: {selectedCar.Item1} ({selectedCar.Item2} Ft / nap)\n" +
                    $"  Időtartam: {inputRentalLength.Text} nap\n" +
                    $"  Biztosítás: {selectedInsurance.Item1} (+{selectedInsurance.Item2} Ft)\n\n" +
                    $"Extrák: {extraString}\n\n" +
                    $"Fizetés:\n" +
                    $"  Mód: {SelectFromList(paymentMethodNames, [radioPayCash.IsChecked, radioPayCard.IsChecked])}\n" +
                    $"  Végösszeg: {finalCost} Ft";

            if (ShowConfirm(summaryMessage))
            {
                ShowInfo($"A rendelése sikeresen megtörtént!\n\n{summaryMessage}");
            }
        }
    }
}