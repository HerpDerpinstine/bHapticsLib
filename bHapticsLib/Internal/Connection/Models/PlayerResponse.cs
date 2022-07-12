using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib.Internal.Connection.Models
{
    internal class PlayerResponse : JSONObject
    {
        internal int ConnectedDeviceCount
        {
            get => this[nameof(ConnectedDeviceCount)].AsInt;
        }

        internal JSONArray ActiveKeys
        {
            get
            {
                string key = nameof(ActiveKeys);
                if (this[key] == null)
                    this[key] = new JSONArray();
                return this[key].AsArray;
            }
        }

        internal JSONArray ConnectedPositions
        {
            get
            {
                string key = nameof(ConnectedPositions);
                if (this[key] == null)
                    this[key] = new JSONArray();
                return this[key].AsArray;
            }
        }

        internal JSONArray RegisteredKeys
        {
            get
            {
                string key = nameof(RegisteredKeys);
                if (this[key] == null)
                    this[key] = new JSONArray();
                return this[key].AsArray;
            }
        }

        internal JSONObject Status
        {
            get
            {
                string key = nameof(Status);
                if (this[key] == null)
                    this[key] = new JSONObject();
                return this[key].AsObject;
            }
        }
    }
}