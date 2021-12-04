using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ValueRange {
    public int dieNum;
    public int dieSize;
    public int bonus;

    public int Max {
        get {
            return dieNum * dieSize + bonus;
        }
    }

    public int Min {
        get {
            return dieNum + bonus;
        }
    }

    public int Value {
        get {
            int sum = 0;
            for(int i = 0; i < dieNum; i++){
                sum += Random.Range(1, dieSize+1);
            }
            return sum + bonus;
        }
    }

    public ValueRange(int dieNum, int dieSize, int bonus = 0){
        this.dieNum = dieNum;
        this.dieSize = dieSize;
        this.bonus = bonus;
    }

    public int DieNumber { get { return dieNum; } }
    public int DieSize { get { return dieSize; } }
    public int DieBonus { get { return bonus; } }
}
[System.Serializable]
public class Damage : ValueRange
{
    public Damage(int dieNum, int dieSize, int bonus = 0) : base(dieNum, dieSize, bonus){
        
    }
    public Damage(Damage d) : base(d.dieNum, d.dieSize, d.bonus){
        
    }
}
