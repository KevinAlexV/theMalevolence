using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to determine which targets are valid when the player selects from targets
public delegate bool TargetSelector(ITargetable target);
public interface ITargetable{
    //As of now (10/15), ITargetable can be left empty and only added to if more than just characters can be target

    // TargetTypeEnum GetType();
    
    // //Should return true if object is of type T, and pass out a reference to the object of type T. Otherwise, return false and pass out null
    // bool TryGetAsBaseClass<T>(out T type);
}