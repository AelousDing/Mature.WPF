namespace Mature.Socket
{
    public class StringPackageInfo : IkeyedPackageInfo<string>
    {
        public string Key { get; set; }
        public string Body { get; set; }
        public bool IsCompressed { get; set; }
        public string MessageId { get; set; }
        public string[] Parameters { get; set; }
    }
}
