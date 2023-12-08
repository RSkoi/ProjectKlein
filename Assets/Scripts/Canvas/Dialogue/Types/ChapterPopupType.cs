using System;

[Serializable]
public class ChapterPopupType
{
    public string text;
    public ChapterPopupPosEnum pos;
    public AudioEffectDataType popupClip;

    public int ChapterPopupPosEnumToYOffset()
    {
        return pos switch
        {
            ChapterPopupPosEnum.Top => 150,
            ChapterPopupPosEnum.Bottom => -150,
            _ => 0,
        };
    }
}
