using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlowingReputation
{
    class BodyReputationData
    {
        public string bodyName;
        public FloatCurve bodyRepCurve;

        public BodyReputationData(string nm)
        {
            bodyName = nm;
        }

        public float GetReputationScale(double altitude)
        {
            return bodyRepCurve.Evaluate((float)altitude);
        }
    }
}
