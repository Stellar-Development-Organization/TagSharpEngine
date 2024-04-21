namespace TagSharpEngine.Interface {
    /// <summary>
    /// The base interface for making a custom block in TagScript.
    /// </summary>
    public interface IBaseBlock { 
        public IEnumerable<string>? AcceptedNames { get; set; }
        
        public virtual bool WillAccept(Context ctx) {
            var dec = ctx.Verb.Declaration?.ToLower();
            if (dec is null || AcceptedNames is null) return false;

            return AcceptedNames.Contains(dec);
        }

        public virtual Task<string?> Process(Context ctx) {
            throw new NotImplementedException();
        }
    }
}
