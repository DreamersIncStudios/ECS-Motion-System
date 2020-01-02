using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stats;

// Consider Generalizing this script and adding inheritance

public class EnemySetup : MonoBehaviour
{
    EnemyCharacter EnemyStats;
    public CharacterClass CharClass;
    private void Awake()
    {
        EnemyStats = this.GetComponent<EnemyCharacter>();
    }


    // Start is called before the first frame update
    void Start()
    {
        EnemyStats.Name = CharClass.title.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StatsUpdate () {
        EnemyStats.Level = CharClass.Level;
        float ModValue = CharClass.diffultyMod * CharClass.LevelMod * CharClass.Level;
        EnemyStats.GetPrimaryAttribute((int)AttributeName.Strength).BaseValue = (int)(CharClass.Strength *ModValue);
        EnemyStats.GetPrimaryAttribute((int)AttributeName.Awareness).BaseValue = (int)(CharClass.Awareness * ModValue);
        EnemyStats.GetPrimaryAttribute((int)AttributeName.Charisma).BaseValue = (int)(CharClass.Charisma * ModValue);
        EnemyStats.GetPrimaryAttribute((int)AttributeName.Resistance).BaseValue = (int)(CharClass.Resistance * ModValue);
        EnemyStats.GetPrimaryAttribute((int)AttributeName.WillPower).BaseValue = (int)(CharClass.WillPower * ModValue);
        EnemyStats.GetPrimaryAttribute((int)AttributeName.Vitality).BaseValue = (int)(CharClass.Vitality * ModValue);
        EnemyStats.GetPrimaryAttribute((int)AttributeName.Skill).BaseValue = (int)(CharClass.Skill * ModValue);
        EnemyStats.GetPrimaryAttribute((int)AttributeName.Speed).BaseValue = (int)(CharClass.Speed * ModValue);
        EnemyStats.GetPrimaryAttribute((int)AttributeName.Luck).BaseValue = (int)(CharClass.Luck * ModValue);
        EnemyStats.GetPrimaryAttribute((int)AttributeName.Concentration).BaseValue = (int)(CharClass.Concentration * ModValue);
        EnemyStats.GetVital((int)VitalName.Health).BaseValue = 50;
        EnemyStats.GetVital((int)VitalName.Mana).BaseValue = 25;

    }
}
