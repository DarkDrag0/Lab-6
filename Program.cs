using System;
using System.Collections.Generic;

namespace LR6
{
    internal struct Program
    {
        static void Main(string[] args)
        {
            string textFilePath = "employees.txt";
            string binFilePath = "employees.bin";

            Console.WriteLine("Initializing database...");
            List<TEmployee> employees = EmployeeManager.LoadFromTextFile(textFilePath);

            EmployeeManager.SaveToBinaryFile(binFilePath, employees);
            employees = EmployeeManager.LoadFromBinaryFile(binFilePath);

            while (true)
            {
                Console.WriteLine("\n===============EMPLOYEE LIST===============");
                Console.WriteLine("1. View all records");
                Console.WriteLine("2. Add record");
                Console.WriteLine("3. Delete record");
                Console.WriteLine("4. Edit record");
                Console.WriteLine("5. Search by phone prefix");
                Console.WriteLine("6. Sort");
                Console.WriteLine("7. Correct data");
                Console.WriteLine("8. Filter by Department");
                Console.WriteLine("9. Filter by Hire Date");
                Console.WriteLine("0. Exit");

                decimal avgSalary = EmployeeManager.CalculateAverageSalary(employees);
                Console.WriteLine($"\nAverage Salary: {avgSalary:F2}");

                int choice = EmployeeManager.ReadIntInRange("Your choice: ", 0, 9);

                switch (choice)
                {
                    case 1: EmployeeManager.ShowTable(employees); break;
                    case 2: EmployeeManager.AddRecord(employees, binFilePath); break;
                    case 3: EmployeeManager.DeleteRecord(employees, binFilePath); break;
                    case 4: EmployeeManager.EditRecord(employees, binFilePath); break;
                    case 5: EmployeeManager.SearchByPrefix(employees); break;
                    case 6: EmployeeManager.SortRecords(employees, binFilePath); break;
                    case 7: EmployeeManager.CorrectData(employees, binFilePath); break;
                    case 8: EmployeeManager.FilterByDepartment(employees); break;
                    case 9: EmployeeManager.FilterByHireDate(employees); break;
                    case 0:
                        Console.WriteLine("Exiting program");
                        return;
                }
            }
        }
    }
}