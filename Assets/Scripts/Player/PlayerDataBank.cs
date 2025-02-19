using System;
using UnityEngine;

public class PlayerDataBank
{
    public float maxHP;
    public float currentHP;
    
    public int power;
    public int speed;
    public int magic;
    public int skill;
    public int luck;
    
    public int statPoints;
    
    public float xp;
    public int level;
    public int gold;
    
    public int ore;
    //public int potions;
    public int spellSlots;
    
    public int oreCap;
    //public int potionCap;
    public int spellCap;
    
    public void Initialize()
    {
        oreCap = 100;
        AddOre(10);
        AddGold(100);
        
        maxHP = 100;
        currentHP = maxHP;
        
        power = speed = magic = 5;
        skill = luck = 1;
    }

    public void LevelUpHP()
    {
        if (statPoints > 0)
        {
            maxHP += 10;
            currentHP += 10;
            statPoints--;
        }
    }

    public void LevelUpPower()
    {
        if (statPoints > 0)
        {
            power++;
            statPoints--;
        }
    }

    public void LevelUpSpeed()
    {
        if (statPoints > 0)
        {
            speed++;
            statPoints--;
        }
    }

    public void LevelUpMagic()
    {
        if (statPoints > 0)
        {
            magic++;
            statPoints--;
        }
    }

    public void LevelUpSkill()
    {
        if (statPoints > 0)
        {
            skill++;
            statPoints--;
        }
    }

    public void LevelUpLuck()
    {
        if (statPoints > 0)
        {
            luck++;
            statPoints--;
        }
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);
    }
    
    public bool CanHarvest()
    {
        return (ore < oreCap);
    }
    
    public void AddOre(int oresToGain)
    { ore += oresToGain; }

    public void SpendOre(int oresToSpend)
    { ore -= oresToSpend; }

    public void AddGold(int goldToGain)
    { gold += goldToGain; }

    public void SpendGold(int goldToSpend)
    { gold -= goldToSpend; }

    public float GetNextLevelCost()
    {
        return 100 + (20 * Mathf.Pow(1.1f, level - 1));
    }

    public void AddXP(float xpToGain)
    {
        xp += xpToGain;
        
        if (xp >= GetNextLevelCost())
        {
            AddLevel(false);
        }
    }

    public void AddLevel(bool isFree)
    {
        statPoints += 1 + (level / 3);
        
        xp -= GetNextLevelCost();
        
        if (isFree) 
            xp = 0;
        
        level++;
        currentHP = maxHP;
    }
}