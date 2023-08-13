namespace TagSharpEngine.Interface {
    /// <summary>
    /// The base interface for making a custom block in TagScript.
    /// </summary>
    public abstract class Block {
        public List<string>? AcceptedNames;

        public virtual bool WillAccept(Context ctx) {
            var dec = ctx.Verb.Declaration?.ToLower();
            if (dec is null || AcceptedNames is null) return false;

            return AcceptedNames.Contains(dec);
        }

        public virtual object? PreProcess(Context ctx) {
            return null;
        }

        public virtual string? Process(Context ctx) {
            throw new NotImplementedException();
        }

        public virtual object? PostProcess(Context ctx) {
            // No use of it yet.
            return null;
        }
    }
}
