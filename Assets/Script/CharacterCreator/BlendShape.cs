//Defines the Blendshape class for use in AvatarCustomizer feature
public class Blendshape
{
    public int posIndex { get; set; }
    public int negIndex { get; set; }

    public Blendshape(int positiveIndex, int negativeIndex)
    {
        this.posIndex = positiveIndex;
        this.negIndex = negativeIndex;
    }

}