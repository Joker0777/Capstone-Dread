using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter2D
{
    public void Move(Vector2 moveInput);

    public void JumpButtonDown();
    public void JumpButtonUp();

    public void SprintOn();
    public void SprintOff();



}
