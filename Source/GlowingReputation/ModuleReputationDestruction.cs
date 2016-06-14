using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlowingReputation
{
    public class ModuleReputationDestruction:PartModule
    {
        [KSPField(isPersistant = false)]
        public bool SafeUntilFirstActivation = true;

        [KSPField(isPersistant = false)]
        public float BaseReputationHit = 15f;

        [KSPField(isPersistant = true)]
        public bool Unsafe = false;

        public void Start()
        {
            GameEvents.onPartDestroyed.Add(new EventData<Part>.OnEvent(OnPartDestroyed));
        }

        public void Update()
        {
            if (SafeUntilFirstActivation && Unsafe)
            {
                EvaluateSafety();
            }
        }

        protected void OnPartDestroyed(Part p)
        {
            
        }

        protected void EvaluateSafety()
        {
            // check this for all potential conditions
            // ModuleResourceConverter
            // ModuleEnginesFX
        }
    }
}
