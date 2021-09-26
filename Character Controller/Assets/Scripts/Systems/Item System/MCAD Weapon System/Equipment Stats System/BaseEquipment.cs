using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;
using Dreamers.Global;

namespace EquipmentStats
{
    public partial class BaseEquipment : MonoBehaviour
    {
        private string _name;
        private Attributes[] _primaryAttribute;
        private Stat[] _stats;
        private ModAttributes[] _modAttributes;
 
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private void SetupPrimaryAttributes()
        {
            for (int cnt = 0; cnt < _primaryAttribute.Length; cnt++)
            {
                _primaryAttribute[cnt] = new Attributes();
            }
        }
        private void SetupModAttributes()
        {
            for (int cnt = 0; cnt < _primaryAttribute.Length; cnt++)
            {
                _primaryAttribute[cnt] = new Attributes();
            }
        }
        private void SetupStats()
        {
            for (int cnt = 0; cnt < _stats.Length; cnt++)
                _stats[cnt] = new Stat();
            SetupStatsModifiers();
        }

        public Attributes GetPrimaryAttribute(int index)
        {
            return _primaryAttribute[index];
        }
        public ModAttributes GetModAttribute(int index)
        {
            return _modAttributes[index];
        }
        public Stat GetStat(int index)
        {
            return _stats[index];
        }

        private void Awake()
        {
            _name = string.Empty;
            _primaryAttribute = new Attributes[Enum.GetValues(typeof(AttributeNames)).Length];
            _modAttributes = new ModAttributes[Enum.GetValues(typeof(ModAttributeNames)).Length];
            _stats = new Stat[Enum.GetValues(typeof(StatNames)).Length];
            SetupPrimaryAttributes();
            SetupModAttributes();
            SetupStats();
  
        }

        // Update is called once per frame


        void SetupStatsModifiers() {
            // Casting Level;
            GetStat((int)StatNames.Casting_Level).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeNames.Level), 1));
            //Cooling Rate
            GetStat((int)StatNames.Cooling_Rate).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeNames.Durablity), 1));
            GetStat((int)StatNames.Cooling_Rate).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeNames.Speed), 1));



        }

        public void StatUpdate() 
        {
            for (int j = 0; j < _stats.Length; j++)
                _stats[j].Update();
        }

        #region public functions


        #endregion
    }
}