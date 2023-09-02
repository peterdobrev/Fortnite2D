using UnityEngine;

public interface IBullet
{
    void Move();
    void OnCollision();
    void SetDirection(Vector2 direction);
}