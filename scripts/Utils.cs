using Godot;

namespace Utils{
    public enum Dir
    {
        None,
        Right,
        Left,
        Up,
        Down,
    }

    public static class Utils{
        // transform r and c to get the first array index to acces the array in a specified direction
        public static int GetIdx2(Dir dir,int r, int c)
        {
            switch(dir)
            {   case Dir.Right:
                case Dir.Left:
                    return r;
                case Dir.Down:
                    return 3-c;
                default:
                    return c;
            }
        }

        // transform r and c to get the first array index to acces the array in a specified direction
        public static int GetIdx1(Dir dir, int r, int c)
        {
            switch(dir)
            {
                case Dir.Right:
                    return 3-c;
                case Dir.Left:
                    return c;
                default:
                    return r;
            }
        }
    }
}
