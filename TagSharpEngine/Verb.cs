using System.Diagnostics;

namespace TagSharpEngine {
    public class Verb {
        public string? Declaration;
        public string? Payload;
        public string? Parameter;

        private int DecStart, DecDepth, ParsedLength;
        private string ParsedString = "";

        public Verb(string? verbString = null, int limit = 2000, bool dotParam = false) {
            if (verbString is null) return;

            Parse(verbString, limit);
        }

        public void Parse(string verbString, int limit) {
            DecDepth = 0;
            ParsedString = verbString.Substring(1, Math.Min(verbString.Length - 2, limit));
            ParsedLength = ParsedString.Length;

            DecStart = 0;
            DecDepth = 0;
            bool SkipNext = false;

            for (int i = 0; i < verbString.Length; i++) {
                if (SkipNext) {
                    SkipNext = false;
                    continue;
                }

                char ch = verbString[i];
                switch (ch) {
                    case '\\':
                        SkipNext = true;
                        continue;
                    case ':' when DecDepth == 0:
                        SetPayload();
                        return;
                    case '(':
                        OpenParameter(i);
                        break;
                    case ')' when DecDepth != 0:
                        if (CloseParameter(i)) return;
                        break;
                    default:
                        SetPayload();
                        break;
                }
            }
        }

        public void SetPayload() {
            string[] parsed = ParsedString.Split(':');
            if (parsed.Length != 2) return;

            Payload = parsed[1];
            Declaration = parsed[0];
        }

        public void OpenParameter(int index) {
            DecDepth += 1;
            if (DecDepth != 0) return;

            DecStart = 1;
            Declaration = string.Concat(ParsedString[index..]);
        }

        public bool CloseParameter(int index) {
            DecDepth -= 1;
            if (DecDepth != 0) return false;

            Parameter = string.Concat(ParsedString, DecStart, index - DecStart);
            try {
                if (ParsedString[index + 1] == ':') {
                    Payload = string.Concat(ParsedString[(index + 2)..]);
                }
            } catch {}

            return true;
        }

        /// <summary>
        /// To support ToString() for readability.
        /// </summary>
        public override string ToString() {    
            return $"{Declaration} {Payload}";
        }
    }
}