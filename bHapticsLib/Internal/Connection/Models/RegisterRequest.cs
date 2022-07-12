using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib.Internal.Connection.Models
{
    internal class RegisterRequest : JSONObject
    {
        internal string key
        {
            get => this[nameof(key)];
            set => this[nameof(key)] = value;
        }

        internal JSONObject project
        {
            get
            {
                string key = nameof(project);
                if (this[key] == null)
                    this[key] = new JSONObject();
                return this[key].AsObject;
            }
            set => this[nameof(project)] = value;
        }
    }
}
