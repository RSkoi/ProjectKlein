using System;

[Serializable]
public class SaveStatesDataType
{
    public int dialogueState = -1;
    public int backgroundState = -1;
    public int entityState = -1;
    public int audioEffectsState = -1;
    public int bgSongsState = -1;
    public int particleSystemsState = -1;

    public SaveStatesDataType(
        int dialogueState,
        int backgroundState,
        int entityState,
        int audioEffectsState,
        int bgSongsState,
        int particleSystemsState)
    {
        this.dialogueState = dialogueState;
        this.backgroundState = backgroundState;
        this.entityState = entityState;
        this.audioEffectsState = audioEffectsState;
        this.bgSongsState = bgSongsState;
        this.particleSystemsState = particleSystemsState;
    }
}
