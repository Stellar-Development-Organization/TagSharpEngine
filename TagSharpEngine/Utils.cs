using System.Text.RegularExpressions;

namespace TagSharpEngine {
    public partial class Utils {
        public static string? EscapeContent(string input) {
            if (input is null) {
                return null;
            }

            Regex checker = MyRegex();
            return checker.Replace(input, new MatchEvaluator(SubMatch));
        }

        private static string SubMatch(Match match) {
            return "\\" + match.Groups[1].Value;
        }

        [GeneratedRegex(@"(?<!\\)([{():|}])")]
        private static partial Regex MyRegex();

        public static string? ParseToOutput(string payload, bool? result) {
            if (result is null) return null;

            try {
                string[]? output = HelperSplitIf(payload, false);
                if (output is not null && output.Length == 2) {
                    if (result.Value) {
                        return output[0];
                    } else {
                        return output[1];
                    }
                } else if (result is not null) {
                    return payload;
                } else {
                    return string.Empty;
                }
            } catch {
                return null;
            }
        }

        private static bool? ImplicitBool(string booleanData) {
            Dictionary<string, bool> table = new (){ 
                { "True", true },
                { "False", false },
                { "true", true },
                { "false", false }
            };

            return table.TryGetValue(booleanData, out bool result);
        }

        public static bool? HelperParseIf(string ifStr) {
            bool? value = ImplicitBool(ifStr);
            if (value is not null) {
                return value;
            }

            try {
                if (ifStr.Contains("!=")) {
                    string[] spl = ifStr.Split("!=");
                    return spl[0].Trim() != spl[1].Trim();
                }

                if (ifStr.Contains("==")) {
                    string[] spl = ifStr.Split("==");
                    return spl[0].Trim() == spl[1].Trim();
                }

                if (ifStr.Contains(">=")) {
                    string[] spl = ifStr.Split(">=");
                    return float.Parse(spl[0].Trim()) >= float.Parse(spl[1].Trim());
                }

                if (ifStr.Contains("<=")) {
                    string[] spl = ifStr.Split("<=");
                    return float.Parse(spl[0].Trim()) <= float.Parse(spl[1].Trim());
                }

                if (ifStr.Contains('>')) {
                    string[] spl = ifStr.Split(">");
                    return float.Parse(spl[0].Trim()) > float.Parse(spl[1].Trim());
                }

                if (ifStr.Contains('<')) {
                    string[] spl = ifStr.Split("<");
                    return float.Parse(spl[0].Trim()) < float.Parse(spl[1].Trim());
                }

            } catch {}

            return null;
        }

        public static bool?[] HelperParseListIf(string ifStr) {
            string[]? spl = HelperSplitIf(ifStr);
            if (spl!.Length == 0) {
                return new bool?[] { HelperParseIf(ifStr) };
            }

            return spl.Select(item => HelperParseIf(item)).ToArray();
        }

        private static string[]? HelperSplitIf(string payload, bool easy = true) {
            if (payload.Contains('|')) {
                return payload.Split('|');
            }

            if (easy) {
                if (payload.Contains('~')) {
                    return payload.Split('~');
                }

                if (payload.Contains(',')) {
                    return payload.Split(',');
                }
            }

            return null;
        }

        [GeneratedRegex(@"(?<!\\)\|")]
        private static partial Regex SplitRegex();
    }
}
