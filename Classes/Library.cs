﻿using I2P_Project.DataBases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace I2P_Project.Classes
{
    /// <summary>
    /// Class for managing library.
    /// Here are methods that are acessed by the system mostly, not by any users.
    /// </summary>
    class Library
    {
        private LINQtoUserDBDataContext db;

        /// <summary> Initializing DB </summary>
        public Library()
        {
            db = new LINQtoUserDBDataContext();
            db.SubmitChanges(); // DB Preload
        }

        /// <summary> Registers new user in data base </summary>
        public bool RegisterUser(string login, string password, string name, string adress, string phone, bool isLibrarian)
        {
            if (CheckLogin(login)) return false;

            users newUser = new users();
            newUser.login = login;
            newUser.password = password;
            newUser.name = name;
            newUser.address = adress;
            newUser.phoneNumber = phone;
            newUser.userType = isLibrarian ? 2 : 0;
            db.users.InsertOnSubmit(newUser);
            db.SubmitChanges();
            return true;
        }

        #region Output for viewing in tables

        public ObservableCollection<Pages.MyBooksTable> GetUserBooks()
        {
            ObservableCollection<Pages.MyBooksTable> temp_table = new ObservableCollection<Pages.MyBooksTable>();
            var load_user_books = from c in db.checkouts
                                  join b in db.documents on c.bookID equals b.Id
                                  where c.userID == SDM.CurrentUser.PersonID && c.isReturned == false
                                  select new
                                  {
                                      c.checkID,
                                      c.bookID,
                                      b.Title,
                                      c.dateTaked,
                                      c.timeToBack
                                  };
            foreach (var element in load_user_books)
            {
                Pages.MyBooksTable row = new Pages.MyBooksTable
                {
                    checkID = element.checkID,
                    bookID = element.bookID,
                    b_title = element.Title,
                    c_dateTaked = (System.DateTime)element.dateTaked,
                    c_timeToBack = (System.DateTime)element.timeToBack
                };
                temp_table.Add(row);
            }
            return temp_table;
        }

        public ObservableCollection<Pages.DocsTable> TestDocsTableOnlyBooks()
        {
            ObservableCollection<Pages.DocsTable> temp_table = new ObservableCollection<Pages.DocsTable>();
            var load_user_docs = from b in db.documents
                                 select new
                                 {
                                     b.Id,
                                     b.Title,
                                     b.DocType,
                                     b.IsReference
                                 };
            foreach (var element in load_user_docs)
            {
                checkouts checkoutInfo = GetOwnerInfo(element.Id);
                Pages.DocsTable row = new Pages.DocsTable
                {
                    docID = element.Id,
                    docTitle = element.Title,
                    docType = TypeString(element.DocType),
                    docOwnerID = checkoutInfo == null ? -1 : checkoutInfo.userID,
                    dateTaked = checkoutInfo == null ? DateTime.Now : (System.DateTime)checkoutInfo.dateTaked,
                    timeToBack = checkoutInfo == null ? DateTime.Now : (System.DateTime)checkoutInfo.timeToBack,
                    isReference = element.IsReference
                };
                temp_table.Add(row);
            }
            return temp_table;
        }

        public ObservableCollection<Pages.DocsTable> TestDocsTableUsersBooks(int user_id)
        {
            ObservableCollection<Pages.DocsTable> temp_table = new ObservableCollection<Pages.DocsTable>();
            var load_user_docs = from c in db.checkouts
                                 join b in db.documents on c.bookID equals b.Id
                                 where c.userID == user_id
                                 select new
                                 {
                                     c.bookID,
                                     b.Title,
                                     b.DocType,
                                     c.dateTaked,
                                     c.timeToBack
                                 };
            foreach (var element in load_user_docs)
            {
                Pages.DocsTable row = new Pages.DocsTable
                {
                    docID = element.bookID,
                    docOwnerID = user_id,
                    docTitle = element.Title,
                    docType = TypeString(element.DocType),
                    dateTaked = (DateTime)element.dateTaked,
                    timeToBack = element.timeToBack
                };
                temp_table.Add(row);
            }
            return temp_table;
        }

        public ObservableCollection<Pages.UserTable> TestUsersTable()
        {
            ObservableCollection<Pages.UserTable> temp_table = new ObservableCollection<Pages.UserTable>();
            var load_users = from u in db.users
                             join ut in db.userTypes on u.userType equals ut.typeID
                             select new
                             {
                                 u.id,
                                 u.name,
                                 u.address,
                                 u.phoneNumber,
                                 ut.typeName
                             };
            foreach (var element in load_users)
            {
                Pages.UserTable row = new Pages.UserTable
                {
                    userID = element.id,
                    userName = element.name,
                    userAddress = element.address,
                    userPhoneNumber = element.phoneNumber,
                    userType = element.typeName
                };
                temp_table.Add(row);
            }
            return temp_table;
        }

        // TODO Replace with Observable collection
        public List<document> GetAllDocs()
        {
            var test = (from p in db.documents select p);
            return test.ToList();
        }

        private checkouts GetOwnerInfo(int docID)
        {
            var test = from c in db.checkouts
                       where c.bookID == docID
                       select c;
            if (test.Any()) return test.Single();
            else return null;
        }

        #endregion

        #region Existence checking

        /// <summary> Checks if there exist a user with given e-mail </summary>
        public bool CheckLogin(string login)
        {
            var test = (from p in db.users
                        where p.login == login
                        select p);
            return test.Any();
        }

        /// <summary> Checks if a user with given e-mail has given password </summary>
        public bool CheckPassword(string login, string password)
        {
            var test = (from p in db.users
                        where (p.login == login && p.password == password)
                        select p);
            return test.Any();
        }

        #endregion

        /// <summary> Returns numerator of user type </summary>
        /// <returns>
        /// 0 - Student
        /// 1 - Faculty
        /// 2 - Librarian
        /// </returns>
        public int GetUserType(string login)
        {
            var test = (from p in db.users
                        where (p.login == login)
                        select p);
            if (test.Any())
                return test.Single().userType;
            return -1;
        }

        /// <summary> Clears DB (for test cases only) </summary>
        public void ClearDB()
        {
            db.ExecuteCommand("DELETE FROM documents");
            db.ExecuteCommand("DELETE FROM users");
            db.ExecuteCommand("DELETE FROM checkouts");
        }

        private static string TypeString(int num)
        {
            switch (num)
            {
                case 0:
                    return "Book";
                case 1:
                    return "Journal";
                case 2:
                    return "Audio";
                case 3:
                    return "Video";
                default:
                    throw new Exception("Something went wrong");
            }
        }

    }
}
