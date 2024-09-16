using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICompletionElement
{ 
    public bool isComplete { get; set; } 

    public bool isFailed {  get; set; }
}
