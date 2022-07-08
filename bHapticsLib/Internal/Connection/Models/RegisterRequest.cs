using bHapticsLib.SimpleJSON;

namespace bHapticsLib.Internal.Connection.Models
{
    internal class RegisterRequest : JSONObject
    {
        internal string key
        {
            get => this[nameof(key)];
            set => this[nameof(key)] = value;
        }
    }
}
