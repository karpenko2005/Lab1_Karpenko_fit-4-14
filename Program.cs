using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Lab1_Console
{
    // Mock class for Microsoft.Maui.Controls.Command to make it work in Console without MAUI installation
    public class Command : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public Command(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute();
        public void Execute(object? parameter) => _execute();
        public event EventHandler? CanExecuteChanged;
    }

    public class Student
    {
        public string FullName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public double AverageScore { get; set; }
    }

    public class StudentViewModel : INotifyPropertyChanged
    {
        private Student _student = new();

        public ObservableCollection<Student> Students { get; } = new();
        public ICommand AddStudentCommand { get; }

        public string FullName
        {
            get => _student.FullName;
            set
            {
                if (_student.FullName != value)
                {
                    _student.FullName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Greeting));
                }
            }
        }

        public string Group
        {
            get => _student.Group;
            set
            {
                if (_student.Group != value)
                {
                    _student.Group = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Greeting));
                }
            }
        }

        public double AverageScore
        {
            get => _student.AverageScore;
            set
            {
                if (_student.AverageScore != value)
                {
                    _student.AverageScore = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StatusColor));
                }
            }
        }

        public string Greeting => !string.IsNullOrWhiteSpace(FullName) && !string.IsNullOrWhiteSpace(Group)
            ? $"Student: {FullName}, Group: {Group}"
            : "Data is empty";

        public string StatusColor => AverageScore >= 4.0 ? "GREEN" : "RED";

        public StudentViewModel()
        {
            AddStudentCommand = new Command(AddStudent, CanAddStudent);
            Students.CollectionChanged += Students_CollectionChanged;
        }

        private void AddStudent()
        {
            Students.Add(new Student
            {
                FullName = FullName,
                Group = Group,
                AverageScore = AverageScore
            });

            FullName = string.Empty;
            Group = string.Empty;
            AverageScore = 0;
        }

        private bool CanAddStudent() => !string.IsNullOrWhiteSpace(FullName);

        private void Students_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Console.WriteLine("\n[Collection Notification]: CollectionChanged triggered! UI updated with new student.");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Lab 1 Part 1 & Part 2 Execution ===");
            
            var viewModel = new StudentViewModel();

            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.Greeting))
                {
                    Console.WriteLine($"[UI Binding Mode=OneWay]: {viewModel.Greeting}");
                }
                if (e.PropertyName == nameof(viewModel.StatusColor))
                {
                    Console.WriteLine($"[UI Color Converter Simulated]: Color is {viewModel.StatusColor}");
                }
            };

            Console.WriteLine("\nStep 1: UI input fields change (TwoWay Data Binding)...");
            viewModel.FullName = "Ivanov Ivan"; 
            viewModel.Group = "KI-21-1"; 
            viewModel.AverageScore = 4.5; 

            Console.WriteLine("\nStep 2: Checking ICommand CanExecute logic...");
            if (viewModel.AddStudentCommand.CanExecute(null))
            {
                Console.WriteLine("Button Status: Enabled (FullName is not empty). Executing Command...");
                viewModel.AddStudentCommand.Execute(null);
            }

            Console.WriteLine("\nStep 3: Checking CollectionView ItemsSource Binding...");
            foreach (var student in viewModel.Students)
            {
                Console.WriteLine($"[CollectionView Item]: Name: {student.FullName} | Group: {student.Group} | Score: {student.AverageScore}");
            }

            Console.WriteLine("\nExecution successful. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
