using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib.Internal.Models.Connection
{
    internal class SubmitRequest : JSONObject
    {
        internal string type
        {
            get => this[nameof(type)];
            set => this[nameof(type)] = value;
        }

        internal string key
        {
            get => this[nameof(key)];
            set => this[nameof(key)] = value;
        }

        internal JSONObject Parameters
        {
            get
            {
                string key = nameof(Parameters);
                if (this[key] == null)
                    this[key] = new JSONObject();
                return this[key].AsObject;
            }
        }

        internal SubmitRequestFrame Frame
        {
            get
            {
                string key = nameof(Frame);
                if (this[key] == null)
                    this[key] = new SubmitRequestFrame();
                return this[key].AsObject as SubmitRequestFrame;
            }
        }
    }
}
