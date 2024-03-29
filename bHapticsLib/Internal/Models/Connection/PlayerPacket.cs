﻿using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib.Internal.Models.Connection
{
    internal class PlayerPacket : JSONObject
    {
        internal JSONArray Register
        {
            get
            {
                string key = nameof(Register);
                if (this[key] == null)
                    this[key] = new JSONArray();
                return this[key].AsArray;
            }
        }

        internal JSONArray Submit
        {
            get
            {
                string key = nameof(Submit);
                if (this[key] == null)
                    this[key] = new JSONArray();
                return this[key].AsArray;
            }
        }

        internal void Clear()
        {
            Register.Clear();
            Submit.Clear();
        }

        internal bool IsEmpty()
            => ((Register.Count <= 0) && (Submit.Count <= 0));
    }
}
