namespace TagSharpEngine {
    public class Verb {
        public string? Declaration;
        public string? Payload;
        public string? Parameter;

        private int DecStart, DecDepth;
        private string ParsedString = string.Empty;

        public Verb(string? verbString = null, int limit = 2000, bool dotParam = false) {
            if (verbString is null) return;

            Parse(verbString, limit);
        }

        public void Parse(string verbString, int limit) {
            DecDepth = DecStart = 0;

            string parsing = verbString[1..^1];
            ParsedString = parsing[..Math.Min(limit, parsing.Length)];
            bool SkipNext = false;

            for (int i = 0; i < ParsedString.Length; i++) {
                char ch = ParsedString[i];
                if (SkipNext) {
                    SkipNext = false;
                    continue;
                } else if (ch == '\\') {
                    SkipNext = true;
                    continue;
                }

                if (ch == ':' && DecDepth == 0) {
                    SetPayload();
                    return;
                } else if (ParseParameter(i, ch)) {
                    return;
                }
            }

            SetPayload();
        }

        private void SetPayload() {
            string[] parsed = ParsedString.Split(':', 2);

            if (parsed.Length == 2) {
                Payload = parsed[1];
            }
            
            Declaration = parsed[0];
        }

        private bool ParseParameter(int index, char verbChar) {
            if (verbChar == '(') OpenParameter(index);
            
            else if (verbChar == ')' && DecDepth != 0) {
                return CloseParameter(index);
            }
            return false;
        }

        private void OpenParameter(int index) {
            DecDepth++;

            if (DecStart == 0) {
                DecStart = index;
                Declaration = ParsedString[..index];
            }
        }

        private bool CloseParameter(int index) {
            DecDepth--;

            if (DecDepth == 0){
                Parameter = ParsedString[(DecStart + 1)..index];
                try {
                    if (ParsedString[index + 1] == ':') {
                        Payload = ParsedString[(index + 2)..];
                    }
                } catch (IndexOutOfRangeException) {} //Silence this as it is not important.

                return true;
            }

            return false;
        }

        /// <summary>
        /// To support ToString() for readability.
        /// </summary>
        public override string ToString() {    
            string response = "{";

            if (Declaration is not null) { response += Declaration; }
            if (Parameter is not null) { response += $"({Parameter})"; }
            if (Payload is not null) { response += $":{Payload}"; }

            return response + "}";
        }
    }
}