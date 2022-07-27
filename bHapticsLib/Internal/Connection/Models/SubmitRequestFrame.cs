using System;
using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib.Internal.Connection.Models
{
    internal class SubmitRequestFrame : JSONObject
    {
        internal int durationMillis
        {
            get => this[nameof(durationMillis)].AsInt;
            set => this[nameof(durationMillis)] = value;
        }

        internal PositionID position
        {
            get => (PositionID)Enum.Parse(typeof(PositionID), this[nameof(position)]);
            set => this[nameof(position)] = value.ToString();
        }

        internal JSONArray dotPoints
        {
            get
            {
                string key = nameof(dotPoints);
                if (this[key] == null)
                    this[key] = new JSONArray();
                return this[key].AsArray;
            }
        }

        internal JSONArray pathPoints
        {
            get
            {
                string key = nameof(pathPoints);
                if (this[key] == null)
                    this[key] = new JSONArray();
                return this[key].AsArray;
            }
        }
    }
}
