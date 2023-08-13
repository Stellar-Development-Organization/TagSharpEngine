namespace TagSharpEngine.Interface {
    /// <summary>
    /// An abstract base class for creating your own tag block.
    /// </summary>
    public abstract class Adapter {
        public virtual string? GetValue(Context ctx) {
            throw new NotImplementedException();
        }
    }
}
