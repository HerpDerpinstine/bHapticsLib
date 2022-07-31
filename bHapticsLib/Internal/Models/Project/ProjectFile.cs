using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib.Internal.Models.Project
{
    internal class ProjectFile : JSONObject
    {
        internal int intervalMillis
        {
            get => this[nameof(intervalMillis)];
            set => this[nameof(intervalMillis)] = value;
        }

        internal int durationMillis
        {
            get => this[nameof(durationMillis)];
            set => this[nameof(durationMillis)] = value;
        }

        internal int size
        {
            get => this[nameof(size)];
            set => this[nameof(size)] = value;
        }

        internal ProjectData project
        {
            get
            {
                string key = nameof(project);
                if (this[key] == null)
                    this[key] = new ProjectData();
                return this[key].AsObject as ProjectData;
            }
            set => this[nameof(project)] = value;
        }
    }
}