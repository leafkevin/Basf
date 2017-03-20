using Basf.Logging;
using MongoDB.Driver;
using System;

namespace Basf.LogMongo
{
    public class MongoLogger : ILogger
    {
        private IMongoCollection<Logger> collection = null;
        public MongoLogger()
        {
            MongoClient client = new MongoClient(Utility.GetAppSettingValue("MongoStore",""));
            IMongoDatabase db = client.GetDatabase("LoggerStore");
            this.collection = db.GetCollection<Logger>("Logger");
        }
        public bool IsDebugEnabled { get { return true; } }
        public bool IsErrorEnabled { get { return true; } }
        public bool IsInfoEnabled { get { return true; } }
        public void Debug(object objMessage)
        {
            this.collection.InsertOneAsync(new Logger("Debug", objMessage.ToString()));
        }
        public void Debug(object objMessage, Exception objException)
        {
            this.collection.InsertOneAsync(new Logger("Debug", objMessage, objException));
        }
        public void DebugFormat(string strFormat, params object[] objArgs)
        {
            this.collection.InsertOneAsync(new Logger("Debug", strFormat, objArgs));
        }
        public void Error(object objMessage)
        {
            this.collection.InsertOneAsync(new Logger("Error", objMessage.ToString()));
        }
        public void Error(object objMessage, Exception objException)
        {
            this.collection.InsertOneAsync(new Logger("Error", objMessage, objException));
        }
        public void ErrorFormat(string strFormat, params object[] objArgs)
        {
            this.collection.InsertOneAsync(new Logger("Error", strFormat, objArgs));
        }
        public void Fatal(object objMessage)
        {
            this.collection.InsertOneAsync(new Logger("Fatal", objMessage.ToString()));
        }
        public void Fatal(object objMessage, Exception objException)
        {
            this.collection.InsertOneAsync(new Logger("Fatal", objMessage, objException));
        }
        public void FatalFormat(string strFormat, params object[] objArgs)
        {
            this.collection.InsertOneAsync(new Logger("Fatal", strFormat, objArgs));
        }
        public void Info(object objMessage)
        {
            this.collection.InsertOneAsync(new Logger("Info", objMessage.ToString()));
        }
        public void Info(object objMessage, Exception objException)
        {
            this.collection.InsertOneAsync(new Logger("Info", objMessage, objException));
        }
        public void InfoFormat(string strFormat, params object[] objArgs)
        {
            this.collection.InsertOneAsync(new Logger("Info", strFormat, objArgs));
        }
        public void Warn(object objMessage)
        {
            this.collection.InsertOneAsync(new Logger("Warn", objMessage.ToString()));
        }
        public void Warn(object objMessage, Exception objException)
        {
            this.collection.InsertOneAsync(new Logger("Warn", objMessage, objException));
        }
        public void WarnFormat(string strFormat, params object[] objArgs)
        {
            this.collection.InsertOneAsync(new Logger("Warn", strFormat, objArgs));
        }
        class Logger
        {
            public DateTime CreateAt { get; } = DateTime.Now;
            public string Level { get; set; }
            public string Content { get; set; }
            public Logger(string level, object objMessage, Exception objException)
            {
                this.Level = level;
                this.Content = String.Format("Error:{0},Exception:{1}", objMessage, objException);
            }
            public Logger(string level, string strFormat, params object[] objArgs)
            {
                this.Level = level;
                this.Content = String.Format(strFormat, objArgs);
            }
        }
    }
}
