using Storm.Execution;
using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Storm.Helpers
{
    internal static class SelectCommandHelper
    {
        const string TOKEN_CLASS = "a-zA-Z0-9_";

        public static IEnumerable<(String[], string)> ValidatePath(string RequestPath)
        {
            var _rq = RequestPath.Trim();
            if (Regex.IsMatch(_rq,$@"^(?'path'[{TOKEN_CLASS}.]+)\.(?'field'[{TOKEN_CLASS}]+)$"))
            {
                var m = Regex.Match(_rq, $@"^(?'path'[{TOKEN_CLASS}.]+)\.(?'field'[{TOKEN_CLASS}]+)$");
                var string_path = m.Groups["path"].Value;
                var field = m.Groups["field"].Value;
                return new List<(String[], String)> { (string_path.Split('.'), field) };
            }
            else if (Regex.IsMatch(_rq, $@"^(?'path'[{TOKEN_CLASS}.]+)\.(?'wildcard'\*)$"))
            {
                var m = Regex.Match(_rq, $@"^(?'path'[{TOKEN_CLASS}.]+)\.(?'wildcard'\*)$");
                var string_path = m.Groups["path"].Value;
                return new List<(String[], String)> { (string_path.Split('.'), "*") };
            }
            else if (Regex.IsMatch(_rq, $@"^(?'path'[{TOKEN_CLASS}.]+)\.\{{(?'braces'[{TOKEN_CLASS}, ]+)\}}$"))
            {
                var m = Regex.Match(_rq, $@"^(?'path'[{TOKEN_CLASS}.]+)\.\{{(?'braces'[{TOKEN_CLASS}, ]+)\}}$");
                var string_path = m.Groups["path"].Value;
                var brace_content = m.Groups["braces"].Value;
                var fields = Regex.Matches(brace_content, $"[{TOKEN_CLASS}]+")
                    .Cast<Match>()
                    .Select(x => x.Value);
                return fields.Select(x => (string_path.Split('.'), x));
            }
            else
            {
                throw new ArgumentException($"Invalid select path {RequestPath}");
            }
        }

        public static SelectNode GenerateSingleSelectNode((String[], string) validatedPath, FromTree fromTree)
        {
            var x = fromTree.Resolve(validatedPath.Item1);
            return x.Entity.entityFields
                .Where(ef => ef.CodeName == validatedPath.Item2)
                .Select(ef => {
                    return new SelectNode()
                    {
                        FullPath = new FieldPath(x.FullPath.Root, x.FullPath.Path, ef.CodeName),
                        EntityField = ef,
                        FromNode = x
                    };
                })
                .FirstOrDefault();
        }

    }
}
