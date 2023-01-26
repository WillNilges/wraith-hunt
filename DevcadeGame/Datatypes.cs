using System;

namespace DevcadeGame
{
    public enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }
    public enum WHPlane
    {
        MATERIAL,
        ETHEREAL
    }
    public enum AETag
    {
        WORLD, // For maptiles and other solid things part of the material geometry
        MEDIUM, // Is, or originated from, the Medium
        WRAITH, // Is, originated from, the Wraith
        MEDIUMATTACK, // An attack from the Medium
        WRAITHATTACK, // An attack from the Wraith
        KILLPLANE, // Touch this, you die
        NONE // For things that shouldn't collide at all, like the background.
    }
}