using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using WpfApp1.Models;
using System.Diagnostics;

namespace WpfApp1
{
    public  class DataService
    {


        public static Test LoadQuestions(string _filePath)
        {
            if (!File.Exists(_filePath))
                return new Test();

            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<Test>(json);
        }

        public static void SaveQuestions(Test questions, string _filePath)
        {
            string json = JsonConvert.SerializeObject(questions, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
        public static ObservableCollection<Test> LoadTests(string _filePath)
        {
            ObservableCollection<Test> tests = new ObservableCollection<Test>();
            if (Directory.Exists(_filePath))
            {
                foreach(var file in Directory.GetFiles(_filePath,"*.json")) { 
                    tests.Add(LoadQuestions(file));

                }
            }
            if (tests.Count == 0)
            {
                Debug.WriteLine("СПИСОК ПУСТ");
            }
            else
            {
                Debug.WriteLine($"{tests.Count} tests");
            }
            return tests;
        }
    }
}
