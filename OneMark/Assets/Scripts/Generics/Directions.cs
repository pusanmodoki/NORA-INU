//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Directions
{
    /// <summary>
    /// 方向を示すDirectionType
    /// </summary>
    public enum DirectionType
    {
        /// <summary>transform.forward</summary>
        Forward = 0x12,
        /// <summary>-transform.forward</summary>
        Back = 0x10,
        /// <summary>transform.up</summary>
        Up = 0x22,
        /// <summary>-transform.up</summary>
        Down = 0x20,
        /// <summary>transform.right</summary>
        Right = 0x42,
        /// <summary>-transform.right</summary>
        Left = 0x40,
    }
    /// <summary>
    /// Directionをビット変数として格納したDirectionBits
    /// </summary>
    struct DirectionBits
    {
        /// <summary>transform.forward or -transform.forward</summary>
        public static readonly int cForwardBit = 0x10;
        /// <summary>transform.forward</summary>
        public static readonly int cForward = 0x12;
        /// <summary>-transform.forward</summary>
        public static readonly int cBack = 0x10;

        /// <summary>transform.up or -transform.up</summary>
        public static readonly int cUpBit = 0x20;
        /// <summary>transform.up</summary>
        public static readonly int cUp = 0x22;
        /// <summary>-transform.up</summary>
        public static readonly int cDown = 0x20;

        /// <summary>transform.right or -transform.right</summary>
        public static readonly int cRightBit = 0x40;
        /// <summary>transform.right</summary>
        public static readonly int cRight = 0x42;
        /// <summary>-transform.right</summary>
        public static readonly int cLeft = 0x40;
    }

    /// <summary>
    /// [GetDirection]
    /// return: m_directionに沿った方向
    /// 引数1: type
    /// 引数2: transform
    /// </summary>
    public static Vector3 GetDirection(DirectionType type, Transform transform)
    {
        //intに変換
        int toBit = (int)type;

        //各ビットでどの方向変数を使うか求めた後、-1 or 1で掛け算して返却
        if ((toBit & DirectionBits.cForwardBit) != 0)
            return transform.forward * ((toBit & 0x2) - 1);
        else if ((toBit & DirectionBits.cUpBit) != 0)
            return transform.up * ((toBit & 0x2) - 1);
        else if ((toBit & DirectionBits.cRightBit) != 0)
            return transform.right * ((toBit & 0x2) - 1);
        else
            return Vector3.zero;
    }
}
