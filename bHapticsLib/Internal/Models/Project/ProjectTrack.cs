using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib.Internal.Models.Project
{
    internal class ProjectTrack : JSONObject
    {
        internal JSONArray effects
        {
            get
            {
                string key = nameof(effects);
                if (this[key] == null)
                    this[key] = new JSONArray();
                return this[key].AsArray;
            }
            set => this[nameof(effects)] = value;
        }
    }
}
