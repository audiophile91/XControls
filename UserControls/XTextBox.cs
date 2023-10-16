using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace XControls.UserControls
{
    public class XTextBox : TextBox
    {
        public XTextBox()
        {
            Loaded += Control_Loaded;
            PreviewLostKeyboardFocus += Control_PreviewLostKeyboardFocus;
            Text = "0";
        }

        #region Properties

        public bool LimitationInteger { get; set; }
        public double Maximum { get; set; }
        public double Minimum { get; set; }
        public uint LimitationPrecision { get; set; }
        public LimitationType LimitationType { get; set; }
        public LimitationValue LimitationValue { get; set; }

        #endregion

        #region Getters

        private bool IsTypeSet => LimitationType != LimitationType.Custom;

        #endregion

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsTypeSet)
            {
                SetType();
            }

            ValidateValue(this);
        }
        private void Control_PreviewLostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            if (sender is XTextBox textBox)
            {
                try
                {
                    double.Parse(textBox.Text);
                }
                catch
                {
                    MessageBoxResult result = MessageBox.Show("Data has incorrect format!\n" +
                                                              $"(Current separator is \"{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}\")\n" +
                                                              "Do you want to fix format?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        textBox.Focus();
                        e.Handled = true;
                        return;
                    }
                    textBox.Text = "0";
                }
                ValidateValue(textBox);
            }
        }

        private void SetType()
        {
            switch (LimitationType)
            {
                case LimitationType.Positive:
                    {
                        LimitationValue = LimitationValue.Minimum;
                        Minimum = 0;
                    }
                    break;

                case LimitationType.Negative:
                    {
                        LimitationValue = LimitationValue.Maximum;
                        Maximum = 0;
                    }
                    break;

                case LimitationType.Currency:
                    {
                        LimitationInteger = false;
                        LimitationValue = LimitationValue.Minimum;
                        Minimum = 0;
                        LimitationPrecision = 2;
                    }
                    break;

                case LimitationType.Percents:
                    {
                        LimitationInteger = false;
                        LimitationValue = LimitationValue.Range;
                        Minimum = 0;
                        Maximum = 100;
                    }
                    break;
            }
        }
        private void ValidateValue(XTextBox textBox)
        {
            double value = double.Parse(textBox.Text);

            if (LimitationInteger)
            {
                value = Math.Truncate(value);
            }
            else
            {
                if (LimitationPrecision > 0)
                {
                    value = Math.Round(value, (int)LimitationPrecision);
                }
            }
            switch (LimitationValue)
            {
                case LimitationValue.Minimum: value = Math.Max(Minimum, value); break;
                case LimitationValue.Maximum: value = Math.Min(Maximum, value); break;
                case LimitationValue.Range:
                    {
                        value = Math.Max(Minimum, value);
                        value = Math.Min(Maximum, value);
                    }
                    break;
            }
            textBox.Text = value.ToString();
        }
    }

    #region Enums

    /// <summary>
    /// Allows to set type in order to automaticly assign other limitations
    /// </summary>
    public enum LimitationType
    {
        /// <summary>
        /// Does not set anything.
        /// </summary>
        Custom,
        /// <summary>
        /// Sets value limitation to mininum 0.
        /// </summary>
        Positive,
        /// <summary>
        /// Sets value limitation to maximum 0.
        /// </summary>
        Negative,
        /// <summary>
        /// Sets value limitation to positive and precision to 2
        /// </summary>
        Currency,
        /// <summary>
        /// Sets value limitations to range from 0 to 100.
        /// </summary>
        Percents
    }
    /// <summary>
    /// Allows to set value limitation to groups: None, Posistive, Negative or Range
    /// </summary>
    public enum LimitationValue
    {
        /// <summary>
        /// No value limitations
        /// </summary>
        None,
        /// <summary>
        /// Sets value limitation to non lesser than declared in Minimum property.
        /// </summary>
        Minimum,
        /// <summary>
        /// Sets value limitation to non greater than declared in Maximum property.
        /// </summary>
        Maximum,
        /// <summary>
        /// Sets value limitation to non lesser than declared in Minimum property and non greater than declared in Maximum property.
        /// </summary>
        Range
    }

    #endregion
}