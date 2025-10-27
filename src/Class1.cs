using System;
using System.Collections.Generic;

namespace Toon
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }

    public class UserDataSet
    {
        public IEnumerable<User> Users { get; set; }
    }

    public static class Program
    {
        public static void Main()
        {
            var data = new[]
            {
                new User { Id = 1, Name = "Alice", Role = "admin" },
                new User { Id = 2, Name = "Bob", Role = "user" }
            };

            var userDataSet = new UserDataSet
            {
                Users = data
            };

            var serializer = new ToonSerializer();

            var results = serializer.Serialize(userDataSet);

            Console.WriteLine(results);
            Console.ReadLine();
        }
    }
}
