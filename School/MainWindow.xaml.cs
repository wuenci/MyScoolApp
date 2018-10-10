using School.Data;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;



namespace School
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Connection to the School database
        private SchoolDBEntities schoolContext = null;

        // Field for tracking the currently selected teacher
        private Teacher teacher = null;

        // List for tracking the students assigned to the teacher's class
        private IList studentsInfo = null;

        #region Predefined code

        public MainWindow()
        {
            InitializeComponent();
        }

        // Connect to the database and display the list of teachers when the window appears
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.schoolContext = new SchoolDBEntities();
            teachersList.DataContext = this.schoolContext.Teachers;
        }

        // When the user selects a different teacher, fetch and display the students for that teacher
        private void teachersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Find the teacher that has been selected
            this.teacher = teachersList.SelectedItem as Teacher;
            this.schoolContext.LoadProperty<Teacher>(this.teacher, s => s.Students);

            // Find the students for this teacher
            this.studentsInfo = ((IListSource)teacher.Students).GetList();

            // Use databinding to display these students
            studentsList.DataContext = this.studentsInfo;
        }

        #endregion

        // When the user presses a key, determine whether to add a new student to a class, remove a student from a class, or modify the details of a student
        private void studentsList_KeyDown(object sender, KeyEventArgs e)
        {
            // TODO: Exercise 1: Task 1a: If the user pressed Enter, edit the details for the currently selected student

            switch (e.Key)
            {
                case Key.Enter:
                    EditStudent();
                    break;

                case Key.Insert:
                    NewStudent();
                    break;

                case Key.Delete:
                    DeleteStudent();
                    break;
            }
        }

        // TODO: Exercise 1: Task 2a: Use the StudentsForm to display and edit the details of the student
        // TODO: Exercise 1: Task 2b: Set the title of the form and populate the fields on the form with the details of the student
        // TODO: Exercise 1: Task 3a: Display the form
        // TODO: Exercise 1: Task 3b: When the user closes the form, copy the details back to the student
        // TODO: Exercise 1: Task 3c: Enable saving (changes are not made permanent until they are written back to the database)
        private void EditStudent()
        {
            Student selectedStudent = (Student)studentsList.SelectedItem;
            StudentForm studentForm = new StudentForm();
            studentForm.Title = "Modifica lo studente selezionato";
            studentForm.firstName.Text = selectedStudent.FirstName;
            studentForm.lastName.Text = selectedStudent.LastName;
            studentForm.dateOfBirth.Text = selectedStudent.DateOfBirth.ToString("d");
            if (studentForm.ShowDialog().Value)
            {
                selectedStudent.FirstName = studentForm.firstName.Text;
                selectedStudent.LastName = studentForm.lastName.Text;
                selectedStudent.DateOfBirth = DateTime.ParseExact(studentForm.dateOfBirth.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                saveChanges.IsEnabled = true;
            }

        }


        // TODO: Exercise 2: Task 1a: If the user pressed Insert, add a new student
        // TODO: Exercise 2: Task 2a: Use the StudentsForm to get the details of the student from the user
        // TODO: Exercise 2: Task 2b: Set the title of the form to indicate which class the student will be added to (the class for the currently selected teacher)
        // TODO: Exercise 2: Task 3a: Display the form and get the details of the new student
        // TODO: Exercise 2: Task 3b: When the user closes the form, retrieve the details of the student from the form and use them to create a new Student object
        // TODO: Exercise 2: Task 4a: Assign the new student to the current teacher
        // TODO: Exercise 2: Task 4b: Add the student to the list displayed on the form
        // TODO: Exercise 2: Task 4c: Enable saving (changes are not made permanent until they are written back to the database)
        private void NewStudent()
        {
            Student newStudent = new Student();
            StudentForm studentForm = new StudentForm();
            studentForm.Title = "inserire nuovo studente";

            if (studentForm.ShowDialog().Value)
            {
                newStudent.FirstName = studentForm.firstName.Text;
                newStudent.LastName = studentForm.lastName.Text;
                newStudent.DateOfBirth = DateTime.ParseExact(studentForm.dateOfBirth.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                teacher.Students.Add(newStudent);
                studentsInfo.Add(newStudent);

                saveChanges.IsEnabled = true;
            }
        }


        // TODO: Exercise 3: Task 1a: If the user pressed Delete, remove the currently selected student
        // TODO: Exercise 3: Task 2a: Prompt the user to confirm that the student should be removed
        // TODO: Exercise 3: Task 3a: If the user clicked Yes, remove the student from the database
        // TODO: Exercise 3: Task 3b: Enable saving (changes are not made permanent until they are written back to the database)
        private void DeleteStudent()
        {
            MessageBoxResult response = MessageBox.Show("Sei sicuro di volere cancellare lo studente selezionato?", "Conferma", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (response == MessageBoxResult.Yes)
            {
                Student selectedStudent = (Student)studentsList.SelectedItem;
                schoolContext.Students.DeleteObject(selectedStudent);
                saveChanges.IsEnabled = true;
            }
        }

        #region Predefined code

        private void studentsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditStudent();
        }

        // Save changes back to the database and make them permanent
        private void saveChanges_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Exercise 3: Task 2b: Save the changes by calling the SaveChanges method of the schoolContext object
            // TODO: Exercise 3: Task 3a: If an OptimisticConcurrencyException occurs then another user has changed the same students earlier, then overwrite their changes with the new data (see the lab instructions for details)
            // TODO: Exercise 3: Task 3b: If an UpdateException occurs then report the error to the user and rollback (see the lab instructions for details)
            // TODO: Exercise 3: Task 3c: If some other sort of error has occurs, report the error to the user and retain the data so the user can try again - the error may be transitory (see the lab instructions for details)
            StringBuilder messages = new StringBuilder();
            try
            {
                schoolContext.SaveChanges();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                messages.AppendLine("Optimistic concurrency exception");
                this.schoolContext.Refresh(RefreshMode.ClientWins, schoolContext);
            }
            catch (UpdateException updex)
            {
                messages.AppendLine("Update exception");
                this.schoolContext.Refresh(RefreshMode.StoreWins, schoolContext);

            }
            catch (Exception ex)
            {
                messages.AppendLine("È successo un Errore");
            }

            if (messages.Length > 0)
            {
                MessageBox.Show(messages.ToString());
            }

            saveChanges.IsEnabled = false;
        }

        #endregion

        private void addStudent_Click(object sender, RoutedEventArgs e)
        {
            NewStudent();
        }

        private void deleteStudent_Click(object sender, RoutedEventArgs e)
        {
            DeleteStudent();
        }
    }


    // Convert the date of birth provided in the value parameter and convert to the age of the student in years
    // TODO: Exercise 4: Task 2a: Check that the value provided is not null. If it is, return an empty string
    // TODO: Exercise 4: Task 2b: Convert the value provided into a DateTime value
    // TODO: Exercise 4: Task 2c: Work out the difference between the current date and the value provided
    // TODO: Exercise 4: Task 2d: Convert this result into a number of years
    // TODO: Exercise 4: Task 2e: Convert the number of years into a string and return it
}
