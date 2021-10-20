using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CalculateDistances
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Regex InputCheck;
        private readonly Dictionary<string, double> mappingFromUnitsToValues;
        private bool FirstClick = false;
        private const string UserInputPattern =
            @"^(\s*\d+([.]{0,1}\d+){0,1}\s*([eE]\s*[+-]{0,1}\s*\d+){0,1}\s*(([dcmμnpfazyhkMGTPEZY]{0,1}m)|dam)\s*){1}(\s*[+-]{0,1}\s*\d+([.]{0,1}\d+){0,1}\s*([eE]\s*[+-]{0,1}\s*\d+){0,1}\s*(([dcmμnpfazyhkMGTPEZY]{0,1}m)|dam)\s*)*$";
        public MainWindow()
        {
            InputCheck = new Regex(UserInputPattern);

            mappingFromUnitsToValues = new Dictionary<string, double>();
            mappingFromUnitsToValues["ym"] = Math.Pow(10d, -24d);
            mappingFromUnitsToValues["zm"] = Math.Pow(10d, -21d);
            mappingFromUnitsToValues["am"] = Math.Pow(10d, -18d);
            mappingFromUnitsToValues["fm"] = Math.Pow(10d, -15d);
            mappingFromUnitsToValues["pm"] = Math.Pow(10d, -12d);
            mappingFromUnitsToValues["nm"] = Math.Pow(10d, -9d);
            mappingFromUnitsToValues["μm"] = Math.Pow(10d, -6d);
            mappingFromUnitsToValues["mm"] = Math.Pow(10d, -3d);
            mappingFromUnitsToValues["cm"] = Math.Pow(10d, -2d);
            mappingFromUnitsToValues["dm"] = Math.Pow(10d, -1d);
            mappingFromUnitsToValues["m"] = 1d;
            mappingFromUnitsToValues["dam"] = Math.Pow(10d, 1d);
            mappingFromUnitsToValues["hm"] = Math.Pow(10d, 2d);
            mappingFromUnitsToValues["km"] = Math.Pow(10d, 3d);
            mappingFromUnitsToValues["Mm"] = Math.Pow(10d, 6d);
            mappingFromUnitsToValues["Gm"] = Math.Pow(10d, 9d);
            mappingFromUnitsToValues["Tm"] = Math.Pow(10d, 12d);
            mappingFromUnitsToValues["Pm"] = Math.Pow(10d, 15d);
            mappingFromUnitsToValues["Em"] = Math.Pow(10d, 18d);
            mappingFromUnitsToValues["Zm"] = Math.Pow(10d, 21d);
            mappingFromUnitsToValues["Ym"] = Math.Pow(10d, 24d);

            InitializeComponent();
            UsersInput.AddHandler(TextBox.MouseLeftButtonDownEvent, new MouseButtonEventHandler(UsersInput_OnMouseLeftButtonDown), true);
        }

        /// <summary>
        /// Extracts number from string
        /// </summary>
        /// <param name="numberAndUnit"></param>
        /// <returns>number</returns>
        private decimal ExtractNumber(string numberAndUnit)
        {
            string stringWithUnit = Regex.Replace(numberAndUnit, @"(([dcmμnpfazyhkMGTPEZY]{0,1}m)|dam){0,1}", "");
            return (decimal)Double.Parse(stringWithUnit);
        }

        /// <summary>
        /// Extracts unit from string
        /// </summary>
        /// <param name="numberAndUnit">number and unit as string</param>
        /// <returns>unit in relevance to meter unit</returns>
        private decimal ExtractUnit(string numberAndUnit)
        {
            string[] stringWithUnit = Regex.Split(numberAndUnit, "[0-9]+");
            return (decimal)mappingFromUnitsToValues[stringWithUnit[stringWithUnit.Length - 1]];
        }

        /// <summary>
        /// Calculates given expression
        /// </summary>
        /// <param name="inputString">users input</param>
        private void CalculateResult(string inputString)
        {
            var usersInput = inputString.Replace(" ", "");
            var splitNumbers = usersInput.Split(new string[] { "m+", "m-" }, StringSplitOptions.None);

            for (int i = 0; i < splitNumbers.Length-1; i++)
            {
                splitNumbers[i] = splitNumbers[i] + "m";
            }

            var result = this.ExtractNumber(splitNumbers[0]) * ExtractUnit(splitNumbers[0]);
            int currentLength = splitNumbers[0].Length;

            for (int i = 1; i < splitNumbers.Length; i++)
            {
                switch (usersInput[currentLength])
                {
                    case '+':
                        result += this.ExtractNumber(splitNumbers[i]) * ExtractUnit(splitNumbers[i]);
                        break;
                    case '-':
                        result -= this.ExtractNumber(splitNumbers[i]) * ExtractUnit(splitNumbers[i]);
                        break;
                }
                currentLength += splitNumbers[i].Length + 1;
            }

            ComboBoxItem  comboItem= this.ResultUnit.SelectedItem as ComboBoxItem;
            string resultUnit = comboItem.Content.ToString();
            Result.Content = $"Result is {result / (decimal)mappingFromUnitsToValues[resultUnit]} {resultUnit}";
        }

        /// <summary>
        /// When calculate button is click it triggers calculation of the user's expression
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button's args</param>
        private void Calculate_OnClick(object sender, RoutedEventArgs e)
        {
            if (InputCheck.IsMatch(UsersInput.Text))
            {
                UsersInput.BorderThickness = new Thickness(1.0d);
                UsersInput.BorderBrush = Brushes.Green;
                CalculateResult(UsersInput.Text);
                Result.Foreground = Brushes.Black;
            }
            else
            {
                UsersInput.BorderThickness = new Thickness(6.0d);
                UsersInput.BorderBrush = Brushes.Red;
                Result.Content = "Invalid input";
                Result.Foreground = Brushes.Red;
            }
        }

        /// <summary>
        /// When user first select textBox string with explanation is deleted
        /// </summary>
        /// <param name="sender">textBox</param>
        /// <param name="e">arguments</param>
        private void UsersInput_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!FirstClick)
            {
                UsersInput.Text = "";
                FirstClick = true;
            }
        }
    }
}
