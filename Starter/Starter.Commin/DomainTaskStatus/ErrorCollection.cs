using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Starter.Common.DomainTaskStatus
{
    public sealed class ErrorCollection
    {
        [JsonProperty("errors")]
        private Dictionary<string, string> _errors { get; set; } = new Dictionary<string, string>();

        [JsonIgnore]
        public bool HasErrors => _errors.Any();

        public void AddError(string key, string error)
        {
            if (!_errors.TryGetValue(key, out var existedError))
            {
                _errors[key] = error;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var key in _errors.Keys)
            {
                sb.Append($@" > ""{key}""").AppendLine();
                sb.Append($@"   > ""{_errors[key]}""").AppendLine();
            }

            return sb.ToString();
        }
    }
}