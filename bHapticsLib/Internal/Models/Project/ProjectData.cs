using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib.Internal.Models.Project
{
    internal class ProjectData : JSONObject
    {
        internal JSONArray tracks
        {
            get
            {
                string key = nameof(tracks);
                if (this[key] == null)
                    this[key] = new JSONArray();
                return this[key].AsArray;
            }
            set => this[nameof(tracks)] = value;
        }

        internal JSONArray layout
        {
            get
            {
                string key = nameof(layout);
                if (this[key] == null)
                    this[key] = new JSONArray();
                return this[key].AsArray;
            }
            set => this[nameof(layout)] = value;
        }
    }
}