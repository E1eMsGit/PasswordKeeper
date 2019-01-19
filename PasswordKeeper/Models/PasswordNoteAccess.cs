using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace PasswordKeeper.Models
{
    public class PasswordNoteAccess
    {
        static string md = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string catalog = md + "\\PasswordKeeper";

        private XDocument _xDoc;
        private XElement _passwords;

        public int Id { get; set; }

        public PasswordNoteAccess()
        {
            try
            {
                _xDoc = XDocument.Load(catalog + "\\passwords.xml");
                _passwords = _xDoc.Element("passwords");
            }
            catch (Exception)
            {
                _xDoc = new XDocument();
                _passwords = new XElement("passwords");
                _xDoc.Add(_passwords);
                _xDoc.Save(catalog + "\\passwords.xml");
            }
        }

        public IEnumerable<PasswordNoteModel> GetData()
        {
            Id = 0;

            var items = from xe in _passwords.Elements("note")
                        select new PasswordNoteModel
                        {
                            Id = Id++,
                            Title = xe.Element("title").Value,
                            Login = xe.Element("login").Value,
                            Password = xe.Element("password").Value
                        };

            return items.OrderBy(item => item.Title);
        }

        public void AddNote(PasswordNoteModel newNote)
        {
            XElement note = new XElement("note");
            XAttribute noteIdAttr = new XAttribute("Id", Id);
            XElement noteTitleElem = new XElement("title", newNote.Title);
            XElement noteLoginElem = new XElement("login", newNote.Login);
            XElement notePasswordElem = new XElement("password", newNote.Password);

            note.Add(noteIdAttr);
            note.Add(noteTitleElem);
            note.Add(noteLoginElem);
            note.Add(notePasswordElem);

            _passwords.Add(note);
            _xDoc.Save(catalog + "\\passwords.xml");

            Id++;
        }

        public void UpdateNote(PasswordNoteModel updateNote)
        {
            foreach (XElement xe in _passwords.Elements("note").ToList())
            {
                if (xe.Attribute("Id").Value == updateNote.Id.ToString())
                {
                    xe.Element("title").Value = updateNote.Title;
                    xe.Element("login").Value = updateNote.Login;
                    xe.Element("password").Value = updateNote.Password;
                }
            }

            _xDoc.Save(catalog + "\\passwords.xml");
        }

        public void DeleteNotes(List<PasswordNoteModel> deleteNotes)
        {
            foreach (var note in deleteNotes)
            {
                foreach (XElement xe in _passwords.Elements("note").ToList())
                {
                    if (xe.Attribute("Id").Value == note.Id.ToString())
                    {
                        xe.Remove();
                    }
                }
            }
   

            UpdateId();

            _xDoc.Save(catalog + "\\passwords.xml");
        }

        private void UpdateId()
        {
            int id = 0;

            foreach (XElement xe in _passwords.Elements("note").ToList())
            {
                xe.Attribute("Id").Value = id++.ToString();
            }
        }
    }
}
