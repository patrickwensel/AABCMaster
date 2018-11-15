using System;

namespace Dymeng.Framework.Validation
{
    public static class ValidationHelpers
    {

        //public static bool AreAllOverlapping(params Tuple<DateTime, DateTime>[] ranges) {

        //    for (int i = 0; i < ranges.Length; i++) {
        //        for (int j = i + 1; j < ranges.Length; j++) {
        //            if (!(ranges[i].Item1 <= ranges[j].Item2 && ranges[i].Item2 >= ranges[j].Item1)) {
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}

        //public static bool AreAnyOverlapping(params Tuple<DateTime, DateTime>[] ranges) {

        //    for (int i = 0; i < ranges.Length; i++) {
        //        for (int j = i + 1; j < ranges.Length; j++) {
        //            if (ranges[i].Item1 <= ranges[j].Item2 && ranges[i].Item2 >= ranges[j].Item1) {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        public static bool AreAllOverlapping(params Tuple<DateTime, DateTime>[] ranges)
        {

            for (int i = 0; i < ranges.Length; i++)
            {
                for (int j = i + 1; j < ranges.Length; j++)
                {
                    if (!(ranges[i].Item1 < ranges[j].Item2 && ranges[i].Item2 > ranges[j].Item1))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool AreAnyOverlapping(params Tuple<DateTime, DateTime>[] ranges)
        {

            for (int i = 0; i < ranges.Length; i++)
            {
                for (int j = i + 1; j < ranges.Length; j++)
                {
                    if (ranges[i].Item1 < ranges[j].Item2 && ranges[i].Item2 > ranges[j].Item1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
