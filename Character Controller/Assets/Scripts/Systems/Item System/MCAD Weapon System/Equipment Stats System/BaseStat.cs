
namespace EquipmentStats {
    [System.Serializable]
    public class BaseStat 
    {
        private int _baseValue;
        private int _buffValue;

        public BaseStat()
        {
            //_name = null;
            _baseValue = 0;
            _buffValue = 0;
        }
        #region setters and getters

        public int BaseValue
        {
            get { return _baseValue; }
            set { _baseValue = value; }
        }
        public int BuffValue
        {
            get { return _buffValue; }
            set { _buffValue = value; }
        }
 
        #endregion

        public int AdjustBaseValue
        {
            get { return _baseValue + _buffValue; }
        }
    }
}