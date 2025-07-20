using IUiUS_Projekat.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IUiUS_Projekat.Services
{
    public static class UserService
    {
        private static readonly string filePath = "Users.xml";

        public static List<User> LoadUsers()
        {
            if (!File.Exists(filePath)) return new List<User>();

            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                return (List<User>)serializer.Deserialize(fs);
            }
        }
    }
}
