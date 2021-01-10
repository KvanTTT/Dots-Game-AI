namespace DotsGame
{
    public interface IDotsGameFormatParser
    {
        GameInfo Parse(byte[] text);

        byte[] Serialize(GameInfo gameInfo);
    }
}
