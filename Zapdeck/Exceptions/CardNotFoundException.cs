namespace Zapdeck.Exceptions
{
    [Serializable]
    internal class CardNotFoundException : Exception
    {
        public CardNotFoundException() { }
        public CardNotFoundException(string cardName) : base($"Unable to find card \"{cardName}.\" Can you be more specific?") { }
        public CardNotFoundException(string cardName, Exception inner) : base($"Unable to find card \"{cardName}\". Can you be more specific?", inner) { }
    }
}
