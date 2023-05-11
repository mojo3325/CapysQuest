using UnityEngine;

[CreateAssetMenu(fileName = "SkinData", menuName = "Custom/SkinData", order = 1)]
public class SkinData : ScriptableObject
{
    public string skinName;
    public Sprite skinIcon;
    public Sprite bodySprite;
    public Sprite tearSprite;
    public Sprite backLeftPawSprite;
    public Sprite backRightPawSprite;
    public Sprite forwLeftPawSprite;
    public Sprite forwRightPawSprite;
    public Sprite backEar;
    public Sprite forwEar;
    public Sprite mustacheSprite;
}

