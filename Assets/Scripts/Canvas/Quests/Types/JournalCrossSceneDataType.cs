using System;

[Serializable]
public class JournalCrossSceneDataType
{
    public QuestReferenceDataType[] questStates;

    public JournalCrossSceneDataType(QuestReferenceDataType[] questStates)
    {
        this.questStates = questStates;
    }
}
