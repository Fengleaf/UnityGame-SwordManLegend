using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    public Vector2 GetAnimationPosition(string name)
    {
        float x = transform.parent.localScale.x;
        // x < 0: Player face right.
        // x > 0: Player face left.
        if (name == "Slash")
        {
            if (x < 0)
                return new Vector2(0.9757992f, 0.4289431f);
            else
                return new Vector2(-0.9757992f, 0.4289431f);
        }
        else if (name == "Slash2")
        {
            if (x < 0)
                return new Vector2(0.4947992f, 0.4289431f);
            else
                return new Vector2(-0.4947992f, 0.4289431f);
        }
        else if (name == "Slash3") 
        {
            if (x < 0)
                return new Vector2(0.5f, 0.0519431f);
            else
                return new Vector2(-0.5f, 0.0519431f);
        }
        return new Vector2();
    }

    public Vector2 GetAnimationDirection(string name)
    {
        float x = transform.parent.localScale.x;
        if (name == "Slash")
        {
            if (x < 0)
                return new Vector2(1, 0);
            else
                return new Vector2(-1, 0);
        }
        else if (name == "Slash2")
        {
            if (x < 0)
                return new Vector2(1, 0);
            else
                return new Vector2(-1, 0);
        }
        else if (name == "Slash3")
        {
            if (x < 0)
                return new Vector2(1, 0);
            else
                return new Vector2(-1, 0);
        }
        return new Vector2();
    }

    public float GetAnimationRotationZ(string name)
    {
        float x = transform.parent.localScale.x;
        if (name == "Slash")
        {
            if (x < 0)
                return -30.0f;
            else
                return 30.0f;
        }
        else if (name == "Slash2")
        {
            if (x < 0)
                return 30.0f;
            else
                return -30.0f;
        }
        return 0;
    }
}
