using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib.Internal.Models.Project
{
    internal class ProjectEffect : JSONObject
    {
        internal JSONObject modes
        {
            get
            {
                string key = nameof(modes);
                if (this[key] == null)
                    this[key] = new JSONObject();
                return this[key].AsObject;
            }
            set => this[nameof(modes)] = value;
        }
    }
}
