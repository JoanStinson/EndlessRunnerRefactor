public struct HighscoreEntry : System.IComparable<HighscoreEntry>
{
    public string name;
    public int score;

    public int CompareTo(HighscoreEntry other)
    {
        // We want to sort from highest to lowest, so inverse the comparison.
        return other.score.CompareTo(score);
    }
}