using System;
using System.IO;
using System.Xml.Linq;

namespace PasswordKeeper.Models
{
    class LoginPasswordAccess
    {
        static string md = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string catalog = md + "\\PasswordKeeper";

        private XDocument _xDoc;
        public string LoginPassword { get; set; }

        public LoginPasswordAccess()
        {
            if (Directory.Exists(catalog) == false)
            {
                Directory.CreateDirectory(catalog);
            }

            try
            {
                _xDoc = XDocument.Load(catalog + "\\login_password.xml");       
                var root = _xDoc.Element("login_password");
                LoginPassword = root.Element("password").Value;
            }
            catch (Exception)
            {
                _xDoc = new XDocument();
                GenerateDefaultPassword();

                var root = _xDoc.Element("login_password");
                LoginPassword = root.Element("password").Value;
            }
        }
    
        private void GenerateDefaultPassword()
        {
            XElement note = new XElement("login_password");
            XElement notePasswordElem = new XElement("password", "12345");

            note.Add(notePasswordElem);

            _xDoc.Add(note);
            _xDoc.Save(catalog + "\\login_password.xml");
        }
    }
}
