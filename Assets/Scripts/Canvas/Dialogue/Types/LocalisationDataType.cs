using System;
using UnityEngine;

[Serializable]
public class LocalisationDataType
{
    [TextArea]
    public string text;
    public string name;
    public LocalisationNamePosEnum namePos;
    public bool isNarrator;
    public float sizeIncrease;
    public bool hasChoices = false;
    // this is hidden/visible depending on hasChoices, see LocalisationDataTypeDrawerUIE
    public ChoiceDataType[] choices;
    public bool hasChapterPopup = false;
    // this is hidden/visible depending on hasChapterPopup, see LocalisationDataTypeDrawerUIE
    public ChapterPopupType chapterPopup;
    public bool isConditional = false;
    public ConditionalTextDataType conditionalText;
}
