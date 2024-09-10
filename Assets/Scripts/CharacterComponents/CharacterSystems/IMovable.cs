using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public interface IMovable
    {
        public void Move(Vector2 direction);
    void SetRunning(bool isRunning);
    void SetCrouching(bool isCrouching);
    void Jump();
}
