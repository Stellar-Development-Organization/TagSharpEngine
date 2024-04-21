namespace TagSharpEngine.Interface {
    /// <summary>
    /// An abstract base class for creating your own tag block.
    /// </summary>
    public interface IAdapter {
        public string? GetValue(Verb ctx) => throw new NotImplementedException();
    }
}
