using System;
using VectorMath;

namespace BeautySim2023
{
    public class LimitDefinition
    {
        public Enum_TypeLimit typeLimit { get; private set; }
        public Enum_VariableTypeLimit variableTypeLimit { get; private set; }
        public Enum_VariableTypeLimit variableLimitExceeded { get; private set; }
        private float valueMin;
        private float valueMax;
        public float Perc { get; private set; }
        public float PercError { get; private set; }
        public bool ExceededMin { get; private set; }
        public bool ExceededMax { get; private set; }

        public LimitDefinition(float minValue, float maxValue, Enum_TypeLimit type, Enum_VariableTypeLimit valueLimit)
        {
            valueMin = minValue;
            valueMax = maxValue;
            typeLimit = type;
            variableTypeLimit = valueLimit;
        }

        public void Evaluate(float value, bool applyCorrection, float correction, bool applyAbs, bool isRoll)
        {

            if (!applyAbs)
            {
                float valueToConsider = value + (applyCorrection ? correction : 0);
                if (isRoll)
                {
                    if (valueToConsider > 180.0)
                    {
                        valueToConsider = -180 + (valueToConsider - 180);
                    }
                    else
                    {
                        if (valueToConsider < -180)
                        {
                            valueToConsider = 180 - ( - 180- valueToConsider);
                        }
                    }
                }


                Perc = (valueToConsider - valueMin) / (valueMax - valueMin) * 100;

                if ((Perc >= 0) && (Perc <= 100))
                {
                    PercError = 0;
                    ExceededMin = false; ExceededMax = false;
                }
                else
                {
                    if (Perc < 0)
                    {
                        PercError = Math.Abs(Perc);
                        ExceededMin = true; ExceededMax = false;
                    }
                    else
                    {
                        PercError = Perc - 100;
                        ExceededMin = false; ExceededMax = true;
                    }
                }
            }
            else
            {
                Perc = (Math.Abs(value) + (applyCorrection ? correction : 0) - valueMin) / (valueMax - valueMin) * 100;

                if ((Perc >= 0) && (Perc <= 100))
                {
                    PercError = 0;
                    ExceededMin = false; ExceededMax = false;
                }
                else
                {
                    if (value < 0)
                    {
                        PercError = Math.Abs(Perc);
                        ExceededMin = true; ExceededMax = false;
                    }
                    else
                    {
                        PercError = Math.Abs(Perc - 100);
                        ExceededMin = false; ExceededMax = true;
                    }
                }
            }
        }

        /// <summary>
        /// Evalutat distance froma a segment
        /// </summary>
        /// <param name="valueY"></param>
        /// <param name="valueZ"></param>
        /// <param name="applyCorrection"></param>
        /// <param name="correction"></param>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        public void Evaluate(float valueY, float valueZ, bool applyCorrection, float correction, Vector3 start, Vector3 finish, bool isRoll)
        {

            Vector2 st = new Vector2(start.Y, start.Z);
            Vector2 fi = new Vector2(finish.Y, finish.Z);
            Vector2 pp = new Vector2(valueY, valueZ);
            Vector2 dirSeg = fi - st; float lenDirSeg = dirSeg.Length;
            Vector2 vecPoint = pp - st; float lenVecPoint = vecPoint.Length;
            Vector2 closest = new Vector2();
            float c = Vector2.Dot(vecPoint, dirSeg)/lenDirSeg;
            dirSeg.Normalize();

            if (c> lenDirSeg)
            {
                closest = fi;
            }
            else
            {
                if (c<=0)
                {
                    closest = st;
                }
                else
                {
                    closest = st + Vector2.Multiply(dirSeg, c);
                }
            }

            Perc = c/ lenDirSeg * 100;



            //Evaluate the closest
            Vector2 dist = pp - closest;
            ExceededMin = false;
            ExceededMax = false;
            if ((Math.Abs(dist.X) > valueMax) || (Math.Abs(dist.Y)> valueMax))
            {
                if (Math.Abs(dist.X) > Math.Abs(dist.Y))
                {
                    if (dist.X < -valueMin)
                    {
                        ExceededMin = true;
                    }
                    if (dist.X > valueMax)
                    {
                        ExceededMax = true;
                    }
                    variableLimitExceeded = Enum_VariableTypeLimit.Y;
                }
                else
                {
                    if (dist.Y < -valueMin)
                    {
                        ExceededMin = true;
                    }
                    if (dist.Y > valueMax)
                    {
                        ExceededMax = true;
                    }
                    variableLimitExceeded = Enum_VariableTypeLimit.Z;
                }
            }
            if (ExceededMax || ExceededMin)
            {
                PercError = 100;
            }
            

        }

        public int GiveMeIndexImage(int minIndex, int maxIndex)
        {
            int indexImage = 0;

            indexImage = (int)(Math.Ceiling((Perc / 100.0) * ((float)maxIndex - (float)minIndex) + (float)minIndex));
            if ((indexImage >= maxIndex) || (indexImage < minIndex))
            {
                indexImage = -1;
            }

            return indexImage;
        }
    }
}