using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Newtonsoft.Json;

namespace ICENoticeBot.Core
{
    public class UserManager
    {
        private static HashSet<int> subscribedUsers;

        private static UserManager _instance;

        public static UserManager Instance()
        {// make new instance if null
            return _instance ?? (_instance = new UserManager());
        }

        protected UserManager()
        {
            
            if (File.Exists(Properties.Constants.SUB_USERS_PATH))
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader sr = new StreamReader(Properties.Constants.SUB_USERS_PATH))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    subscribedUsers = serializer.Deserialize<HashSet<int>>(reader);
                    Console.WriteLine($"Loaded notice db from '{Properties.Constants.SUB_USERS_PATH}'");
                }
            }
            else
            {
                subscribedUsers = new HashSet<int>();
                Console.WriteLine($"Creating '{Properties.Constants.SUB_USERS_PATH}'");
            }
        }

        /// <summary>
        /// Returns readonly collection
        /// </summary>
        /// <returns></returns>
        public ImmutableHashSet<int> Get()
        {
            return ImmutableHashSet.ToImmutableHashSet(subscribedUsers);
        }

        public bool Exists(int userID)
        {
            return subscribedUsers.Contains(userID);
        }

        /// <summary>
        /// Returns false if userID already exists
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Add(int userID)
        {
            if(subscribedUsers.Add(userID))
            {
                SaveUsersDB();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns false if there's no such user already
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Remove(int userID)
        {
            if(subscribedUsers.Remove(userID))
            {
                SaveUsersDB();
                return true;
            }
            return false;
        }

        private void SaveUsersDB()
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(Properties.Constants.SUB_USERS_PATH))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, subscribedUsers);
                Console.WriteLine($"Updated '{Properties.Constants.SUB_USERS_PATH}'");
            }
        }
    }
}
