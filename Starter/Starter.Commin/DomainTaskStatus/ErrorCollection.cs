using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Starter.Common.DomainTaskStatus
{
    public sealed class ErrorCollection
    {
        [JsonProperty("errors")]
        private Dictionary<string, List<string>> _errors { get; set; } = new Dictionary<string, List<string>>();

        [JsonIgnore]
        public bool HasErrors => _errors.Any();

        public void AddError(string key, string error)
        {
            if (!_errors.TryGetValue(key, out List<string> errors))
            {
                errors = new List<string>();
                _errors[key] = errors;
            }
            errors.Add(error);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var key in _errors.Keys)
            {
                sb.Append($@" > ""{key}""").AppendLine();
                foreach (var error in _errors[key])
                {
                    sb.Append($@"   > ""{error}""").AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}