using TagSharpEngine.Interface;

namespace TagSharpEngine.Adapters {
    public class StringAdapter : IAdapter {
        public string InputString { get; set; }
        public bool Escape { get; set; }

        public StringAdapter(string input, bool escape = false) {
            InputString = input;
            Escape = escape;
        }

        public string? GetValue(Verb ctx) {
            return EscapeReturnHandler(VerbCtxHandler(ctx)!);
        }

        private protected string EscapeReturnHandler(string input) {
            return Escape ? Utils.EscapeContent(input)! : input;
        }

        private protected string? VerbCtxHandler(Verb ctx) {
            if (ctx.Parameter is null) {
                return InputString;
            }

            try {
                if (!ctx.Parameter.Contains('+')) {
                    int idx = ctx.Parameter.Length - 1;
                    string splter = ctx.Payload is null ? " " : ctx.Payload;
                    
                    return InputString.Split(splter)[idx];
                } else {
                    int idx = ctx.Parameter.Replace("+", "").Length - 1;
                    string splter = ctx.Payload is null ? " " : ctx.Payload;

                    if (ctx.Parameter.StartsWith('+')) {
                        return string.Join(splter, InputString.Take(idx + 1));
                    } else if (ctx.Parameter.EndsWith('+')) {
                        return string.Join(splter, InputString.Skip(idx));
                    } else {
                        return InputString.Split(splter)[idx];
                    }
                }
            } catch {
                return InputString;
            }
        }

    }
}