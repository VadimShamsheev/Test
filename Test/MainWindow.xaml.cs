using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.ComponentModel;

using Microsoft.EntityFrameworkCore;

namespace Test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Test();
        }

        public class ApplicationContext : DbContext
        {
            public DbSet<test> test { get; set; }
            public ApplicationContext()
            {
                Database.EnsureCreated();
            }
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseMySQL(
                    "server=127.0.0.1;port=3307;user=root;password=root;database=test;"
                    );
            }
            public static void AddingMethod(int id, string title1, string title2)
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    test t = new test()
                    {
                        Id = id,
                        Title = title1,
                        Title2 = title2
                    };
                    db.test.Add(t);
                    db.SaveChanges();
                }
            }
            public static void EditingMethod(int id, string title1, string title2)
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var t = db.test.Single(i => i.Id == id);
                    t.Title = title1;
                    t.Title2 = title2;
                    db.SaveChanges();
                }
            }
        }

        public void Test()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                List<test> tests = new List<test>();
                tests = db.test.ToList();

                gridTitle.ItemsSource = tests;
                
            }
        }

        public class test : IEditableObject
        {
          
            public int Id { get; set; }
            public string Title { get; set; }
            public string Title2 { get; set; }

            struct backupDataStruct
            {
                public int Id { get; set; }
                public string Title { get; set; }
                public string Title2 { get; set; }
            }
            
            private backupDataStruct backupData;

            private bool isEditing = false;

            void IEditableObject.BeginEdit()
            {
                if (!isEditing)
                {
                    this.backupData.Id = Id;
                    this.backupData.Title = Title;
                    this.backupData.Title2 = Title2;
                    isEditing = true;
                    MessageBox.Show("Начато редактирование данных преподавателя " + this.Title + this.Title2 + ".");
                }
            }
            void IEditableObject.CancelEdit()
            {
                if (isEditing)
                {
                    this.Id = this.backupData.Id;
                    this.Title = this.backupData.Title;
                    this.Title2 = this.backupData.Title2;
                    isEditing = false;
                    MessageBox.Show("Изменение отменено. Данные восстановлены к изначальному состоянию.");
                }
            }
            void IEditableObject.EndEdit()
            {
                if (isEditing)
                {
                    try
                    {
                        ApplicationContext.EditingMethod(this.Id, this.Title, this.Title2);
                        MessageBox.Show("Сохранение прошло успешно!");
                    }
                    catch (Exception e)
                    {
                        this.Id = this.backupData.Id;
                        this.Title = this.backupData.Title;
                        this.Title2 = this.backupData.Title2;
                        isEditing = false;
                        MessageBox.Show("Произошла ошибка при сохранении! Изменения отменены!\nСведения об ошибке " 
                            + e.Message);
                    }
                }
            }
        }
    }
}
