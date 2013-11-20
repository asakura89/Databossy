using System;

namespace Databossy.Sql
{
    public static class Join
    {
        public enum Type
        {
            NONE,
            LEFT,
            RIGHT,
            INNER,
            OUTER,
            LEFT_INNER,
            RIGHT_INNER,
            LEFT_OUTER,
            RIGHT_OUTER,
            CROSS
        }

        public static String GetType(Type type)
        {
            String joinString = String.Empty;
            switch (type)
            {
                case Type.LEFT:
                    joinString = "LEFT";
                    break;
                case Type.RIGHT:
                    joinString = "RIGHT";
                    break;
                case Type.LEFT_INNER:
                    joinString = "LEFT INNER";
                    break;
                case Type.LEFT_OUTER:
                    joinString = "LEFT OUTER";
                    break;
                case Type.RIGHT_INNER:
                    joinString = "RIGHT INNER";
                    break;
                case Type.RIGHT_OUTER:
                    joinString = "RIGHT OUTER";
                    break;
                case Type.OUTER:
                    joinString = "OUTER";
                    break;
                case Type.CROSS:
                    joinString = "CROSS";
                    break;
            }

            return joinString;
        }
    }
}