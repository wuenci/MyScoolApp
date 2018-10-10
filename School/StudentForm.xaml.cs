using System;
using System.Globalization;
using System.Text;
using System.Windows;

namespace School
{
    /// <summary>
    /// Interaction logic for StudentForm.xaml
    /// </summary>
    public partial class StudentForm : Window
    {
        #region Predefined code

        public StudentForm()
        {
            InitializeComponent();
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Exercise 2: Task 2a: Check that the user has provided a first name
            // TODO: Exercise 2: Task 2b: Check that the user has provided a last name
            // TODO: Exercise 2: Task 3a: Check that the user has entered a valid date for the date of birth
            // TODO: Exercise 2: Task 3b: Verify that the student is at least 5 years old

            StringBuilder messages = new StringBuilder();

            if (string.IsNullOrEmpty(firstName.Text.Trim()))
            {
                messages.AppendLine("Inserire un nome");
            }

            if (string.IsNullOrEmpty(lastName.Text.Trim()))
            {
                messages.AppendLine("Inserire un cognome!");
            }



            if (DateTime.TryParseExact(dateOfBirth.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirthOut))
            {
                if (dateOfBirthOut > DateTime.Now)
                {
                    messages.AppendLine("Lo studente non può esere nato prima di oggi!");
                }
                if (Utils.AgeConverter.GetAge(dateOfBirthOut) < 5)
                {
                    messages.AppendLine("Lo studente deve avere almeno 5 anni!");
                }
            }
            else
            {
                messages.AppendLine("Inserire una data di nascita!");
            }

            if(messages.Length > 0)
            {
                MessageBox.Show(messages.ToString());
                return;
            }

            this.DialogResult = true;
        }

        #endregion
    }
}
