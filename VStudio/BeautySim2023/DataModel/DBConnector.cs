using LiteDB;
using BeautySim2023.DataModel;
using System.Collections.Generic;

namespace BeautySim2023
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Phones { get; set; }
        public bool IsActive { get; set; }
    }

    public class DBConnector
    {
        public const string ID = "DB";
        public const string LOGFILE = "DataBase.txt";
        private string connection_string;
        private LiteDatabase db;
        private static volatile DBConnector instance;
        private static object _sync_object = new object();

        private DBConnector()
        {
        }

        //proprietà istanza singleton
        public static DBConnector Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBConnector();
                }
                return instance;
            }
        }

        public void InitDB(string dbName)
        {
            connection_string = dbName;
            db = new LiteDatabase(connection_string);
            LiteCollection<Users> usersCollection = db.GetCollection<Users>("Users");

            if (!db.CollectionExists("Users"))
            {
                CreateAccurateBeautySimUser(usersCollection);
                CreateAccurateAdminUser(usersCollection);
            }
            else if (!CheckAdminUserExists(usersCollection))
            {
                CreateAccurateAdminUser(usersCollection);
            }

            if (!db.CollectionExists("Events"))
            {
                var col = db.GetCollection<Events>("Events");
                col.EnsureIndex(x => x.Id);
            }
            if (!db.CollectionExists("Results"))
            {
                var col = db.GetCollection<Results>("Results");
                col.EnsureIndex(x => x.Id);
            }
            if (!db.CollectionExists("Cases"))
            {
                var col = db.GetCollection<Cases>("Cases");
                col.EnsureIndex(x => x.Id);
            }
        }

        private void CreateAccurateBeautySimUser(LiteCollection<Users> usersCollection)
        {
            Users user = new Users();
            user.IdParentUser = null;
            user.Name = "Accurate";
            user.Surname = "BeautySim";
            user.Organization = "Accurate";
            user.Password = "password";
            user.Role = 2;
            user.Title = "Company";
            user.UserName = AppControl.BeautySim_USERNAME;
            usersCollection.Insert(user);
            usersCollection.EnsureIndex(x => x.Id);
        }

        private void CreateAccurateAdminUser(LiteCollection<Users> usersCollection)
        {
            Users user = new Users();
            user.IdParentUser = null;
            user.Name = "Accurate";
            user.Surname = "Admin";
            user.Organization = "Accurate";
            user.Password = "4ccur4t3$42";
            user.Role = 2;
            user.Title = "Company";
            user.UserName = AppControl.ADMIN_USERNAME;
            usersCollection.Insert(user);
            usersCollection.EnsureIndex(x => x.Id);
        }

        private bool CheckAdminUserExists(LiteCollection<Users> usersCollection)
        {
            return usersCollection.FindOne("$.UserName = 'Accurate.Admin'") != null;
        }

        public void CreateTable<T>(Dictionary<string, bool> indexedItems = null)
        {
            using (var db = new LiteDatabase(connection_string))
            {
                var collection = db.GetCollection<T>(typeof(T).Name);

                // create index
                if (indexedItems != null)
                {
                    foreach (var item in indexedItems)
                    {
                        collection.EnsureIndex(item.Key, item.Value);
                    }
                }
            }
        }

        public void DeleteTable<T>()
        {
            using (var db = new LiteDatabase(connection_string))
            {
                db.DropCollection(typeof(T).Name);
            }
        }

        //public BsonValue InsertUser(Users item)
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Users>();

        //        if (item != null)
        //        {
        //            return (collection.Insert(item));
        //        }
        //    }
        //    return null;
        //}

        public BsonValue InsertRow<T>(T item)
        {
            //using (var db = new LiteDatabase(connection_string))
            //{
            var collection = db.GetCollection<T>(typeof(T).Name);

            if (item != null)
            {
                return (collection.Insert(item));
            }
            else
            {
                return null;
            }
            //}
        }

        //public BsonValue InsertEvent(Events item)
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Events>("Events");

        //        if (item != null)
        //        {
        //            return (collection.Insert(item));
        //        }
        //    }
        //    return null;
        //}

        //public BsonValue InsertResult(Results item)
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Results>("Results");

        //        if (item != null)
        //        {
        //            return (collection.Insert(item));
        //        }
        //    }
        //    return null;
        //}

        //public BsonValue InsertCase(Cases item)
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Cases>("Cases");

        //        if (item != null)
        //        {
        //            return (collection.Insert(item));
        //        }
        //    }
        //    return null;
        //}

        public void UpdateRow<T>(T item)
        {
            //using (var db = new LiteDatabase(connection_string))
            //{
            var collection = db.GetCollection<T>(typeof(T).Name);

            if (item != null)
            {
                collection.Update(item);
            }
            //}
        }

        //public void UpdateCase(Cases item)
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Cases>("Cases");

        //        if (item != null)
        //        {
        //            collection.Update(item);
        //        }
        //    }
        //}

        //public void UpdateUser(Users item)
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Users>("Users");

        //        if (item != null)
        //        {
        //            collection.Update(item);
        //        }
        //    }
        //}

        //public void UpdateResults(Results item)
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Results>("Results");

        //        if (item != null)
        //        {
        //            collection.Update(item);
        //        }
        //    }
        //}

        public void DeleteRow<T>(BsonValue objectId)
        {
            //using (var db = new LiteDatabase(connection_string))
            //{
            var collection = db.GetCollection<T>(typeof(T).Name);
            if (objectId != null)
            {
                collection.Delete(objectId);
            }
            //}
        }

        //public void DeleteEvent(BsonValue objectId)
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Events>("Events");
        //        if (objectId != null)
        //        {
        //            collection.Delete(objectId);
        //        }
        //    }
        //}

        //public void DeleteResult(BsonValue objectId)
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Results>("Results");
        //        if (objectId != null)
        //        {
        //            collection.Delete(objectId);
        //        }
        //    }
        //}

        //public void DeleteUser(BsonValue objectId)
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Users>("Users");
        //        if (objectId != null)
        //        {
        //            collection.Delete(objectId);
        //        }
        //    }
        //}

        public T FindRowById<T>(BsonValue id)
        {
            //using (var db = new LiteDatabase(connection_string))
            //{
            var collection = db.GetCollection<T>(typeof(T).Name);
            return collection.FindById(id);
            //}
        }

        //public Cases FindCaseById(ObjectId id)
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Cases>("Cases");
        //        return collection.FindById(id);
        //    }
        //}

        //public Results FindResultById(ObjectId id)
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Results>("Results");
        //        return collection.FindById(id);
        //    }
        //}

        //public Users FindUserById(ObjectId id)
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Users>("Users");
        //        return collection.FindById(id);
        //    }
        //}

        public IEnumerable<T> FindAll<T>()
        {
            //using (var db = new LiteDatabase(connection_string))
            //{
            var collection = db.GetCollection<T>(typeof(T).Name);
            IEnumerable<T> toRet = collection.FindAll();
            return toRet;
            // }
        }

        //public IEnumerable<Users> FindAllUsers()
        //{
        //    var collection = db.GetCollection<Users>("Users");
        //    return collection.FindAll();
        //}

        //public IEnumerable<Results> FindAllResults()
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Results>("Results");
        //        IEnumerable<Results> toRet = collection.FindAll();
        //        return toRet;
        //    }
        //}

        //public IEnumerable<Results> FindResultsByAttribute(string attributeName, string attributeValue)
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Results>("Results");
        //        return collection.Find(Query.EQ(attributeName, attributeValue)).ToList();
        //    }
        //}

        //public IEnumerable<Users> FindUsersByAttribute(string attributeName, string attributeValue)
        //{
        //    using (var db = new LiteDatabase(connection_string))
        //    {
        //        var collection = db.GetCollection<Users>("Users");
        //        return collection.Find(Query.EQ(attributeName, attributeValue)).ToList();
        //    }
        //}
    }
}