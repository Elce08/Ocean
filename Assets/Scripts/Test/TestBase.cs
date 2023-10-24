using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestBase : MonoBehaviour
{
    InputAction inputActions;

    protected virtual void Awake()
    {
        inputActions = new InputAction();
    }

    protected virtual void OnEnable()
    {
        //inputActions.Test.Enable();
        //inputActions.Test.Test1.performed += Test1;
        //inputActions.Test.Test2.performed += Test2;
        //inputActions.Test.Test3.performed += Test3;
    }

    protected virtual void OnDisable()
    {
        //inputActions.Test.Disable();
        //inputActions.Test.Test1.performed -= Test1;
        //inputActions.Test.Test2.performed -= Test2;
        //inputActions.Test.Test3.performed -= Test3;
    }

    protected virtual void Test1(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        
    }
    protected virtual void Test2(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        
    }
    protected virtual void Test3(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        
    }
    protected virtual void Test4(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        
    }
    protected virtual void Test5(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        
    }
}
