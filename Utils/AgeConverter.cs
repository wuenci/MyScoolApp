using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Utils
{
    [ValueConversion(typeof(string), typeof(Decimal))]
    public class AgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
        {
            // Convert the date of birth provided in the value parameter and convert to the age of the student in years
            // Check that the value provided is not null. If it is, return an empty string
            if (value != null)
            {
                // Convert the value provided into a DateTime value
                DateTime studentDateOfBirth = (DateTime)value;
                return AgeConverter.GetAge(studentDateOfBirth);
            }
            else
            {
                return "";
            }
        }

        public static int GetAge(DateTime dateOfBirth)
        {
            int age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now < dateOfBirth.AddYears(age))
            {
                age--;
            }
            return age;
        }

        #region Predefined code

        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}