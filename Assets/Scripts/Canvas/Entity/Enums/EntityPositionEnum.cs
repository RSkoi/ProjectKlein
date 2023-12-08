public enum EntityPositionEnum
{
    /* below refers to Big-Endian
     * first bit is sign, next 12 bits are pos offset
     * basically two's complement
     * reference point is middle of screen (xPos 0)
     * e.g. 0b_1_01100_10000 == -400 */

    // TODO: the actual values might need to change if Unity's reference resolution changes
    //Right = 0b_0_01100_10000,
    //Left = 0b_1_01100_10000,
    //Middle = 0b_0_00000_00000, // 'hmm yes today I will use positive and negative zero'
                                 // statements made by the utterly deranged

    // TODO: the actual values might need to change if Unity's reference resolution changes
    Right = 250,
    Left = -250,
    Middle = 0,
}
