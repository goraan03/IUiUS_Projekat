using IUiUS_Projekat.Models;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace IUiUS_Projekat.Services
{
    public static class UserFileGenerator
    {
        public static void GenerateUsersFile()
        {
            var users = new List<User>
            {
                new User("admin", "admin123", "Admin"),
                new User("visitor", "visitor123", "Visitor")
            };

            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
            using (FileStream fs = new FileStream("Users.xml", FileMode.Create))
            {
                serializer.Serialize(fs, users);
            }
        }
    }
}
