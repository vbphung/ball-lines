using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallAvatarUI : DragItem<Ball>
{
    [SerializeField] private Sprite hoverFrame;

    public void Setup(Sprite avatar)
    {
        Image avatarImage = GetComponent<Image>();
        var color = avatarImage.color;
        color.a = avatar ? 255f : 0f;
        avatarImage.color = color;
        avatarImage.sprite = avatar;
    }
}
