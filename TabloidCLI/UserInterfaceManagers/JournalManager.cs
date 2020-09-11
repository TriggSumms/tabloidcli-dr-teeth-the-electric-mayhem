﻿using System;
using System.Collections.Generic;
using TabloidCLI.Models;

namespace TabloidCLI.UserInterfaceManagers
{
    public class JournalManager : IUserInterfaceManager
    {
        private readonly IUserInterfaceManager _parentUI;
        private JournalRepository _journalRepository;
        private string _connectionString;

        public JournalManager(IUserInterfaceManager parentUI, string connectionString)
        {
            _parentUI = parentUI;
            _journalRepository = new JournalRepository(connectionString);
            _connectionString = connectionString;
        }

        public IUserInterfaceManager Execute()
        {
            Console.WriteLine("Journal Menu");
            Console.WriteLine(" 1) List Journal Entries");
            Console.WriteLine(" 2) Add Entry");
            Console.WriteLine(" 3) Edit Entry");
            Console.WriteLine(" 4) Remove Entry");
            Console.WriteLine(" 0) Go Back to Main Menu");

            Console.Write("> ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    List();
                    return this;
                case "2":
                    Add();
                    return this;
                case "3":
                    Edit();
                    return this;
                case "4":
                    Remove();
                    return this;
                case "0":
                    return _parentUI;
                default:
                    Console.WriteLine("Invalid Selection");
                    return this;
            }
        }

        private void List()
        {
            List<Journal> journals = _journalRepository.GetAll();
            foreach (Journal journal in journals)
            {
                Console.WriteLine($@"
        Title: {journal.Title}
        Entry: {journal.Content}
                                        
        Created: {journal.CreateDateTime}
        ------------------------------------");
            }
        }

        private Journal Choose(string prompt = null)
        {
            if(prompt == null)
            {
                prompt = "Please choose an Entry:";
            }
            Console.WriteLine(prompt);

            List<Journal> journals = _journalRepository.GetAll();

            for (int i = 0; i < journals.Count; i++)
            {
                Journal journal = journals[i];
                Console.WriteLine($"{i + 1}) {journal.Title}");
            }
            Console.Write("> ");

            string input = Console.ReadLine();
            try
            {
                int choice = int.Parse(input);
                return journals[choice - 1];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid Selection");
                return null;
            }
        }

        private void Add()
        {
            Console.WriteLine("New Journal Entry");
            Journal journal = new Journal();

            Console.WriteLine("Please enter in the Title of the new entry");
            string title = Console.ReadLine();
            if(!string.IsNullOrEmpty(title))
                {
                    journal.Title = title;
                } 
            else
                {
                    Console.WriteLine("You must enter in a Title");
                    return;
                }

            Console.WriteLine("Please enter the journal entry");
            string content = Console.ReadLine();
            if(!string.IsNullOrEmpty(content))
                {
                    journal.Content = content;
                }
            else
                {
                    Console.WriteLine("You must enter in content for your entry");
                    return;
                }

            journal.CreateDateTime = DateTime.Now;

            _journalRepository.Insert(journal);
        }

        private void Edit()
        {
            Journal journalToEdit = Choose("which journal entry would you like to edit?");
            if(journalToEdit == null)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine("New title (blank to leave unchanged: ");
                string title = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(title))
                {
                    journalToEdit.Title = title;
                }
                Console.WriteLine("New content(blank to leave unchanged: ");
                string content = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    journalToEdit.Content = content;
                }
                journalToEdit.CreateDateTime = journalToEdit.CreateDateTime;

                _journalRepository.Update(journalToEdit);
        }

        private void Remove()
        {
            Journal journalToDelete = Choose("Which entry would you like to remove?");
            if(journalToDelete != null)
            {
                _journalRepository.Delete(journalToDelete.Id);
            }
        }
    }
}
