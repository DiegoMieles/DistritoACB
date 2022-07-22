using System.Collections.Generic;
using System.Security;

namespace WebAPI
{
    public class DataSnapshot
    {
        private object val_obj;
        private Dictionary<string, object> val_dict;
        private List<string> keys;
        private string json;
        private string message;

        protected DataSnapshot()
        {
            val_dict = null;
            val_obj = null;
            keys = null;
            json = null;
        }
        
        public DataSnapshot(string _json = "", string _message = "")
        {
            object obj = (_json != null && _json.Length > 0)?Json.Deserialize(_json):null;

            if (obj is Dictionary<string, object>)
                val_dict = obj as Dictionary<string, object>;
            else
                val_obj = obj;

            keys = null;
            json = _json;
            message = _message;
        }


        public List<string> Keys
        {
            get
            {
                if (keys == null && val_dict != null)
                    keys = new List<string>(val_dict.Keys);

                return keys;
            }
        }
        
        public string FirstKey
        {
            get
            {
                return (val_dict == null) ? null : Keys[0];
            }
        }
        
        public string Message
        {
            get
            {
                return message;
            }
        }

        public string MessageCustom
        {
            get
            {
                return (val_dict == null) ? string.Empty : val_dict.ContainsKey("message") ? val_dict["message"].ToString() : string.Empty ;
            }
        }

        public string RawJson
        {
            get
            {
                return json;
            }
        }

        public int Code
        {
            get
            {
                return (val_dict == null) ? 0 : val_dict.ContainsKey("code") ? int.Parse(val_dict["code"].ToString()) : 0;
            }
        }

        public object RawValue
        {
            get
            {
                return (val_dict == null) ? val_obj : val_dict;
            }
        }

        [SecuritySafeCritical]
        public T Value<T>()
        {
            try
            {
                if (val_obj != null)
                    return (T)val_obj;
                object obj = val_dict;
                return (T)obj;
            }
            catch
            {
                return default(T);
            }
        }
    }
}

