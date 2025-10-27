using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private InputAction attackAction;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        attackAction = InputSystem.actions.FindAction("attack");
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
