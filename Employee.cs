using System;
using System.Collections.Generic;
using System.IO;

namespace LR6
{
    public struct TEmployee
    {
        public string FIO;
        public string Phone;
        public string Department;
        public decimal Salary;
        public DateTime HireDate;
    }

    public struct EmployeeManager
    {
        public static string InputString(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        public static int ReadIntInRange(string prompt, int minVal, int maxVal)
        {
            while (true)
            {
                string input = InputString(prompt);
                if (int.TryParse(input, out int result))
                {
                    if (result >= minVal && result <= maxVal)
                    { return result; }
                    else { Console.WriteLine($"Error: value must be in range [{minVal}..{maxVal}]."); }
                }
                else { Console.WriteLine($"Error: invalid input, please enter a NUMBER"); }
            }
        }

        //Функции валидации
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            string[] parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3) return false;
            foreach (string part in parts)
            {
                int lettersCount = 0;
                int hyphenCount = 0;
                for (int i = 0; i < part.Length; i++)
                {
                    char c = part[i];
                    if (char.IsLetter(c))
                    {
                        lettersCount++;
                    }
                    else if (c == '-')
                    {
                        hyphenCount++;
                        if (i == 0 || i == part.Length - 1 || part[i - 1] == '-') { return false; }
                        if (hyphenCount > 1) { return false; }
                    }
                    else { return false; }
                }
                if (lettersCount < 2) return false;
            }
            return true;
        }

        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            bool startsWithPlus7 = phone.StartsWith("+7") && phone.Length == 12;
            bool startsWith8 = phone.StartsWith("8") && phone.Length == 11;
            if (!startsWithPlus7 && !startsWith8) return false;

            int startIndex = startsWithPlus7 ? 2 : 1;
            for (int i = startIndex; i < phone.Length; i++)
            {
                if (!char.IsDigit(phone[i])) return false;
            }
            return true;
        }

        public static bool IsValidDepartment(string  department)
        {
            if (string.IsNullOrWhiteSpace(department)) return false;
            bool ContainsDigits = department.All(char.IsAsciiDigit);
            if (ContainsDigits == true) return false;
            return true;
        }

        public static bool IsValidSalary(decimal salary)
        {
            const decimal SubsistenceMinimun = 28940m;
            if (salary < SubsistenceMinimun)
            {
                Console.WriteLine($"You can't assign salary less than the subsistence minimum {SubsistenceMinimun}");
                return false;
            }
            return true;
        }

        public static bool IsValidHireDate(DateTime date)
        {
            if (date > DateTime.Now) return false;
            return true;
        }

        //Работа с файлами
        public static List<TEmployee> LoadFromTextFile(string path)
        {
            List<TEmployee> list = new List<TEmployee>();
            if (!File.Exists(path))
            {
                Console.WriteLine($"Warning: text file {path} not found!");
                return list;
            }
            string[] lines = File.ReadAllLines(path);

            for (int i = 0; i + 4 < lines.Length; i += 5)
            {
                string fio = lines[i].Trim();
                string phone = lines[i + 1].Trim();
                string dept = lines[i + 2].Trim();
                string salaryStr = lines[i + 3].Trim();
                string dateStr = lines[i + 4].Trim();
                
                salaryStr = salaryStr.Replace(',', '.');
                bool isSalaryParsed = decimal.TryParse(salaryStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal salary);
                bool isDateParsed = DateTime.TryParse(dateStr, out DateTime hireDate);

                if (IsValidName(fio) &&
                    IsValidPhone(phone) &&
                    IsValidDepartment(dept) &&
                    isSalaryParsed && IsValidSalary(salary) &&
                    isDateParsed && IsValidHireDate(hireDate))
                {
                    list.Add(new TEmployee
                    {
                        FIO = fio,
                        Phone = phone,
                        Department = dept,
                        Salary = salary,
                        HireDate = hireDate
                    });
                }
                else
                {
                    Console.WriteLine($"Warning: skipped invalid record at lines {i + 1}-{i + 5}");
                }
            }
            if (lines.Length % 5 != 0)
            {
                Console.WriteLine("Warning: last record is incomplete or broken.");
            }
            return list;
        }

        public static void SaveToBinaryFile(string path, List<TEmployee> list)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                foreach(var emp in list)
                {
                    writer.Write(emp.FIO);
                    writer.Write(emp.Phone);
                    writer.Write(emp.Department);
                    writer.Write(emp.Salary);
                    writer.Write(emp.HireDate.ToBinary());
                }
            }
        }

        public static List<TEmployee> LoadFromBinaryFile(string path)
        {
            List<TEmployee> list = new List<TEmployee>();
            if (!File.Exists(path)) return list;

            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    string fio = reader.ReadString();
                    string phone = reader.ReadString();
                    string dept = reader.ReadString();
                    decimal salary = reader.ReadDecimal();
                    long dateBin = reader.ReadInt64();
                    DateTime hireDate = DateTime.FromBinary(dateBin);
                    list.Add(new TEmployee{ FIO = fio,
                                            Phone=phone,
                                            Department = dept,
                                            Salary = salary,
                                            HireDate = hireDate});
                }
            }
            return list;
        }

        public static void ShowTable(List<TEmployee> list)
        {
            Console.WriteLine("\n+----+------------------------------------------+------------------+------------------------------------------+------------+-------------+");
            Console.WriteLine("|#   |FIO                                       |Phone             |Department                                |Salary      |Hire Date    |");
            Console.WriteLine("+----+------------------------------------------+------------------+------------------------------------------+------------+-------------+");

            if (list.Count == 0)
            {
                Console.WriteLine("| -  | (empty)                                  | (empty)          |");
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Console.WriteLine($"| {i + 1,-2} | {list[i].FIO,-40} | {list[i].Phone,-16} | {list[i].Department,-40} | {list[i].Salary,-10} | {list[i].HireDate,-11:dd.MM.yyyy} |");
                }
            }
            Console.WriteLine("+----+------------------------------------------+------------------+------------------------------------------+------------+-------------+");
        }

        //"Рабочий процесс"
        public static void AddRecord(List<TEmployee> list, string binPath)
        {
            string fio, phone, dept;
            decimal salary;
            DateTime hireDate;

            while (true)
            {
                fio = InputString("Enter full name: ");
                if (IsValidName(fio)) break;
                Console.WriteLine("Invalid name. Must be 3 words, min 2 letters each.");
            }
            while (true)
            {
                phone = InputString("Enter phone (+7... or 8...): ");
                if (IsValidPhone(phone)) break;
                Console.WriteLine("Invalid phone.");
            }
            while (true)
            {
                dept = InputString("Enter department: ");
                if (IsValidDepartment(dept)) break;
                Console.WriteLine("Invalid department. Must be non-empty text.");
            }
            while (true)
            {
                string salaryStr = InputString("Enter salary: ").Replace(',', '.');
                if (decimal.TryParse(salaryStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out salary)
                    && IsValidSalary(salary))
                    break;
                Console.WriteLine("You can't assign salary less than the subsistence minimum or invalid format.");
            }
            while (true)
            {
                string dateStr = InputString("Enter hire date (dd.mm.yyyy): ");
                if (DateTime.TryParse(dateStr, out hireDate) && IsValidHireDate(hireDate)) break;
                Console.WriteLine("Invalid date. Cannot be in the future.");
            }
            list.Add(new TEmployee { FIO = fio, Phone = phone, Department = dept, Salary = salary, HireDate = hireDate });
            SaveToBinaryFile(binPath, list);
            Console.WriteLine("Record added and saved.");
        }

        public static void DeleteRecord(List<TEmployee> list, string binPath)
        {
            if (list.Count == 0) { Console.WriteLine("Empty list."); return; }
            ShowTable(list);
            int num = ReadIntInRange("Enter record number to delete: ", 1, list.Count);
            list.RemoveAt(num - 1);
            SaveToBinaryFile(binPath, list);
            Console.WriteLine("Record deleted and saved.");
        }

        public static void EditRecord(List<TEmployee> list, string binPath)
        {
            if (list.Count == 0) { Console.WriteLine("Empty list."); return; }
            ShowTable(list);
            int num = ReadIntInRange("Enter record number to edit: ", 1, list.Count);
            int index = num - 1;
            TEmployee emp = list[index];

            Console.WriteLine($"Current FIO: {emp.FIO}");
            string newFio = InputString("New FIO (Enter to keep): ");
            if (!string.IsNullOrWhiteSpace(newFio))
            {
                while (!IsValidName(newFio)) { newFio = InputString("Invalid name. Try again: "); }
                emp.FIO = newFio;
            }

            Console.WriteLine($"Current Phone: {emp.Phone}");
            string newPhone = InputString("New Phone (Enter to keep): ");
            if (!string.IsNullOrWhiteSpace(newPhone))
            {
                while (!IsValidPhone(newPhone)) { newPhone = InputString("Invalid phone. Try again: "); }
                emp.Phone = newPhone;
            }

            Console.WriteLine($"Current Department: {emp.Department}");
            string newDept = InputString("New Department (Enter to keep): ");
            if (!string.IsNullOrWhiteSpace(newDept))
            {
                while (!IsValidDepartment(newDept)) { newDept = InputString("Invalid department. Try again: "); }
                emp.Department = newDept;
            }

            Console.WriteLine($"Current Salary: {emp.Salary}");
            string newSalaryStr = InputString("New Salary (Enter to keep): ");
            if (!string.IsNullOrWhiteSpace(newSalaryStr))
            {
                newSalaryStr = newSalaryStr.Replace(',', '.');
                decimal newSalary;
                while (!decimal.TryParse(newSalaryStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out newSalary) || !IsValidSalary(newSalary))
                {
                    newSalaryStr = InputString("Invalid salary (must be >= min). Try again: ").Replace(',', '.');
                }
                emp.Salary = newSalary;
            }

            Console.WriteLine($"Current Hire Date: {emp.HireDate:dd.MM.yyyy}");
            string newDateStr = InputString("New Hire Date (Enter to keep): ");
            if (!string.IsNullOrWhiteSpace(newDateStr))
            {
                DateTime newDate;
                while (!DateTime.TryParse(newDateStr, out newDate) || !IsValidHireDate(newDate))
                {
                    newDateStr = InputString("Invalid date (can't be future). Try again: ");
                }
                emp.HireDate = newDate;
            }

            list[index] = emp;
            SaveToBinaryFile(binPath, list);
            Console.WriteLine("Record updated and saved.");
        }

        public static void SortRecords(List<TEmployee> list, string binPath)
        {
            Console.WriteLine("1 - Sort by Surname\n2 - Sort by Phone");
            int choice = ReadIntInRange("Choice: ", 1, 2);

            if (choice == 1) list.Sort((x, y) => string.Compare(x.FIO, y.FIO));
            else list.Sort((x, y) => string.Compare(x.Phone, y.Phone));

            SaveToBinaryFile(binPath, list);
            Console.WriteLine("Sorted and saved.");
        }

        //функция поиска
        public static void SearchByPrefix(List<TEmployee> list)
        {
            string input = InputString("Enter digit triples separated by commas (e.g. 361, 273): ");
            string[] prefixes = input.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int totalFound = 0;
            Console.WriteLine("\nSearch results:");
            foreach (var prefix in prefixes)
            {
                if (prefix.Length == 3)
                {
                    int count = 0;
                    Console.WriteLine($"\n-- Prefix {prefix} --");
                    foreach (var emp in list)
                    {
                        if (emp.Phone.Contains(prefix))
                        {
                            string surname = emp.FIO.Split(' ')[0];
                            //Добавляю отдел
                            Console.WriteLine($"{surname} | {emp.Department} | {emp.Phone}");
                            count++;
                        }
                    }
                    Console.WriteLine($"  Found: {count}");
                    totalFound += count;
                }
                else
                {
                    Console.WriteLine($"Skipped '{prefix}': must be 3 digits");
                }
            }
            Console.WriteLine($"\nTotal records found: {totalFound}");
        }

        public static void CorrectData(List<TEmployee> list, string binPath)
        {
            int corrected = 0;
            for (int i = 0; i < list.Count; i++)
            {
                TEmployee emp = list[i];
                bool changed = false;

                string cleanPhone = "";
                foreach (char c in emp.Phone)
                {
                    if (char.IsDigit(c) || c == '+') cleanPhone += c;
                }

                if (cleanPhone != emp.Phone) { emp.Phone = cleanPhone; changed = true; }
                if (!IsValidPhone(emp.Phone)) { emp.Phone = "+70000000000"; changed = true; }
                if (string.IsNullOrWhiteSpace(emp.FIO)) { emp.FIO = "Unknown Unknown Unknown"; changed = true; }

                if (changed)
                {
                    list[i] = emp;
                    corrected++;
                }
            }
            if (corrected > 0)
            {
                SaveToBinaryFile(binPath, list);
                Console.WriteLine($"Corrected records: {corrected}");
            }
            else Console.WriteLine("All data is already correct.");
        }

        public static decimal CalculateAverageSalary(List<TEmployee> list)
        {
            if (list.Count == 0) return 0;
            decimal total = 0;
            foreach (TEmployee emp in list)
            {
                total += emp.Salary;
            }
            return total / list.Count;
        }

        public static void FilterByDepartment(List<TEmployee> list)
        {
            string dept = InputString("Enter department to filter by: ");
            List<TEmployee> filtered = new List<TEmployee>();

            foreach (var emp in list)
            {
                if (emp.Department.Equals(dept, StringComparison.OrdinalIgnoreCase))
                {
                    filtered.Add(emp);
                }
            }

            if (filtered.Count == 0)
            {
                Console.WriteLine("No employees found in this department.");
            }
            else
            {
                filtered.Sort((x, y) =>
                {
                    int deptCompare = string.Compare(x.Department, y.Department);
                    if (deptCompare == 0) return string.Compare(x.FIO, y.FIO);
                    return deptCompare;
                });
                ShowTable(filtered);
            }
        }

        public static void FilterByHireDate(List<TEmployee> list)
        {
            string dateStr = InputString("Enter date to filter (hired before/on dd.mm.yyyy): ");
            if (!DateTime.TryParse(dateStr, out DateTime targetDate))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            List<TEmployee> filtered = new List<TEmployee>();
            foreach (var emp in list)
            {
                if (emp.HireDate <= targetDate)
                {
                    filtered.Add(emp);
                }
            }
            if (filtered.Count == 0)
            {
                Console.WriteLine("No employees hired before this date.");
            }
            else
            {
                ShowTable(filtered);
            }
        }
    }
}