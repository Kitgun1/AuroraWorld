using System;

namespace AuroraWorld.Core.Mathematics.Base
{
    public class CubeVector
    {
        public int Q { get; private set; }
        public int R { get; private set; }
        public int S { get; private set; }

        public CubeVector(int q, int r, int s)
        {
            SetValue(q, r, s);
        }
        
        public void SetValue(int q, int r, int s)
        {
            if (q + r + s != 0)
            {
                throw new Exception($"sum vector q:{q} + r:{r} + s:{s} is not 0!");
            }
            Q = q;
            R = r;
            S = s;
        }
    }

    public static class CubeVector
    {
        public const CubeVector zero = VALUE;
    }
}