using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers
{

    /// <summary>
    /// Saves an animator's state.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    [RequireComponent(typeof(Animator))]
    public class AnimatorSaver : Saver
    {

        [Serializable]
        public class LayerData
        {
            public int hash;
            public float time;
        }

        // Credit: Trigger saving contributed by Magique.
        [Serializable]
        public class TriggerData
        {
            public string name;
            public bool isTriggered;
        }

        [Serializable]
        public class Data
        {
            public LayerData[] layers = null;
            public List<bool> bools = new List<bool>();
            public List<float> floats = new List<float>();
            public List<int> ints = new List<int>();
            public List<string> strings = new List<string>();
            public List<TriggerData> triggers = new List<TriggerData>();
        }

        private Data m_data = new Data();
        private Animator m_animator;
        private Animator animator
        {
            get
            {
                if (m_animator == null) m_animator = GetComponent<Animator>();
                return m_animator;
            }
        }

        private void CheckAnimator()
        {
            if (animator == null) return;
            if (m_data == null) m_data = new Data();
            if (m_data.layers == null || m_data.layers.Length != animator.layerCount)
            {
                m_data.layers = new LayerData[animator.layerCount];
                for (int i = 0; i < animator.layerCount; i++)
                {
                    m_data.layers[i] = new LayerData();
                }
            }
        }

        public override string RecordData()
        {
            if (animator == null) return string.Empty;
            CheckAnimator();

            // Record layer states:
            for (int i = 0; i < animator.layerCount; i++)
            {
                var state = animator.GetCurrentAnimatorStateInfo(i);
                m_data.layers[i].hash = state.fullPathHash;
                m_data.layers[i].time = state.normalizedTime;
            }

            // Record parameter values:
            int numBools = 0;
            int numFloats = 0;
            int numInts = 0;
            for (int i = 0; i < animator.parameterCount; i++)
            {
                var parameter = animator.parameters[i];
                switch (parameter.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        var boolValue = animator.GetBool(parameter.name);
                        if (numBools < m_data.bools.Count)
                        {
                            m_data.bools[numBools] = boolValue;
                        }
                        else
                        {
                            m_data.bools.Add(boolValue);
                        }
                        numBools++;
                        break;
                    case AnimatorControllerParameterType.Float:
                        var floatValue = animator.GetFloat(parameter.name);
                        if (numFloats < m_data.floats.Count)
                        {
                            m_data.floats[numFloats] = floatValue;
                        }
                        else
                        {
                            m_data.floats.Add(floatValue);
                        }
                        numFloats++;
                        break;
                    case AnimatorControllerParameterType.Int:
                        var intValue = animator.GetInteger(parameter.name);
                        if (numInts < m_data.ints.Count)
                        {
                            m_data.ints[numInts] = intValue;
                        }
                        else
                        {
                            m_data.ints.Add(intValue);
                        }
                        numInts++;
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        var triggerValue = animator.GetCurrentAnimatorStateInfo(0).IsName(parameter.name);
                        m_data.triggers.Add(new TriggerData() { isTriggered = triggerValue, name = parameter.name });
                        break;
                }
            }
            return SaveSystem.Serialize(m_data);
        }

        public override void ApplyData(string s)
        {
            if (string.IsNullOrEmpty(s) || animator == null) return;
            m_data = SaveSystem.Deserialize<Data>(s, m_data);
            if (m_data == null)
            {
                m_data = new Data();
            }
            else if (m_data.layers != null)
            {
                // Apply layer states:
                for (int i = 0; i < animator.layerCount; i++)
                {
                    if (i < m_data.layers.Length)
                    {
                        animator.Play(m_data.layers[i].hash, i, m_data.layers[i].time);
                    }
                }

                // Set or Reset triggers
                foreach (var trigger in m_data.triggers)
                {
                    if (trigger.isTriggered)
                    {
                        animator.SetTrigger(trigger.name);
                    }
                    else
                    {
                        animator.ResetTrigger(trigger.name);
                    }
                }

                // Apply parameter values:
                int numBools = 0;
                int numFloats = 0;
                int numInts = 0;
                for (int i = 0; i < animator.parameterCount; i++)
                {
                    var parameter = animator.parameters[i];
                    switch (parameter.type)
                    {
                        case AnimatorControllerParameterType.Bool:
                            if (numBools < m_data.bools.Count)
                            {
                                animator.SetBool(parameter.name, m_data.bools[numBools++]);
                            }
                            break;
                        case AnimatorControllerParameterType.Float:
                            if (numFloats < m_data.floats.Count)
                            {
                                animator.SetFloat(parameter.name, m_data.floats[numFloats++]);
                            }
                            break;
                        case AnimatorControllerParameterType.Int:
                            if (numInts < m_data.ints.Count)
                            {
                                animator.SetInteger(parameter.name, m_data.ints[numInts++]);
                            }
                            break;
                    }
                }
            }
        }

    }
}
