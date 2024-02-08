namespace SweetMeSoft.Base.Tools
{
    public class StringMatch
    {
        public StringMatch(string text, decimal match)
        {
            Text = text;
            Match = match;
        }

        public string Text { get; }
        public decimal Match { get; }
    }
}
