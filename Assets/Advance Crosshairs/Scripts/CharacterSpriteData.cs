using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSpriteData", menuName = "Game/Character Sprite Data")]
public class CharacterSpriteData : ScriptableObject
{
    [Header("Character Sprites")]
    public Sprite idle;
    public Sprite aim;
    public Sprite damage;
    public Sprite dead;
}